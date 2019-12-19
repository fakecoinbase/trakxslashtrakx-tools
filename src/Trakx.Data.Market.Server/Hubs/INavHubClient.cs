using System.Threading.Tasks;
using Trakx.Data.Market.Common.Pricing;

namespace Trakx.Data.Market.Server.Hubs
{
    public interface INavHubClient
    {
        Task ReceiveNavUpdate(NavUpdate update);
    }
}