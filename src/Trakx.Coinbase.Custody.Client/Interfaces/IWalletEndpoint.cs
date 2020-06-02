﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Trakx.Coinbase.Custody.Client.Models;

namespace Trakx.Coinbase.Custody.Client.Interfaces
{
    /// <summary>
    /// Use this Endpoint to retrieve data about Wallets
    /// </summary>
    public interface IWalletEndpoint
    {
        /// <summary>
        /// Lists current user’s wallets to which the authentication method has access to.
        /// </summary>
        /// <param name="currency">The currency of the wallet, optional parameter.</param>
        /// <returns></returns>
        Task<PagedResponse<Wallet>> ListWalletsAsync([CanBeNull] string currency=null, CancellationToken cancellationToken=default);

        /// <summary>
        /// Allows you to retrieve a specific wallet if you have the wallet Id.
        /// </summary>
        /// <param name="walletId">The Id of the wallet that we looking for.</param>
        /// <returns>A <see cref="Wallet"/> if request succeed.</returns>
        Task<Wallet> GetWalletAsync(string walletId, CancellationToken cancellationToken=default);
    }
}