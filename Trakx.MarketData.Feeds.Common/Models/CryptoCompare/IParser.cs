using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

namespace Trakx.MarketData.Feeds.Common.Models.CryptoCompare
{
    public interface IParser
    {
        IReadOnlyDictionary<string, ICoin> ReadCoinsBySymbol(string responseContent);
        IList<ICoin> GetTop10Coins(string responseContent);
    }
}
