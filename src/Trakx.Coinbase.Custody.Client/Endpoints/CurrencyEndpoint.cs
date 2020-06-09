using System.Threading;
using System.Threading.Tasks;
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
        public async Task<PagedResponse<Currency>> ListCurrenciesAsync(CancellationToken cancellationToken = default)
        {
            return await _client
                .Request("currencies")
                .GetJsonAsync<PagedResponse<Currency>>(cancellationToken);
        }
    }
}
