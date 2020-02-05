using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoinGecko.Entities.Response.Coins;
using Trakx.Data.Common.Pricing;

namespace Trakx.Data.Common.Sources.CoinGecko
{
    public interface ICoinGeckoClient
    {
        Task<decimal?> GetLatestPrice(string symbol, string quoteCurrency = Constants.DefaultQuoteCurrency);
        Task<decimal?> GetPriceAsOf(string symbol, DateTime asOf, string quoteCurrency = Constants.DefaultQuoteCurrency);
        bool TryRetrieveSymbol(string coinName, out string? symbol);
        Task<IReadOnlyList<CoinList>> GetCoinList();
    }
}