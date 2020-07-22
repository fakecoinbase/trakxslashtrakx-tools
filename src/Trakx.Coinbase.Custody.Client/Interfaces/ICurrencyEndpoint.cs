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
        /// <param name="paginationOptions">Optional custom pagination options. Allows to start searching from a custom point
        /// in time, or to change default number of results returned per page.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
        /// <returns>A <see cref="PagedResponse{Currency}"/> response.</returns>
        Task<PagedResponse<Currency>> ListCurrenciesAsync(PaginationOptions? paginationOptions = default, CancellationToken cancellationToken = default);

        /// <summary>
        /// Allows to retrieve a specific currency from Coinbase Custody API
        /// </summary>
        /// <param name="symbol">The symbol of the currency that we want.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
        /// <returns>A <see cref="Currency"/> response.</returns>
        Task<Currency?> GetCurrencyAsync(string symbol, CancellationToken cancellationToken = default);
    }
}