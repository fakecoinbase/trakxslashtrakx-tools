using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Trakx.Common.Interfaces;
using Trakx.Common.Interfaces.Indice;
using Trakx.Common.Utils;
using Trakx.MarketData.Collector.CryptoCompare;
using Trakx.MarketData.Collector.CryptoCompare.DTOs.Inbound;
using Trakx.MarketData.Collector.CryptoCompare.DTOs.Outbound;
using Trakx.Tests.Data;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.MarketData.Collector.Tests.Unit
{
    public sealed class PriceCacheTests : IDisposable
    {
        private readonly IIndiceDataProvider _indiceDataProvider;
        private readonly ICryptoCompareWebSocketClient _webSocketClient;
        private readonly IDistributedCache _cache;
        private readonly MockCreator _mockCreator;
        private readonly CancellationToken _cancellationToken;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly PriceCache _priceCache;

        public PriceCacheTests(ITestOutputHelper output)
        {
            _indiceDataProvider = Substitute.For<IIndiceDataProvider>();
            _webSocketClient = Substitute.For<ICryptoCompareWebSocketClient>();
            var webSocketStreamer = Substitute.For<IWebSocketStreamer>();
            webSocketStreamer.AggregateIndiceStream.Returns(Observable.Empty<AggregateIndice>());
            webSocketStreamer.HeartBeatStream.Returns(Observable.Empty<HeartBeat>());
            _webSocketClient.WebSocketStreamer.Returns(webSocketStreamer);
            _cache = Substitute.For<IDistributedCache>();
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;

            var logger = output.ToLogger<PriceCache>();

            var serviceScopeFactory = PrepareScopeResolution();

            _priceCache = new PriceCache(_webSocketClient, _cache, 
                serviceScopeFactory, logger, 
                TimeSpan.FromMilliseconds(50));

            _mockCreator = new MockCreator(output);
        }

        private IServiceScopeFactory PrepareScopeResolution()
        {
            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService<IIndiceDataProvider>().Returns(_indiceDataProvider);
            var serviceScope = Substitute.For<IServiceScope>();
            serviceScope.ServiceProvider.Returns(serviceProvider);
            var serviceScopeFactory = Substitute.For<IServiceScopeFactory>();
            serviceScopeFactory.CreateScope().Returns(serviceScope);
            return serviceScopeFactory;
        }

        [Fact]
        public async Task StartAsync_should_wait_for_indiceDataProvider_to_have_content()
        {
            var indiceesEventuallyReturned = new List<string> { _mockCreator.GetRandomIndiceSymbol() };
            _ = _indiceDataProvider.GetAllIndiceSymbols(_cancellationToken)
                .Returns(_ => { throw new TimeoutException("failed to connect to database"); },
                    _ => new List<string>(),
                    _ => indiceesEventuallyReturned);

            _indiceDataProvider.GetAllComponentsFromCurrentCompositions(_cancellationToken)
                .Returns(new List<IComponentDefinition>());

            await _priceCache.StartCaching(_cancellationToken);

            await _indiceDataProvider.Received(3).GetAllIndiceSymbols(_cancellationToken);
        }

        [Fact]
        public async Task StartAsync_should_subscribe_to_all_active_component_tickers_and_add_usdc()
        {
            var expectedSymbols = PrepareIndiceDataProviderExpectations();

            await _priceCache.StartCaching(_cancellationToken);

            await _indiceDataProvider.Received(1).GetAllIndiceSymbols(_cancellationToken);
            _ = _indiceDataProvider.Received(1).GetAllComponentsFromCurrentCompositions(_cancellationToken);
            await _webSocketClient.Received(1).AddSubscriptions(Arg.Any<ICryptoCompareSubscription[]>());

            var addSubscriptionCall = _webSocketClient.ReceivedCalls()
                .Single(c => c.GetMethodInfo().Name == nameof(_webSocketClient.AddSubscriptions));
            var addSubscriptionArg = (AggregateIndiceSubscription[])addSubscriptionCall.GetArguments()[0];

            addSubscriptionArg.Select(a => a.BaseCurrency).Should().BeEquivalentTo(expectedSymbols);
            addSubscriptionArg.Select(a => a.QuoteCurrency)
                .All(q => q.Equals("USD", StringComparison.InvariantCultureIgnoreCase)).Should().BeTrue();
        }

        [Fact]
        public async Task Receiving_a_non_usdc_price_update_should_store_both_usdc_converted_and_non_converted_price()
        {
            _ = PrepareIndiceDataProviderExpectations();

            var usdcPrice = 0.999m;
            var abcPrice = 1.2m;

            _cache.GetAsync("usdc".GetLatestPriceCacheKey("USD"), _cancellationToken).Returns(usdcPrice.GetBytes());

            var message = new AggregateIndice { FromSymbol = "ABC", ToSymbol = "USD", Price = 1.2m };
            AddMessageToWebSocketStream(message);

            await _priceCache.StartCaching(_cancellationToken);

            await CheckUpdateWasSetInCache(message, abcPrice/usdcPrice, "usdc");

            await CheckUpdateWasSetInCache(message, abcPrice);
        }

        [Fact]
        public async Task Receiving_a_non_usdc_price_update_should_only_store_unconverted_price_if_usdc_price_not_available()
        {
            _ = PrepareIndiceDataProviderExpectations();

            var abcPrice = 1.2m;

            var message = new AggregateIndice {FromSymbol = "ABC", ToSymbol = "EUR", Price = 1.2m};
            AddMessageToWebSocketStream(message);

            await _priceCache.StartCaching(_cancellationToken);

            await CheckUpdateWasSetInCache(message, abcPrice);
        }

        [Fact]
        public async Task Receiving_a_usdc_price_update_should_store_price_without_converting_it()
        {
            _ = PrepareIndiceDataProviderExpectations();

            var usdcPrice = 0.999m;

            var message = new AggregateIndice {FromSymbol = "USDC", ToSymbol = "GBP", Price = usdcPrice};
            AddMessageToWebSocketStream(message);

            await _priceCache.StartCaching(_cancellationToken);

            await CheckUpdateWasSetInCache(message, usdcPrice);
        }

        private IEnumerable<string> PrepareIndiceDataProviderExpectations()
        {
            var indicees = new List<string> {_mockCreator.GetRandomIndiceSymbol()};
            _indiceDataProvider.GetAllIndiceSymbols(_cancellationToken).Returns(indicees);

            var components = _mockCreator.GetIndiceComposition(3).ComponentQuantities
                .Select(c => c.ComponentDefinition).ToList();
            _indiceDataProvider.GetAllComponentsFromCurrentCompositions(_cancellationToken)
                .Returns(components);

            var expectedSymbols = components.Select(c => c.Symbol.ToUpper()).Union(new[] {"USDC"});
            return expectedSymbols;
        }

        private void AddMessageToWebSocketStream(AggregateIndice message)
        {
            var receivedMessages = new[] {message};
            _webSocketClient.WebSocketStreamer.AggregateIndiceStream.Returns(receivedMessages.ToObservable());
        }

        private async Task CheckUpdateWasSetInCache(AggregateIndice message, decimal abcPrice, string overrideToSymbol = default)
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