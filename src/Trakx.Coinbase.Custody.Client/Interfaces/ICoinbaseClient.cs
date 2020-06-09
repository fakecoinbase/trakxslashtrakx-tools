using Flurl.Http;

namespace Trakx.Coinbase.Custody.Client.Interfaces
{
    public interface ICoinbaseClient : IAddressEndpoint, IWalletEndpoint, ITransactionEndpoint,ICurrencyEndpoint
    {
        IFlurlRequest Request(params object[] urlSegments);
    }
}