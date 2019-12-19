using System;
using System.Threading.Tasks;
using Trakx.Data.Market.Common.Sources.Kaiko.DTOs;

namespace Trakx.Data.Market.Common.Sources.Kaiko.Client
{
    public interface IKaikoClient
    {
        Task<SpotDirectExchangeRateResponse> GetSpotExchangeRate(SpotExchangeRateRequest request);
        [Obsolete("broken")]
        Task<AssetsResponse> GetAssets();
        Task<InstrumentsResponse> GetInstruments();
        Task<ExchangesResponse> GetExchanges();
        SpotExchangeRateRequest CreateSpotExchangeRateRequest(string coinSymbol, string quoteSymbol,
            bool direct = false, DateTime? dateTime = null);
    }
}