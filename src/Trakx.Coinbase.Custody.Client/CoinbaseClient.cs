using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Trakx.Coinbase.Custody.Client.Interfaces;
using Trakx.Coinbase.Custody.Client.Models;


namespace Trakx.Coinbase.Custody.Client
{
    public class CoinbaseClient : ICoinbaseClient
    {
        private readonly IAddressEndpoint _addressEndpoint;
        private readonly IWalletEndpoint _walletEndpoint;
        private readonly ITransactionEndpoint _transactionEndpoint;
        private readonly ICurrencyEndpoint _currencyEndpoint;

        public CoinbaseClient(IAddressEndpoint addressEndpoint, IWalletEndpoint walletEndpoint, ITransactionEndpoint transactionEndpoint, ICurrencyEndpoint currencyEndpoint)
        {
            _addressEndpoint = addressEndpoint;
            _walletEndpoint = walletEndpoint;
            _transactionEndpoint = transactionEndpoint;
            _currencyEndpoint = currencyEndpoint;
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<CoinbaseRawTransaction> GetTransactions(string? currency = null,
            TransactionState? state = null,
            string? walletId = null,
            TransactionType? type = null,
            DateTime? startTime = null,
            DateTime? endTime = null,
            PaginationOptions? paginationOptions = null,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            async Task<PagedResponse<CoinbaseRawTransaction>> FetchPage(PaginationOptions? pagination) => 
                await _transactionEndpoint.ListTransactionsAsync(currency, state, walletId, type, 
                startTime, endTime,
                pagination, cancellationToken);

            await foreach (var transaction in FromPagedResponse(FetchPage, paginationOptions)
                .WithCancellation(cancellationToken))
            {
                yield return transaction;
            }
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<Currency> GetCurrencies(PaginationOptions? paginationOptions = null,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            async Task<PagedResponse<Currency>> FetchPage(PaginationOptions? pagination) =>
                await _currencyEndpoint.ListCurrenciesAsync(pagination, cancellationToken);

            await foreach (var currency in FromPagedResponse(FetchPage, paginationOptions)
                .WithCancellation(cancellationToken))
            {
                yield return currency;
            }
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<Wallet> GetWallets(string? currency = null, PaginationOptions? paginationOptions = null,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            async Task<PagedResponse<Wallet>> FetchPage(PaginationOptions? pagination) =>
                await _walletEndpoint.ListWalletsAsync(currency, pagination, cancellationToken);

            await foreach (var wallet in FromPagedResponse(FetchPage, paginationOptions)
                .WithCancellation(cancellationToken))
            {
                yield return wallet;
            }
        }


        /// <summary>
        /// This function can be reused for all other page responses.
        /// </summary>
        internal async IAsyncEnumerable<T> FromPagedResponse<T>(
            Func<PaginationOptions, Task<PagedResponse<T>>> fetchPage, PaginationOptions paginationOptions)
        {
            PagedResponse<T> page;
            do
            {
                try
                {
                    page = await fetchPage(paginationOptions).ConfigureAwait(false);
                    paginationOptions = new PaginationOptions(page.Pagination?.After, default, paginationOptions?.PageSize);
                }
                catch (Exception)
                {
                    yield break;
                }

                foreach (var data in page.Data)
                {
                    yield return data;
                }
            } while (page.Data.Length >= paginationOptions.PageSize);
        }

        #region Implementation of IAddressEndpoint

        /// <inheritdoc />
        public async Task<PagedResponse<AddressResponse>> ListAddressesAsync(string? currency = null, string? state = null, PaginationOptions paginationOptions = default,
            CancellationToken cancellationToken = default)
        {
            return await _addressEndpoint.ListAddressesAsync(currency, state, paginationOptions, cancellationToken);
        }

        #endregion

        #region Implementation of ITransactionEndpoint

        /// <inheritdoc />
        public async Task<PagedResponse<CoinbaseRawTransaction>> ListTransactionsAsync(string? currency = null, TransactionState? state = null, string? walletId = null,
            TransactionType? type = null, DateTime? startTime = null, DateTime? endTime = null,
            PaginationOptions paginationOptions = default, CancellationToken cancellationToken = default)
        {
            return await _transactionEndpoint.ListTransactionsAsync(currency, state, walletId, type, startTime, endTime, paginationOptions, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<CoinbaseRawTransaction> GetTransactionAsync(string transactionId, CancellationToken cancellationToken = default)
        {
            return await _transactionEndpoint.GetTransactionAsync(transactionId, cancellationToken);
        }

        #endregion

        #region Implementation of IWalletEndpoint

        /// <inheritdoc />
        public async Task<PagedResponse<Wallet>> ListWalletsAsync(string? currency = null, PaginationOptions paginationOptions = default,
            CancellationToken cancellationToken = default)
        {
            return await _walletEndpoint.ListWalletsAsync(currency, paginationOptions, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<Wallet> GetWalletAsync(string walletId, CancellationToken cancellationToken = default)
        {
            return await _walletEndpoint.GetWalletAsync(walletId, cancellationToken);
        }

        #endregion

        #region Implementation of ICurrencyEndpoint

        /// <inheritdoc />
        public async Task<PagedResponse<Currency>> ListCurrenciesAsync(PaginationOptions paginationOptions, CancellationToken cancellationToken = default)
        {
            return await _currencyEndpoint.ListCurrenciesAsync(paginationOptions, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<Currency> GetCurrencyAsync(string symbol, CancellationToken cancellationToken = default)
        {
            return await _currencyEndpoint.GetCurrencyAsync(symbol, cancellationToken);
        }

        #endregion
    }
}
