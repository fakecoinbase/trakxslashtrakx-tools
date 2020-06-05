using Flurl.Http;

namespace Trakx.Coinbase.Custody.Client.Interfaces
{
    public interface ICoinbaseClient : IAddressEndpoint, IWalletEndpoint, ITransactionEndpoint
    {
        IFlurlRequest Request(params object[] urlSegments);
    }
}