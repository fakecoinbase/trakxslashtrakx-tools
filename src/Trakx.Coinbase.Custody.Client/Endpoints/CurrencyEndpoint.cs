using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Flurl.Http;
using Trakx.Coinbase.Custody.Client.Interfaces;
using Trakx.Coinbase.Custody.Client.Models;

namespace Trakx.Coinbase.Custody.Client.Endpoints
{
    internal class CurrencyEndpoint : ICurrencyEndpoint
    {
        private readonly IFlurlClient _client;

        public CurrencyEndpoint(IFlurlClient client)
        {
            _client = client;
        }

        /// <inheritdoc />
        public async Task<PagedResponse<Currency>> ListCurrenciesAsync(PaginationOptions? paginationOptions = null, CancellationToken cancellationToken = default)
        {
            paginationOptions ??= PaginationOptions.Default;
            var page= await _client.Request("currencies")
                .SetQueryParams(new
                {
                    before = paginationOptions.Before,
                    after = paginationOptions.After,
                    limit = paginationOptions.PageSize,
                })
                .GetJsonAsync<PagedResponse<Currency>>(cancellationToken);
            return page;
        }

        /// <inheritdoc />
        public async Task<Currency> GetCurrencyAsync(string symbol, CancellationToken cancellationToken = default)
        {
            Guard.Against.NullOrEmpty(symbol, nameof(symbol));
            return await _client.Request("currencies", symbol).GetJsonAsync<Currency>(cancellationToken);
        }
    }
}
