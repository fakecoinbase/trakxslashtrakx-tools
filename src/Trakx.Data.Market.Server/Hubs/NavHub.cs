using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Trakx.Data.Market.Common.Pricing;
using Trakx.Data.Models.Index;

namespace Trakx.Data.Market.Server.Hubs
{
    public sealed class NavHub : Hub
    {
        private readonly INavUpdater _navUpdater;
        private readonly IMemoryCache _memoryCache;
        private readonly IHubContext<NavHub> _hubContext;
        private readonly ILogger<NavHub> _logger;

        public NavHub(INavUpdater navUpdater, 
            IMemoryCache memoryCache, 
            IHubContext<NavHub> hubContext,
            ILogger<NavHub> logger)
        {
            _navUpdater = navUpdater;
            _memoryCache = memoryCache;
            _hubContext = hubContext;
            _logger = logger;

            _navUpdater.NavUpdates.Subscribe(async update => 
                await SendNavUpdate(update).ConfigureAwait(false));
        }

        private async Task SendNavUpdate(NavUpdate navUpdate)
        {
            try
            {
                await _hubContext.Clients.All.SendCoreAsync("ReceiveNavUpdate",
            new object[] { navUpdate });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to update clients with NavUpdate {0}", 
                    JsonSerializer.Serialize(navUpdate));
            }
        }

        public async Task<bool> RegisterClientToNavUpdates(Guid clientId, Guid indexId)
        {
            try
            {
                if (!_memoryCache.TryGetValue<IndexDefinition>(indexId, out var index))
                    return false;
                var registerToNavUpdates = await _navUpdater.RegisterToNavUpdates(clientId, index)
                    .ConfigureAwait(false);
                return registerToNavUpdates;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to register client {0} for NavUpdate of {1}",
                    clientId, indexId);
                return false;
            }
        }

        public bool DeregisterClientFromNavUpdates(Guid clientId, string symbol)
        {
            try
            {
                return _navUpdater.DeregisterFromNavUpdates(clientId, symbol);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to deregister client {0} for NavUpdates of {1}",
                    clientId, symbol);
                return false;
            }
        }
    }
}
