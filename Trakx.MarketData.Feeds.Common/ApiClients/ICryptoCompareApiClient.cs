using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Trakx.MarketData.Feeds.Common.Models.CryptoCompare;

namespace Trakx.MarketData.Feeds.Common.ApiClients
{
    public interface ICryptoCompareApiClient
    {
        Task<ICryptoCompareResponse> GetAllSupportedCoins();
    }
}
