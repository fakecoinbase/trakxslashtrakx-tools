using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Trakx.Data.Common.Interfaces;
using Trakx.Data.Common.Interfaces.Pricing;
using Trakx.Data.Common.Pricing;

namespace Trakx.Data.Market.Server.Hubs
{
    public sealed class NavHub : Hub
    {
        private readonly INavUpdater _navUpdater;
        private readonly IIndexDataProvider _indexProvider;
        private readonly IHubContext<NavHub> _hubContext;
        private readonly ILogger<NavHub> _logger;

        public NavHub(INavUpdater navUpdater, 
            IIndexDataProvider indexProvider,
            IMemoryCache memoryCache, 
            IHubContext<NavHub> hubContext,
            ILogger<NavHub> logger)
        {
            _navUpdater = navUpdater;
            _indexProvider = indexProvider;
            _hubContext = hubContext;
            _logger = logger;

            _navUpdater.NavUpdates.Subscribe(async update => 
                await SendNavUpdate(update).ConfigureAwait(false));
        }

        private async Task SendNavUpdate(NavUpdate navUpdate)
        {
            try
            {
                await _hubContext.Clients.All.SendCoreAsync(nameof(INavHubClient.ReceiveNavUpdate),
            new object[] { navUpdate });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to update clients with NavUpdate {0}", 
                    JsonSerializer.Serialize(navUpdate));
            }
        }

        public async Task<bool> RegisterClientToNavUpdates(Guid clientId, string indexSymbol)
        {
            try
            {
                var composition = await _indexProvider.GetCurrentComposition(indexSymbol);
                if (composition == default) return false;
                var registerToNavUpdates = _navUpdater.RegisterToNavUpdates(clientId, composition);
                return registerToNavUpdates;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to register client {0} for NavUpdate of [{1}]",
                    clientId, indexSymbol);
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
