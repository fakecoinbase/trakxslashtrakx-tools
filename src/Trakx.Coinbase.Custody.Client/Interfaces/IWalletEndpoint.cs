using System.Threading;
using System.Threading.Tasks;
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
        /// <param name="paginationOptions">Optional custom pagination options. Allows to start searching from a custom point
        /// in time, or to change default number of results returned per page.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>A <see cref="PagedResponse{Wallet}"/> if request succeed.</returns>
        Task<PagedResponse<Wallet>> ListWalletsAsync(string? currency = null, PaginationOptions? paginationOptions = default,
            CancellationToken cancellationToken = default);


        /// <summary>
        /// Allows you to retrieve a specific wallet if you have the wallet Id.
        /// </summary>
        /// <param name="walletId">The Id of the wallet that we looking for.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>A <see cref="Wallet"/> if request succeed.</returns>
        Task<Wallet> GetWalletAsync(string walletId, CancellationToken cancellationToken = default);
    }
}