using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CryptoCompare;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Trakx.Common.Extensions;
using Trakx.Common.Interfaces;
using Trakx.Common.Interfaces.Indice;
using Trakx.Common.Utils;
using Trakx.MarketData.Collector.CryptoCompare;
using Trakx.MarketData.Collector.CryptoCompare.DTOs.Inbound;
using Trakx.MarketData.Collector.CryptoCompare.DTOs.Outbound;

namespace Trakx.MarketData.Collector
{
    /// <inheritdoc cref="IPriceCache"/>
    /// <inheritdoc cref="IDisposable"/>
    public class PriceCache : IPriceCache, IDisposable
    {
        private readonly TimeSpan _waitForDbDelays;
        private IIndiceDataProvider _indiceDataProvider;
        private readonly ICryptoCompareWebSocketClient _webSocketClient;
        private readonly ICryptoCompareClient _restClient;
        private readonly IDistributedCache _cache;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<PriceCache> _logger;
        private readonly IScheduler _scheduler;
        private readonly CancellationTokenSource _pollingCancellationTokenSource;
        private readonly IObservable<CurrencyPair> _restCurrencyPairStream;
        private readonly List<string> _allConstituentsSymbols = new List<string>();
        private readonly List<string> _webSocketSourcedSymbols = new List<string>();
        private readonly List<string> _restSourcedSymbols = new List<string>();
        public static readonly TimeSpan RestPollingInterval = TimeSpan.FromSeconds(5);
        private readonly List<IDisposable> _subscriptions = new List<IDisposable>();

        internal PriceCache(ICryptoCompareWebSocketClient webSocketClient,
            ICryptoCompareClient restClient,
            IDistributedCache cache,
            IServiceScopeFactory serviceScopeFactory,
            ILogger<PriceCache> logger,
            TimeSpan waitForDbDelays,
            IScheduler scheduler = default)
        {
            _waitForDbDelays = waitForDbDelays;
            _webSocketClient = webSocketClient;
            _restClient = restClient;
            _cache = cache;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _scheduler = scheduler ?? Scheduler.Default;
            _pollingCancellationTokenSource = new CancellationTokenSource();
            _restCurrencyPairStream = BuildStartRestPriceStream(_pollingCancellationTokenSource.Token);
        }

        public PriceCache(ICryptoCompareWebSocketClient webSocketClient,
            ICryptoCompareClient restClient,
            IDistributedCache cache, 
            IServiceScopeFactory serviceScopeFactory, 
            ILogger<PriceCache> logger) :this(webSocketClient, restClient, cache, serviceScopeFactory, logger, 
            TimeSpan.FromMilliseconds(500)) { }
        
        /// <inheritdoc />
        public async Task StartCaching(CancellationToken cancellationToken)
        {
            using var initialisationScope = _serviceScopeFactory.CreateScope();
            _indiceDataProvider = initialisationScope.ServiceProvider.GetService<IIndiceDataProvider>();

            await SubscribeToWebSocketStreams(cancellationToken).ConfigureAwait(false);
            await TryForeverGettingConstituentsFromDatabase(cancellationToken).ConfigureAwait(false);
            await SubscribeToAllComponentsFeeds(cancellationToken).ConfigureAwait(false);
            _subscriptions.Add(_restCurrencyPairStream.Subscribe(async p =>
                await SetNewPriceInCache(p, _pollingCancellationTokenSource.Token).ConfigureAwait(false)));
        }

        internal void StopPolling()
        {
            _pollingCancellationTokenSource.Cancel();
        }

        /// <inheritdoc />
        public IReadOnlyList<string> AllConstituentsSymbols => _allConstituentsSymbols.AsReadOnly();

        /// <inheritdoc />
        public IReadOnlyList<string> WebSocketSourcedSymbols => _webSocketSourcedSymbols.AsReadOnly();

        /// <inheritdoc />
        public IReadOnlyList<string> RestSourcedSymbols => _restSourcedSymbols.AsReadOnly();

