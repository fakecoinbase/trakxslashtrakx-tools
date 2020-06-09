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
        /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
        /// <returns></returns>
        Task<PagedResponse<Currency>> ListCurrenciesAsync(CancellationToken cancellationToken = default);
    }
}