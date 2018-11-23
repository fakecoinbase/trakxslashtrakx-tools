using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Microsoft.AspNetCore.Rewrite.Internal.UrlActions;

using Newtonsoft.Json;
using Trakx.MarketData.Feeds.Common.Models.CryptoCompare;

namespace Trakx.MarketData.Feeds.Models.CryptoCompare
{
    public class Parser : IParser
    {
        /// <inheritdoc />
        public IReadOnlyDictionary<string, ICoin> ReadCoinsBySymbol(string responseContent)
        {
            dynamic responseAsJson = JsonConvert.DeserializeObject(responseContent);
            var coinsBySymbol =
                (ReadOnlyDictionary<string, Coin>)JsonConvert.DeserializeObject<ReadOnlyDictionary<string, Coin>>(
                    responseAsJson.Data.ToString());
            var icoinsBySymbol = coinsBySymbol.ToDictionary(e => e.Key, e => (ICoin)e.Value);
            var readOnlyDictionary = new ReadOnlyDictionary<string, ICoin>(icoinsBySymbol);
            return readOnlyDictionary;
        }

        /// <inheritdoc />
        public IList<ICoin> GetTop10Coins(string responseContent)
        {
            dynamic responseAsJson = JsonConvert.DeserializeObject(responseContent);
            var coinsBySymbol =
                (ReadOnlyDictionary<string, Coin>)JsonConvert.DeserializeObject<ReadOnlyDictionary<string, Coin>>(
                    responseAsJson.Data.ToString());
            var top10CoinIds =
                (List<uint>)responseAsJson.DefaultWatchlist.CoinIs.ToString().Split(',').Cast<uint>().ToList();
            var top10Coins = top10CoinIds.Select(i => (ICoin)coinsBySymbol.Single(c => c.Value.Id == i).Value).ToList();
            return top10Coins;
        }
    }
}