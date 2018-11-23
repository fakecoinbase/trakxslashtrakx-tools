using System.Threading.Tasks;
using Trakx.MarketData.Feeds.Common.Models.CoinMarketCap;

namespace Trakx.MarketData.Feeds.Common.ApiClients
{
    public interface ICoinMarketCapApiClient
    {
        Task<ICoinsAndMarketCapListing> GetCoinsAndMarketCapListings();
    }
}