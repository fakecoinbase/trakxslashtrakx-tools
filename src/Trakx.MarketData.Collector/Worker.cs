using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Trakx.MarketData.Collector
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IPriceCache _priceCache;

        public Worker(IPriceCache priceCache, ILogger<Worker> logger)
        {
            _priceCache = priceCache;
            _logger = logger;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _priceCache.StartCaching(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(TimeSpan.FromMinutes(60), stoppingToken).ConfigureAwait(false);
            }
        }
    }
}
