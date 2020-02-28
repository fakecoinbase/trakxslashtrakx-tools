using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoinGecko.Entities.Response.Coins;
using Trakx.Data.Common.Pricing;

namespace Trakx.Data.Common.Sources.CoinGecko
{
    public interface ICoinGeckoClient
    {
        Task<decimal?> GetLatestPrice(string coinGeckoId, string quoteCurrency = Constants.DefaultQuoteCurrency);
        Task<decimal?> GetPriceAsOf(string symbol, DateTime asOf, string quoteCurrency = Constants.DefaultQuoteCurrency);
        bool TryRetrieveSymbol(string coinName, out string? symbol);
        Task<decimal?> GetPriceAsOfFromId(string id, DateTime asOf, string quoteCurrencyId = "usd-coin");
        Task<MarketData> GetMarketDataAsOfFromId(string id, DateTime asOf, string quoteCurrencyId = "usd-coin");

        void RetrieveContractDetailsFromCoinSymbolName(string searchSymbol, string searchName, out string? coinGeckoId,
            out string? symbol, out string? contractAddress);
        Task<IReadOnlyList<CoinList>> GetCoinList();
    }
}