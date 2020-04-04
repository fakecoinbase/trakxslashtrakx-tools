using System.Collections.Generic;

namespace Trakx.Common.Sources.Coinbase
{
    public interface ICoinbaseClient
    {
        List<string> CustodiedCoins { get; }
    }

    public class CoinbaseClient : ICoinbaseClient
    {
        public List<string> CustodiedCoins { get; } = new List<string>()
        {
            "ALGO", "BAND", "BAT", "BCAP", "BCH", "BNT", "BTC", "CVC", "CSP", 
            "EOS", "ETC", "ETH", "FOAM", "GNT", "KIN", "KNC", "LINK", "LOOM", 
            "LTC", "MANA", "MKR", "NMR", "OGN", "OMG", "ORBS", "OXT", "PROPS", 
            "STORJ", "USDC", "XLM", "XRP", "XTZ", "XYO", "ZEC", "ZRX"
        };
    }
}
