using System.Collections.Generic;
using System.Threading.Tasks;
using CoinGecko.Entities.Response.Coins;

namespace Trakx.Data.Market.Common.Sources.CoinGecko
{
    public interface ICoinGeckoClient
    {
        Task<decimal> GetLatestUsdPrice(string symbol);
        bool TryRetrieveSymbol(string coinName, out string? symbol);
        Task<IReadOnlyList<CoinList>> GetCoinList();
    }
}