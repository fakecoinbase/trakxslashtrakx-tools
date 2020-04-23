using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Trakx.Common.Interfaces;
using Trakx.Common.Interfaces.Pricing;
using Trakx.Common.Pricing;

namespace Trakx.MarketData.Server.Hubs
{
    public sealed class NavHub : Hub
    {
        private readonly INavUpdater _navUpdater;
        private readonly IIndiceDataProvider _indiceProvider;
        private readonly IHubContext<NavHub> _hubContext;
        private readonly ILogger<NavHub> _logger;

        public NavHub(INavUpdater navUpdater, 
            IIndiceDataProvider indiceProvider,
            IMemoryCache memoryCache, 
            IHubContext<NavHub> hubContext,
            ILogger<NavHub> logger)
        {
            _navUpdater = navUpdater;
            _indiceProvider = indiceProvider;
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

        public async Task<bool> RegisterClientToNavUpdates(Guid clientId, string indiceSymbol)
        {
            try
            {
                var composition = await _indiceProvider.GetCurrentComposition(indiceSymbol);
                if (composition == default) return false;
                var registerToNavUpdates = _navUpdater.RegisterToNavUpdates(clientId, composition);
                return registerToNavUpdates;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to register client {0} for NavUpdate of [{1}]",
                    clientId, indiceSymbol);
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
