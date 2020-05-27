using System.Dynamic;
using Flurl.Http;
using Trakx.Coinbase.Custody.Client.Interfaces;

namespace Trakx.Coinbase.Custody.Client
{
    public class CoinbaseClient : FlurlClient
    {
        public string ApiUrl { get; }

        public CoinbaseClient(IApiKeyConfig api)
        {
            api.Configure(this);
            ApiUrl = "https://api.custody.coinbase.com/api/v1/";
        }

    }
}
