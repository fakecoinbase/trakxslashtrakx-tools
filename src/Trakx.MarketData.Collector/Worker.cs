using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Trakx.Common.Interfaces;
using Trakx.Common.Interfaces.Index;
using Trakx.Common.Utils;
using Trakx.MarketData.Collector.CryptoCompare;
using Trakx.MarketData.Collector.CryptoCompare.DTOs.Inbound;
using Trakx.MarketData.Collector.CryptoCompare.DTOs.Outbound;

namespace Trakx.MarketData.Collector
{
    public class Worker : BackgroundService
    {
        private readonly IIndexDataProvider _indexDataProvider;
        private readonly ICryptoCompareWebSocketClient _webSocketClient;
        private readonly IDistributedCache _cache;
        private readonly ILogger<Worker> _logger;
        private readonly TimeSpan _waitDelays;

        internal Worker(IIndexDataProvider indexDataProvider,
            ICryptoCompareWebSocketClient webSocketClient,
            IDistributedCache cache,
            ILogger<Worker> logger,
            TimeSpan waitDelays)
        {
            _indexDataProvider = indexDataProvider;
            _webSocketClient = webSocketClient;
            _cache = cache;
            _logger = logger;
            _waitDelays = waitDelays;
        }

        public Worker(IIndexDataProvider indexDataProvider,
            ICryptoCompareWebSocketClient webSocketClient,
            IDistributedCache cache,
            ILogger<Worker> logger) 
            : this(indexDataProvider, 
                webSocketClient, 
                cache, 
                logger, 
                TimeSpan.FromSeconds(1)) {}

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(_waitDelays, stoppingToken).ConfigureAwait(false);
            }
        }

        #region Overrides of BackgroundService

        /// <inheritdoc />
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            while (!(await GetAllIndexSymbols(cancellationToken).ConfigureAwait(false)).Any())
            {
                _logger.LogInformation("Waiting for {0} to provide data.", nameof(IIndexDataProvider));
                await Task.Delay(_waitDelays, cancellationToken).ConfigureAwait(false);
            }

            SubscribeCacheToWebSocketStreams(cancellationToken);
            await SubscribeToAllComponentsFeeds(cancellationToken).ConfigureAwait(false);

            base.StartAsync(cancellationToken);
        }

        #endregion

        private async Task<List<string>> GetAllIndexSymbols(CancellationToken cancellationToken)
        {
            var retryPolicy = Policy.Handle<Exception>().RetryForeverAsync();
            var indexSymbols = await retryPolicy.ExecuteAndCaptureAsync(async () =>
            {
                _logger.LogInformation("Trying to retrieve known indexes from Database...");
                return await _indexDataProvider.GetAllIndexSymbols(cancellationToken);
            });
            return indexSymbols.Outcome == OutcomeType.Successful ? indexSymbols.Result : new List<string>();
        }

        private void SubscribeCacheToWebSocketStreams(CancellationToken cancellationToken)
        {
            _webSocketClient.WebSocketStreamer.AggregateIndexStream.Subscribe(async a => await SetNewPriceInCache(a, cancellationToken).ConfigureAwait(false));
            _webSocketClient.WebSocketStreamer.HeartBeatStream.Subscribe(LogHeartBeat);
        }

        private void LogHeartBeat(HeartBeat h)
        {
            _logger.LogInformation("HeartBeat timed {0} received from WebSocket {1}", h.TimeMs, h.Message);

        }

        private async Task SetNewPriceInCache(AggregateIndex aggregate, CancellationToken cancellationToken) 
        {
            try
            {
                if (!aggregate.FromSymbol.Equals("usdc", StringComparison.InvariantCultureIgnoreCase))
                {
                    var usdcBytes = await _cache.GetAsync("usdc".GetLatestPriceCacheKey(aggregate.ToSymbol), cancellationToken);
                    var usdc = usdcBytes.Length == 16 ? usdcBytes.ToDecimal() : 0m;
                    
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
            var currentComponents = await _indexDataProvider.GetAllComponentsFromCurrentCompositions(cancellationToken).ToListAsync(cancellationToken);
            var currentComponentsSymbols = currentComponents.Select(c => c.Symbol).Union(new []{"usdc"}).Distinct();
            var subscriptions = currentComponentsSymbols
                .Select(s => new AggregateIndexSubscription(s, "usd"))
                .ToArray();

            await _webSocketClient.AddSubscriptions(subscriptions);
        }
    }
}
