using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Trakx.MarketData.Collector
{
    /// <summary>
    /// Service responsible of caching the latest constituents prices in Cache, in order to reduce the
    /// amount of CryptoCompare queries needed by the rest of the applications.
    /// </summary>
    public interface IPriceCache
    {
        /// <summary>
        /// Start caching the latest prices, looking for them first using CryptoCompare websocket endpoints, and,
        /// if not available, polling the REST Api for prices on a regular basis.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to be used to stop the caching task.</param>
        Task StartCaching(CancellationToken cancellationToken);

        /// <summary>
        /// All symbols for which the prices are being cached.
        /// </summary>
        IReadOnlyList<string> AllConstituentsSymbols { get; }

        /// <summary>
        /// Constituent symbols for which the prices are being cached using the websocket feeds.
        /// </summary>
        IReadOnlyList<string> WebSocketSourcedSymbols { get; }

        /// <summary>
        /// Constituent symbols for which the prices are being cached by polling the Rest API.
        /// Websocket subscriptions are preferred, but when a subscription to websocket doesn't succeed,
        /// the prices is getting fetched from the Rest API.
        /// </summary>
        IReadOnlyList<string> RestSourcedSymbols { get; }
    }
}