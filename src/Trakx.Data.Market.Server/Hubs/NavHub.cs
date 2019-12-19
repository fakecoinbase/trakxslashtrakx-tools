using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Trakx.Data.Market.Common.Extensions;
using Trakx.Data.Market.Common.Pricing;
using Trakx.Data.Market.Server.Pages;
using Trakx.Data.Models.Index;

namespace Trakx.Data.Market.Server.Hubs
{
    public sealed class NavHubClient
    {
        private readonly IIndexDefinitionProvider _indexDefinitionProvider;

        public NavHubClient(IIndexDefinitionProvider indexDefinitionProvider)
        {
            _indexDefinitionProvider = indexDefinitionProvider;
        }
    }

    public sealed class NavHub : Hub
    {
        private readonly INavUpdater _navUpdater;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<NavHub> _logger;
        private IDisposable _updatesSubscription;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public NavHub(INavUpdater navUpdater, 
            IMemoryCache memoryCache, 
            ILogger<NavHub> logger)
        {
            _navUpdater = navUpdater;
            _memoryCache = memoryCache;
            _logger = logger;

            _cancellationTokenSource = new CancellationTokenSource();

        }

        public async Task<bool> RegisterClientToNavUpdates(Guid clientId, Guid indexId)
        {
            if (!_memoryCache.TryGetValue<IndexDefinition>(indexId, out var index))
                return false;
            var registerToNavUpdates = await _navUpdater.RegisterToNavUpdates(clientId, index)
                .ConfigureAwait(false);
            
            _logger.LogDebug("Subscribed");
            _navUpdater.NavUpdates.ToEvent().OnNext += update =>
            {
                _logger.LogDebug("UPDAAAATE");
                Clients.All.SendCoreAsync(nameof(INavHubClient.ReceiveNavUpdate), new object[] {update});
            };
            
            return registerToNavUpdates;
        }

        public bool DeregisterClientFromNavUpdates(Guid clientId, string symbol)
        {
            return _navUpdater.DeregisterFromNavUpdates(clientId, symbol);
        }
        
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _cancellationTokenSource?.Cancel();
            _updatesSubscription?.Dispose();
            _cancellationTokenSource?.Dispose();
        }
    }
}