        private async Task TryForeverGettingConstituentsFromDatabase(CancellationToken cancellationToken)
        {
            var retryPolicy = Policy.Handle<Exception>()
                .WaitAndRetryForeverAsync(
                    i => _waitForDbDelays.Multiply(Math.Min(10, i)),
                    (exception, _, timespan) => 
                {
                    _logger.LogInformation("Waiting for {0} to provide data. Received error message {1}",
                        nameof(IIndiceDataProvider), exception.Message);
                    _logger.LogDebug(exception, "Waiting {0} seconds for database.", timespan.ToString(@"ss\.ff"));
                });

            await retryPolicy.ExecuteAndCaptureAsync(async () =>
            {
                _logger.LogInformation("Trying to retrieve known indices from Database...");
                var currentConstituents = await _indiceDataProvider.GetAllComponentsFromCurrentCompositions(cancellationToken);
                
                if (currentConstituents == null || currentConstituents.Count == 0) 
                    throw new InvalidDataException("No indices defined in database yet.");
                
                var currentConstituentsSymbols = new[] { "usdc" }.Union(
                        currentConstituents.Select(c => c.Symbol.ToNativeSymbol().ToLowerInvariant())).Distinct();
                _allConstituentsSymbols.AddRange(currentConstituentsSymbols.ToList());
            });
        }

        private async Task SubscribeToWebSocketStreams(CancellationToken cancellationToken)
        {
            if (_webSocketClient.State != WebSocketState.Open) await _webSocketClient.Connect();
            _subscriptions.Add(_webSocketClient.WebSocketStreamer.AggregateIndiceStream
                .Subscribe(async a => await SetNewPriceInCache(a, cancellationToken).ConfigureAwait(false)));

            _subscriptions.Add(_webSocketClient.WebSocketStreamer.HeartBeatStream.Subscribe(TryLogDebugMessage));
            _subscriptions.Add(_webSocketClient.WebSocketStreamer.SubscribeCompleteStream.Do(TryLogDebugMessage).Subscribe(AddSymbolToWebSocketSourced));
            _subscriptions.Add(_webSocketClient.WebSocketStreamer.LoadCompleteStream.Do(TryLogDebugMessage).Subscribe(_ => BuildRestSourcedList()));
            _subscriptions.Add(_webSocketClient.WebSocketStreamer.UnsubscribeCompleteStream.Subscribe(TryLogDebugMessage));
            _subscriptions.Add(_webSocketClient.WebSocketStreamer.UnsubscribeAllCompleteStream.Subscribe(TryLogDebugMessage));
            _subscriptions.Add(_webSocketClient.WebSocketStreamer.ErrorStream.Subscribe(TryLogWarningMessage));
        }

        private IObservable<CurrencyPair> BuildStartRestPriceStream(CancellationToken cancellationToken)
        {
            var currencyPairStream = Observable.Interval(RestPollingInterval, _scheduler)
                .TakeUntil(_ => cancellationToken.IsCancellationRequested)
                .SelectMany(async _ =>
                {
                    var symbols = RestSourcedSymbols.ToArray();
                    if (!symbols.Any()) return default;
                    try
                    {
                        var prices = await _restClient.Prices
                            .MultipleSymbolsPriceAsync(symbols, new[] {"USD"}, true)
                            .ConfigureAwait(false);

                        var currencyPairs = prices.Keys.Select(baseCurrency =>
                        {
                            var currencyPair = new CurrencyPair(baseCurrency, "USD", prices[baseCurrency]["USD"]);
                            return currencyPair;
                        });
                        return currencyPairs.ToObservable();
                    }
                    catch (Exception e)
                    {
                        _logger.LogWarning(e, "Failed to retrieve prices from CryptoCompare Rest API.");
                        return default;
                    }
                })
                .Where(p => p != default)
                .SelectMany(p => p);
            return currencyPairStream;
        }

        private struct CurrencyPair
        {
            public CurrencyPair(string fromSymbol, string toSymbol, decimal price)
            {
                FromSymbol = fromSymbol;
                ToSymbol = toSymbol;
                Price = price;
            }

            public CurrencyPair(AggregateIndex aggregateIndex)
            {
                FromSymbol = aggregateIndex.FromSymbol.ToUpperInvariant();
                ToSymbol = aggregateIndex.ToSymbol.ToUpperInvariant();
                Price = aggregateIndex.Price ?? 0;
            }

