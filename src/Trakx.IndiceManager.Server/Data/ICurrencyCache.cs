using System.Threading;
using System.Threading.Tasks;
using Trakx.Coinbase.Custody.Client.Models;

namespace Trakx.IndiceManager.Server.Data
{
    /// <summary>
    /// Stores details about Coinbase Custody currencies in cache
    /// to avoid using the Rest method too often.
    /// </summary>
    /// <remarks>
    /// todo: this might belong to the CoinbaseClient implementation? 🤔
    /// </remarks>
    public interface ICurrencyCache
    {
        /// <summary>
        /// Retrieves the number of decimals implemented on a currency.
        /// </summary>
        /// <param name="symbol">The symbol of the currency for which to retrieve the decimals.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
        Task<ushort?> GetDecimalsForCurrency(string symbol, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves from or adds all the details about a currency into a memory cache.
        /// </summary>
        /// <param name="symbol">The symbol of the currency for which to retrieve the details.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
        Task<Currency?> GetCurrency(string symbol, CancellationToken cancellationToken = default);
    }
}