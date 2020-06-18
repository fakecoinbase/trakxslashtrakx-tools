using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Flurl.Http;
using Trakx.Coinbase.Custody.Client.Interfaces;
using Trakx.Coinbase.Custody.Client.Models;

namespace Trakx.Coinbase.Custody.Client.Endpoints
{
    public class CurrencyEndpoint : ICurrencyEndpoint
    {
        private readonly ICoinbaseClient _client;

        internal CurrencyEndpoint(ICoinbaseClient client)
        {
            _client = client;
        }

        /// <inheritdoc />
        public async Task<PagedResponse<Currency>> ListCurrenciesAsync(string? before = null, string? after = null, int? limit = null, CancellationToken cancellationToken = default)
        {
            return await _client
                .Request("currencies")
                .SetQueryParams(new { before, after, limit })
                .GetJsonAsync<PagedResponse<Currency>>(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<Currency> GetCurrencyAsync(string symbol, CancellationToken cancellationToken = default)
        {
            Guard.Against.NullOrEmpty(symbol, nameof(symbol));
            return await _client.Request("currencies", symbol).GetJsonAsync<Currency>(cancellationToken);
        }
    }
}
