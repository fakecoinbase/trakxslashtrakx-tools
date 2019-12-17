using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Trakx.Data.Market.Common.Extensions;
using Trakx.Data.Market.Common.Pricing;
using Trakx.Data.Models.Index;

namespace Trakx.Data.Market.Server.Hubs
{
    public sealed class NavHub : Hub
    {
        private readonly INavUpdater _navUpdater;

        public NavHub(INavUpdater navUpdater)
        {
            _navUpdater = navUpdater;
        }

        public async Task RegisterClientToNavUpdates(Guid clientId, IndexDefinition index)
        {
            await _navUpdater.RegisterToNavUpdates(clientId, index)
                .ConfigureAwait(false);
        }

        public bool DeregisterClientFromNavUpdates(Guid clientId, string symbol)
        {
            return _navUpdater.DeregisterFromNavUpdates(clientId, symbol);
        }

        public ChannelReader<NavUpdate> NavUpdatesStream()
        {
            return _navUpdater.NavUpdates.AsChannelReader(10);
        }
    }
}
