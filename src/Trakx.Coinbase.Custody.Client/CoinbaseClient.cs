using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;
using Trakx.Coinbase.Custody.Client.Endpoints;
using Trakx.Coinbase.Custody.Client.Interfaces;
using Trakx.Coinbase.Custody.Client.Models;


namespace Trakx.Coinbase.Custody.Client
{
    public class CoinbaseClient : FlurlClient, ICoinbaseClient
    {
        private readonly IAddressEndpoint _addressEndpoint;
        private readonly IWalletEndpoint _walletEndpoint;
        private readonly ITransactionEndpoint _transactionEndpoint;
        private readonly ICurrencyEndpoint _currencyEndpoint;

        public CoinbaseClient(IApiKeyConfig api) : base("https://api.custody.coinbase.com/api/v1/")
        {
            api.Configure(this);
            _transactionEndpoint = new TransactionEndpoint(this);
            _walletEndpoint = new WalletEndpoint(this);
            _addressEndpoint = new AddressEndpoint(this);
            _currencyEndpoint=new CurrencyEndpoint(this);
        }

        #region Implementation of IAddressEndpoint

        /// <inheritdoc />
        public async Task<PagedResponse<AddressResponse>> ListAddressesAsync(string? currency = null, string? state = null, CancellationToken cancellationToken = default)
        {
            return await _addressEndpoint.ListAddressesAsync(currency, state, cancellationToken);
        }

        #endregion

        #region Implementation of IWalletEndpoint

        /// <inheritdoc />
        public async Task<PagedResponse<Wallet>> ListWalletsAsync(string? currency = null, CancellationToken cancellationToken = default)
        {
            return await _walletEndpoint.ListWalletsAsync(currency, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<Wallet> GetWalletAsync(string walletId, CancellationToken cancellationToken = default)
        {
            return await _walletEndpoint.GetWalletAsync(walletId, cancellationToken);
        }

        #endregion

        #region Implementation of ITransactionEndpoint

        /// <inheritdoc />
        public async Task<PagedResponse<Transaction>> ListTransactionsAsync(string? currency = null, string? state = null, string? walletId = null, string? type = null,
            string? startTime = null, string? endTime = null, CancellationToken cancellationToken = default)
        {
            return await _transactionEndpoint.ListTransactionsAsync(currency, state, walletId, type, startTime, endTime, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<Transaction> GetTransactionAsync(string transactionId, CancellationToken cancellationToken = default)
        {
            return await _transactionEndpoint.GetTransactionAsync(transactionId, cancellationToken);
        }

        #endregion

        #region Implementation of ICurrencyEndpoint

        /// <inheritdoc />
        public Task<PagedResponse<Currency>> ListCurrenciesAsync( CancellationToken cancellationToken = default)
        {
            return _currencyEndpoint.ListCurrenciesAsync( cancellationToken);
        }

        #endregion
    }
}
