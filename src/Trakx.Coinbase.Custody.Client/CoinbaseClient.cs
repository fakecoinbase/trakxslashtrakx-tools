using Flurl.Http;
using Trakx.Coinbase.Custody.Client.Interfaces;

namespace Trakx.Coinbase.Custody.Client
{
    public class CoinbaseClient : FlurlClient, ICoinbaseClient
    {
        public CoinbaseClient(IApiKeyConfig api) : base("https://api.custody.coinbase.com/api/v1/")
        {
            api.Configure(this);
        }
    }
}
