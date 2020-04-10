using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Trakx.Common.Interfaces;
using Trakx.Common.Interfaces.Index;
using Trakx.Common.Utils;
using Trakx.MarketData.Collector.CryptoCompare;
using Trakx.MarketData.Collector.CryptoCompare.DTOs.Inbound;
using Trakx.MarketData.Collector.CryptoCompare.DTOs.Outbound;
using Trakx.Tests.Data;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.MarketData.Collector.Tests.Unit
{
    public sealed class WorkerTests : IDisposable
    {
        private readonly IIndexDataProvider _indexDataProvider;
        private readonly ICryptoCompareWebSocketClient _webSocketClient;
        private readonly IDistributedCache _cache;
        private readonly MockCreator _mockCreator;
        private readonly CancellationToken _cancellationToken;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly Worker _worker;

        public WorkerTests(ITestOutputHelper output)
        {
            _indexDataProvider = Substitute.For<IIndexDataProvider>();
            _webSocketClient = Substitute.For<ICryptoCompareWebSocketClient>();
            var webSocketStreamer = Substitute.For<IWebSocketStreamer>();
            webSocketStreamer.AggregateIndexStream.Returns(Observable.Empty<AggregateIndex>());
            webSocketStreamer.HeartBeatStream.Returns(Observable.Empty<HeartBeat>());
            _webSocketClient.WebSocketStreamer.Returns(webSocketStreamer);
            _cache = Substitute.For<IDistributedCache>();
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;

            var logger = Substitute.For<ILogger<Worker>>();
            _worker = new Worker(_indexDataProvider, _webSocketClient, _cache, logger, TimeSpan.FromMilliseconds(50));

            _mockCreator = new MockCreator();
        }

        [Fact]
        public async Task StartAsync_should_wait_for_indexDataProvider_to_have_content()
        {
            var indexesEventuallyReturned = new List<string> { _mockCreator.GetRandomIndexSymbol() };
            _ = _indexDataProvider.GetAllIndexSymbols(_cancellationToken)
                .Returns(_ => { throw new TimeoutException("failed to connect to database"); },
                    _ => new List<string>(),
                    _ => indexesEventuallyReturned);

            await _worker.StartAsync(_cancellationToken);

            await _indexDataProvider.Received(3).GetAllIndexSymbols(_cancellationToken);
        }

        [Fact]
        public async Task StartAsync_should_subscribe_to_all_active_component_tickers_and_add_usdc()
        {
            var expectedSymbols = PrepareIndexDataProviderExpectations();

            await _worker.StartAsync(_cancellationToken);

            await _indexDataProvider.Received(1).GetAllIndexSymbols(_cancellationToken);
            _ = _indexDataProvider.Received(1).GetAllComponentsFromCurrentCompositions(_cancellationToken);
            await _webSocketClient.Received(1).AddSubscriptions(Arg.Any<ICryptoCompareSubscription[]>());

            var addSubscriptionCall = _webSocketClient.ReceivedCalls()
                .Single(c => c.GetMethodInfo().Name == nameof(_webSocketClient.AddSubscriptions));
            var addSubscriptionArg = (AggregateIndexSubscription[])addSubscriptionCall.GetArguments()[0];

            addSubscriptionArg.Select(a => a.BaseCurrency).Should().BeEquivalentTo(expectedSymbols);
            addSubscriptionArg.Select(a => a.QuoteCurrency)
                .All(q => q.Equals("USD", StringComparison.InvariantCultureIgnoreCase)).Should().BeTrue();
        }

        [Fact]
        public async Task Receiving_a_non_usdc_price_update_should_store_both_usdc_converted_and_non_converted_price()
        {
            _ = PrepareIndexDataProviderExpectations();

            var usdcPrice = 0.999m;
            var abcPrice = 1.2m;

            _cache.GetAsync("usdc".GetLatestPriceCacheKey("USD"), _cancellationToken).Returns(usdcPrice.GetBytes());

            var message = new AggregateIndex { FromSymbol = "ABC", ToSymbol = "USD", Price = 1.2m };
            AddMessageToWebSocketStream(message);

            await _worker.StartAsync(_cancellationToken);

            await CheckUpdateWasSetInCache(message, abcPrice/usdcPrice, "usdc");

            await CheckUpdateWasSetInCache(message, abcPrice);
        }

        [Fact]
        public async Task Receiving_a_non_usdc_price_update_should_only_store_unconverted_price_if_usdc_price_not_available()
        {
            _ = PrepareIndexDataProviderExpectations();

            var abcPrice = 1.2m;

            var message = new AggregateIndex {FromSymbol = "ABC", ToSymbol = "EUR", Price = 1.2m};
            AddMessageToWebSocketStream(message);

            await _worker.StartAsync(_cancellationToken);

            await CheckUpdateWasSetInCache(message, abcPrice);
        }

        [Fact]
        public async Task Receiving_a_usdc_price_update_should_store_price_without_converting_it()
        {
            _ = PrepareIndexDataProviderExpectations();

            var usdcPrice = 0.999m;

            var message = new AggregateIndex {FromSymbol = "USDC", ToSymbol = "GBP", Price = usdcPrice};
            AddMessageToWebSocketStream(message);

            await _worker.StartAsync(_cancellationToken);

            await CheckUpdateWasSetInCache(message, usdcPrice);
        }

        private IEnumerable<string> PrepareIndexDataProviderExpectations()
        {
            var indexes = new List<string> {_mockCreator.GetRandomIndexSymbol()};
            _indexDataProvider.GetAllIndexSymbols(_cancellationToken).Returns(indexes);

            var components = _mockCreator.GetIndexComposition(3).ComponentQuantities
                .Select(c => c.ComponentDefinition);
            _indexDataProvider.GetAllComponentsFromCurrentCompositions(_cancellationToken)
                .Returns(components.ToAsyncEnumerable());

            var expectedSymbols = components.Select(c => c.Symbol.ToUpper()).Union(new[] {"USDC"});
            return expectedSymbols;
        }

        private void AddMessageToWebSocketStream(AggregateIndex message)
        {
            var receivedMessages = new[] {message};
            _webSocketClient.WebSocketStreamer.AggregateIndexStream.Returns(receivedMessages.ToObservable());
        }

        private async Task CheckUpdateWasSetInCache(AggregateIndex message, decimal abcPrice, string overrideToSymbol = default)
        {
            var toSymbol = overrideToSymbol ?? message.ToSymbol;
            await _cache.Received(1).SetAsync(Arg.Is(message.FromSymbol.GetLatestPriceCacheKey(toSymbol)),
                Arg.Is<byte[]>(b => b.ToDecimal() == abcPrice),
                Arg.Any<DistributedCacheEntryOptions>(),
                Arg.Is(_cancellationToken));
        }

        #region Implementation of IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        #endregion
    }
}