            public string FromSymbol { get; }
            public string ToSymbol { get; }
            public decimal Price { get; }
        }

        private void AddSymbolToWebSocketSourced(SubscribeComplete subscriptionComplete)
        {
            var subscription = CryptoCompareSubscriptionConverter.ParseSubscriptionString(subscriptionComplete.Subscription);
            if (!(subscription is AggregateIndexSubscription aggregateIndexSubscription)) return;
            _webSocketSourcedSymbols.Add(aggregateIndexSubscription.BaseCurrency.ToLowerInvariant());
        }

        /// <summary>
        /// Build the list of symbols for which we didn't successfully get a websocket subscription to get the
        /// prices from the Rest API instead.
        /// </summary>
        private void BuildRestSourcedList()
        {
            _restSourcedSymbols.AddRange(AllConstituentsSymbols.Except(WebSocketSourcedSymbols));
            _logger.LogInformation("Getting prices for [{0}] from Rest.", string.Join(", ", RestSourcedSymbols));
        }

        private async Task SetNewPriceInCache(AggregateIndex aggregate, CancellationToken cancellationToken)
        {
            if(aggregate.Price == default) return;
            var currencyPair = new CurrencyPair(aggregate);
            await SetNewPriceInCache(currencyPair, cancellationToken).ConfigureAwait(false);
        }

        private async Task SetNewPriceInCache(CurrencyPair currencyPair, CancellationToken cancellationToken)
        {
            try
            {
                await _cache.SetAsync(currencyPair.FromSymbol.GetLatestPriceCacheKey(currencyPair.ToSymbol),
                    currencyPair.Price.GetBytes(), cancellationToken);
                _logger.LogInformation("Updated price for {0}/{1} to {2} in cache.", currencyPair.FromSymbol,
                    currencyPair.ToSymbol, currencyPair.Price);

                if (currencyPair.FromSymbol.Equals("USDC")) return;

                var usdcBytes =
                    await _cache.GetAsync("usdc".GetLatestPriceCacheKey(currencyPair.ToSymbol), cancellationToken);
                var usdc = usdcBytes?.Length == 16 ? usdcBytes.ToDecimal() : 0m;

                if (usdc == 0m) return;
                
                var usdcPrice = (currencyPair.Price / usdc);
                await _cache.SetAsync(currencyPair.FromSymbol.GetLatestPriceCacheKey("usdc"),
                    usdcPrice.GetBytes(),
                    cancellationToken);
                _logger.LogInformation("Updated price for {0}/{1} to {2} in cache.", currencyPair.FromSymbol,
                    "USDC", usdcPrice);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to store price update for {0} in cache.",
                    JsonSerializer.Serialize(currencyPair));
            }
        }

        private async Task SubscribeToAllComponentsFeeds(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Adding subscriptions to AggregateIndex updates for tokens {0}.", 
                string.Join(", ", AllConstituentsSymbols));
            
            var subscriptions = AllConstituentsSymbols
                .Select(s => new AggregateIndexSubscription(s, "usd"))
                .ToArray();
            await _webSocketClient.AddSubscriptions(subscriptions);
            _logger.LogInformation("Subscriptions added.");
        }

        #region Logging

        private void TryLogDebugMessage<T>(T message) where T : InboundMessageBase
        {
            TryLogMessage(message, LogLevel.Debug);
        }

        private void TryLogWarningMessage<T>(T message) where T : InboundMessageBase
        {
            TryLogMessage(message, LogLevel.Warning);
        }

        private void TryLogMessage<T>(T message, LogLevel logLevel) where T : InboundMessageBase
        {
            if (!_logger.IsEnabled(logLevel)) return;
            try
            {
                var json = JsonSerializer.Serialize(message);
                _logger.LogDebug("Received message from WebSocket: {1}", json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log incoming websocket message.");
            }
        }

        #endregion

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if(!_pollingCancellationTokenSource.IsCancellationRequested) StopPolling();
            _subscriptions.ForEach(s => s.Dispose());
            _pollingCancellationTokenSource.Dispose();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}