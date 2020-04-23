using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Trakx.Common.Interfaces;
using Trakx.Common.Interfaces.Indice;
using Trakx.Common.Utils;
using Trakx.MarketData.Collector.CryptoCompare;
using Trakx.MarketData.Collector.CryptoCompare.DTOs.Inbound;
using Trakx.MarketData.Collector.CryptoCompare.DTOs.Outbound;

namespace Trakx.MarketData.Collector
{
    public interface IPriceCache
    {
        Task StartCaching(CancellationToken cancellationToken);
    }

    public class PriceCache : IPriceCache
    {
        private readonly TimeSpan _waitForDbDelays;
        private IIndiceDataProvider _indiceDataProvider;
        private readonly ICryptoCompareWebSocketClient _webSocketClient;
        private readonly IDistributedCache _cache;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<PriceCache> _logger;

        internal PriceCache(ICryptoCompareWebSocketClient webSocketClient,
            IDistributedCache cache,
            IServiceScopeFactory serviceScopeFactory,
            ILogger<PriceCache> logger, 
            TimeSpan waitForDbDelays)
        {
            _waitForDbDelays = waitForDbDelays;
            _webSocketClient = webSocketClient;
            _cache = cache;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        public PriceCache(ICryptoCompareWebSocketClient webSocketClient, 
            IDistributedCache cache, 
            IServiceScopeFactory serviceScopeFactory, 
            ILogger<PriceCache> logger) :this(webSocketClient, cache, serviceScopeFactory, logger, 
            TimeSpan.FromMilliseconds(500)) { }

        public async Task StartCaching(CancellationToken cancellationToken)
        {
            using var initialisationScope = _serviceScopeFactory.CreateScope();
            _indiceDataProvider = initialisationScope.ServiceProvider.GetService<IIndiceDataProvider>();

            await WaitForDatabaseToRespond(cancellationToken).ConfigureAwait(false);
            await SubscribeToWebSocketStreams(cancellationToken).ConfigureAwait(false);
            await SubscribeToAllComponentsFeeds(cancellationToken).ConfigureAwait(false);
        }

        private async Task WaitForDatabaseToRespond(CancellationToken cancellationToken)
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
                _logger.LogInformation("Trying to retrieve known indicees from Database...");
                var allIndiceSymbols = await _indiceDataProvider.GetAllIndiceSymbols(cancellationToken);
                if (allIndiceSymbols?.Count == 0) throw new InvalidDataException("No Indices defined in database.");
            });
        }

        private async Task SubscribeToWebSocketStreams(CancellationToken cancellationToken)
        {
            if (_webSocketClient.State != WebSocketState.Open) await _webSocketClient.Connect();
            _webSocketClient.WebSocketStreamer.AggregateIndiceStream
                .Do(TryLogDebugMessage)
                .Subscribe(async a => await SetNewPriceInCache(a, cancellationToken).ConfigureAwait(false));

            _webSocketClient.WebSocketStreamer.HeartBeatStream.Subscribe(TryLogDebugMessage);
            _webSocketClient.WebSocketStreamer.SubscribeCompleteStream.Subscribe(TryLogDebugMessage);
            _webSocketClient.WebSocketStreamer.LoadCompleteStream.Subscribe(TryLogDebugMessage);
            _webSocketClient.WebSocketStreamer.UnsubscribeCompleteStream.Subscribe(TryLogDebugMessage);
            _webSocketClient.WebSocketStreamer.UnsubscribeAllCompleteStream.Subscribe(TryLogDebugMessage);
            _webSocketClient.WebSocketStreamer.ErrorStream.Subscribe(TryLogWarningMessage);
        }

        private async Task SetNewPriceInCache(AggregateIndice aggregate, CancellationToken cancellationToken) 
        {
            try
            {
                if (!aggregate.FromSymbol.Equals("usdc", StringComparison.InvariantCultureIgnoreCase))
                {
                    var usdcBytes = await _cache.GetAsync("usdc".GetLatestPriceCacheKey(aggregate.ToSymbol), cancellationToken);
                    var usdc = usdcBytes?.Length == 16 ? usdcBytes.ToDecimal() : 0m;
                    
                    if (usdc != 0m)
                        await _cache.SetAsync(aggregate.FromSymbol.GetLatestPriceCacheKey("usdc"),
                            (aggregate.Price / usdc).GetBytes(), 
                            cancellationToken);
                }
                
                await _cache.SetAsync(aggregate.FromSymbol.GetLatestPriceCacheKey(aggregate.ToSymbol),
                    aggregate.Price.GetBytes(), cancellationToken);

            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to store price update for {0} in cache.", JsonSerializer.Serialize(aggregate));
            } 

        }

        private async Task SubscribeToAllComponentsFeeds(CancellationToken cancellationToken)
        {
            var currentComponents = await _indiceDataProvider.GetAllComponentsFromCurrentCompositions(cancellationToken);
            var currentComponentsSymbols = new[] { "usdc" }.Union(currentComponents.Select(c => c.Symbol)).Distinct();
            var subscriptions = currentComponentsSymbols
                .Select(s => new AggregateIndiceSubscription(s, "usd"))
                .ToArray();

            await _webSocketClient.AddSubscriptions(subscriptions);
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
    }
}