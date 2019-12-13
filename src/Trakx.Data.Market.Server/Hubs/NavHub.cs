using System.Threading.Channels;
 using Microsoft.AspNetCore.SignalR;
using Trakx.Data.Market.Common.Extensions;
using Trakx.Data.Market.Common.Pricing;

namespace Trakx.Data.Market.Server.Hubs
{
    public sealed class NavHub : Hub
    {
        private readonly INavUpdater _navUpdater;

        public NavHub(INavUpdater navUpdater)
        {
            _navUpdater = navUpdater;
        }

        public void RegisterToNavUpdates(string symbol)
        {
            _navUpdater.RegisterToNavUpdates(symbol);
        }

        public void DeregisterFromNavUpdates(string symbol)
        {
            _navUpdater.DeregisterFromNavUpdates(symbol);
        }

        public ChannelReader<NavUpdate> NavUpdatesStream()
        {
            return _navUpdater.NavUpdates.AsChannelReader(10);
        }
    }
}
