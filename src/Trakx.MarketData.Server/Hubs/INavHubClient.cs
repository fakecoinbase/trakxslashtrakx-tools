using System.Threading.Tasks;
using Trakx.Common.Pricing;

namespace Trakx.MarketData.Server.Hubs
{
    public interface INavHubClient
    {
        Task ReceiveNavUpdate(NavUpdate update);
    }
}