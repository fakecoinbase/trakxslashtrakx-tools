using System;
using System.Collections.Generic;
using System.Threading;
using Trakx.Coinbase.Custody.Client.Models;

namespace Trakx.Coinbase.Custody.Client.Interfaces
{
    public interface ICoinbaseClient : IAddressEndpoint, IWalletEndpoint, ITransactionEndpoint,ICurrencyEndpoint
    {
        IAsyncEnumerable<CoinbaseRawTransaction> GetTransactions(string? currency = null, TransactionState? state = null,
            string? walletId = null, TransactionType? type = null,
            DateTime? startTime = null, DateTime? endTime = null, 
            PaginationOptions? paginationOptions = null,
            CancellationToken cancellationToken = default);

        IAsyncEnumerable<Currency> GetCurrencies(PaginationOptions? paginationOptions = null, 
            CancellationToken cancellationToken = default);

        IAsyncEnumerable<Wallet> GetWallets(string? currency = null, PaginationOptions? paginationOptions = null,
            CancellationToken cancellationToken = default);
    }
}