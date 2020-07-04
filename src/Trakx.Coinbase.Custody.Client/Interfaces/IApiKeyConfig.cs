using Flurl.Http;

namespace Trakx.Coinbase.Custody.Client.Interfaces
{
    public interface IApiKeyConfig
    {
        void Configure(IFlurlClient client);
    }
}