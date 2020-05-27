using Trakx.Coinbase.Custody.Client;

namespace Trakx.Coinbase.Custody.Client.Interfaces
{
    public interface IApiKeyConfig
    {
        void Configure(CoinbaseClient client);
    }
}