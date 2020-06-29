using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using CryptoCompare;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Reactive.Testing;
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
        private readonly ICryptoCompareClient _restSocketClient;
        private readonly TestScheduler _testScheduler;

        public PriceCacheTests(ITestOutputHelper output)
        {
            _indiceDataProvider = Substitute.For<IIndiceDataProvider>();
            _webSocketClient = Substitute.For<ICryptoCompareWebSocketClient>();
            _restSocketClient = Substitute.For<ICryptoCompareClient>();
            var webSocketStreamer = Substitute.For<IWebSocketStreamer>();
            webSocketStreamer.AggregateIndiceStream.Returns(Observable.Empty<AggregateIndex>());
            webSocketStreamer.HeartBeatStream.Returns(Observable.Empty<HeartBeat>());
            _webSocketClient.WebSocketStreamer.Returns(webSocketStreamer);
            _cache = Substitute.For<IDistributedCache>();
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;

            var logger = output.ToLogger<PriceCache>();

            var serviceScopeFactory = PrepareScopeResolution();

            _testScheduler = new TestScheduler();
            _priceCache = new PriceCache(_webSocketClient, _restSocketClient, _cache,
                serviceScopeFactory, logger,
                TimeSpan.FromMilliseconds(50), _testScheduler);

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
            var componentsEventuallyReturned = _mockCreator.GetIndiceComposition(3).ComponentQuantities
                .Select(c => c.ComponentDefinition).ToList();
            _ = _indiceDataProvider.GetAllComponentsFromCurrentCompositions(_cancellationToken)
                .Returns(_ => { throw new TimeoutException("failed to connect to database"); },
                    _ => new List<IComponentDefinition>(),
                    _ => componentsEventuallyReturned);

            await _priceCache.StartCaching(_cancellationToken);

            await _indiceDataProvider.Received(3).GetAllComponentsFromCurrentCompositions(_cancellationToken);
        }

        [Fact]
        public async Task StartAsync_should_subscribe_to_all_active_component_tickers_and_add_usdc()
        {
            var expectedSymbols = PrepareIndiceDataProviderExpectations();

            await _priceCache.StartCaching(_cancellationToken);

            await _indiceDataProvider.Received(1).GetAllComponentsFromCurrentCompositions(_cancellationToken);
            await _webSocketClient.Received(1).AddSubscriptions( 
                Arg.Is<ICryptoCompareSubscription[]>(a => a.Length == expectedSymbols.Count()));

            var addSubscriptionCall = _webSocketClient.ReceivedCalls()
                .Single(c => c.GetMethodInfo().Name == nameof(_webSocketClient.AddSubscriptions));
            var addSubscriptionArg = (AggregateIndexSubscription[])addSubscriptionCall.GetArguments()[0];

            addSubscriptionArg.Select(a => a.BaseCurrency.ToLowerInvariant()).Should().BeEquivalentTo(expectedSymbols);
            addSubscriptionArg.Select(a => a.QuoteCurrency)
                .All(q => q.Equals("USD", StringComparison.InvariantCultureIgnoreCase)).Should().BeTrue();
        }

        [Fact]
        public async Task StartAsync_should_add_symbols_to_AllConstituentsSymbols()
        {
            var expectedSymbols = PrepareIndiceDataProviderExpectations();

            await _priceCache.StartCaching(_cancellationToken);

            _priceCache.AllConstituentsSymbols.Should().BeEquivalentTo(expectedSymbols);
        }

        [Fact]
        public async Task Receiving_subscription_complete_should_add_symbol_to_websocket()
        {
            var expectedSymbols = PrepareIndiceDataProviderExpectations().ToList();
            var websocketSubscriptionsSucceededSymbols = SimulateReceiveSubscribeCompleteFor2Symbols(expectedSymbols);

            await _priceCache.StartCaching(_cancellationToken);
            
            _priceCache.WebSocketSourcedSymbols.Should().BeEquivalentTo(websocketSubscriptionsSucceededSymbols);
        }

        [Fact]
        public async Task Receiving_load_complete_should_assign_rest_symbols()
        {
            var expectedSymbols = PrepareIndiceDataProviderExpectations().ToList();
            var websocketSubscriptionsSucceededSymbols = SimulateReceiveSubscribeCompleteFor2Symbols(expectedSymbols);

            var loadCompleteObservable = GetStartableLoadComplete();

            await _priceCache.StartCaching(_cancellationToken);

            _priceCache.RestSourcedSymbols.Should()
                .BeEmpty("receiving load complete is required to fill in this list.");

            loadCompleteObservable.Connect();

            _priceCache.RestSourcedSymbols.Should()
                .BeEquivalentTo(expectedSymbols);
        }

        private IConnectableObservable<LoadComplete> GetStartableLoadComplete()
        {
            var loadComplete = new LoadComplete();
            var loadCompleteObservable = new[] {loadComplete}.ToObservable().Publish();
            _webSocketClient.WebSocketStreamer.LoadCompleteStream.Returns(loadCompleteObservable);
            return loadCompleteObservable;
        }

        [Fact]
        public async Task StartAsync_should_poll_Rest_api_for_all_RestSourcedSymbols()
        {
            var expectedSymbols = PrepareIndiceDataProviderExpectations().ToList();
            var loadCompleteObservable = GetStartableLoadComplete();

            using var disposable = _testScheduler.Schedule(PriceCache.RestPollingInterval.Multiply(2.1), 
                () => _priceCache.StopPolling());

            await _priceCache.StartCaching(_cancellationToken);
            loadCompleteObservable.Connect();
            _testScheduler.Start();

            await _restSocketClient.Prices.Received(3).MultipleSymbolsPriceAsync(
                Arg.Is<IEnumerable<string>>(l =>
                    expectedSymbols.TrueForAll(l.Contains) && l.Count() == expectedSymbols.Count),
                Arg.Is<IEnumerable<string>>(l => l.Contains("USD") && l.Count() == 1), true);
        }

        private List<string> SimulateReceiveSubscribeCompleteFor2Symbols(List<string> expectedSymbols)
        {
            var websocketSubscriptionsSucceededSymbols = expectedSymbols.Where(s => s != expectedSymbols[1]).ToList();

            _webSocketClient.WebSocketStreamer.SubscribeCompleteStream.Returns(
                websocketSubscriptionsSucceededSymbols.Select(s => new SubscribeComplete
                {
                    Subscription = new AggregateIndexSubscription(s, "usd").ToString()
                }).ToObservable());
            return websocketSubscriptionsSucceededSymbols;
        }

        [Fact]
        public async Task Receiving_a_non_usdc_price_update_should_store_both_usdc_converted_and_non_converted_price()
        {
            _ = PrepareIndiceDataProviderExpectations();

            var usdcPrice = 0.999m;
            var abcPrice = 1.2m;

            _cache.GetAsync("usdc".GetLatestPriceCacheKey("USD"), _cancellationToken).Returns(usdcPrice.GetBytes());

            var message = new AggregateIndex { FromSymbol = "ABC", ToSymbol = "USD", Price = 1.2m };
            AddMessageToWebSocketStream(message);

            await _priceCache.StartCaching(_cancellationToken);

            await CheckUpdateWasSetInCache(message, abcPrice / usdcPrice, "usdc");

            await CheckUpdateWasSetInCache(message, abcPrice);
        }

        [Fact]
        public async Task Receiving_a_non_usdc_price_update_should_only_store_unconverted_price_if_usdc_price_not_available()
        {
            _ = PrepareIndiceDataProviderExpectations();

            var abcPrice = 1.2m;

            var message = new AggregateIndex { FromSymbol = "ABC", ToSymbol = "EUR", Price = 1.2m };
            AddMessageToWebSocketStream(message);

            await _priceCache.StartCaching(_cancellationToken);

            await CheckUpdateWasSetInCache(message, abcPrice);
        }

        [Fact]
        public async Task Receiving_a_usdc_price_update_should_store_price_without_converting_it()
        {
            _ = PrepareIndiceDataProviderExpectations();

            var usdcPrice = 0.999m;

            var message = new AggregateIndex { FromSymbol = "USDC", ToSymbol = "GBP", Price = usdcPrice };
            AddMessageToWebSocketStream(message);

            await _priceCache.StartCaching(_cancellationToken);

            await CheckUpdateWasSetInCache(message, usdcPrice);
        }

        [Fact]
        public async Task Receiving_a_an_aggregate_without_price_should_not_try_to_update()
        {
            _ = PrepareIndiceDataProviderExpectations();

            var message = new AggregateIndex { FromSymbol = "USDC", ToSymbol = "GBP", Price = default };
            AddMessageToWebSocketStream(message);

            await _priceCache.StartCaching(_cancellationToken);

            await _cache.DidNotReceiveWithAnyArgs().SetAsync(default, default, default, default);
        }

        private IEnumerable<string> PrepareIndiceDataProviderExpectations()
        {
            var components = _mockCreator.GetIndiceComposition(3).ComponentQuantities
                .Select(c => c.ComponentDefinition).ToList();
            _indiceDataProvider.GetAllComponentsFromCurrentCompositions(_cancellationToken)
                .Returns(components);

            var expectedSymbols = components.Select(c => c.Symbol.ToLowerInvariant()).Union(new[] { "usdc" });
            return expectedSymbols;
        }

        private void AddMessageToWebSocketStream(AggregateIndex message)
        {
            var receivedMessages = new[] { message };
            _webSocketClient.WebSocketStreamer.AggregateIndiceStream.Returns(receivedMessages.ToObservable());
        }

        private async Task CheckUpdateWasSetInCache(AggregateIndex message, decimal abcPrice, string overrideToSymbol = default)
        {
            var toSymbol = overrideToSymbol ?? message.ToSymbol;
            await _cache.Received(1).SetAsync(Arg.Is(message.FromSymbol.GetLatestPriceCacheKey(toSymbol)),
                Arg.Is<byte[]>(b => b.ToDecimal() == abcPrice),
                Arg.Any<DistributedCacheEntryOptions>(),
                Arg.Is(_cancellationToken));
        }

        #region IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _priceCache?.Dispose();
            _restSocketClient?.Dispose();
        }

        #endregion
    }
}