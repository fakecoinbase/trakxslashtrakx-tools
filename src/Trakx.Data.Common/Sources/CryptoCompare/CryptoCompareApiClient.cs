using System;
using System.Collections.Generic;
using System.Linq;

namespace Trakx.Data.Common.Sources.CryptoCompare
{
    public class CryptoCompareApiClient
    {
        //waiting for https://github.com/joancaron/cryptocompare-api/issues/61
        //private ICryptoCompareClient _client;

        public CryptoCompareApiClient()
        {
            
        }

        public List<string> GetAllErc20Symbols()
        {
            var smartContractCoins = CoinDetailsProvider.CoinDetailsBySymbol.Values.Where(c =>
                c.SmartContractAddress.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase));

            return smartContractCoins.Select(c => c.Symbol).ToList();
        }
    }
}
