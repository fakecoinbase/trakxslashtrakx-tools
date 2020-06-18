using System.Threading;
using System.Threading.Tasks;
using Trakx.Coinbase.Custody.Client.Models;

namespace Trakx.Coinbase.Custody.Client.Interfaces
{
    public interface ICurrencyEndpoint
    {
        /// <summary>
        /// Allows to retrieve all of the currencies available on Coinbase Custody API
        /// </summary>
        /// <param name="before">Request page before (newer than) this pagination id.</param>
        /// <param name="after">Request page after (older than) this pagination id.</param>
        /// <param name="limit">Number of results per request. Maximum 100. Default 25.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
        /// <returns>A <see cref="PagedResponse{Currency}"/> response.</returns>
        Task<PagedResponse<Currency>> ListCurrenciesAsync(string? before = null, string? after = null,
            int? limit = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Allows to retrieve a specific currency from Coinbase Custody API
        /// </summary>
        /// <param name="symbol">The symbol of the currency that we want.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
        /// <returns>A <see cref="Currency"/> response.</returns>
        Task<Currency> GetCurrencyAsync(string symbol, CancellationToken cancellationToken = default);
    }
}