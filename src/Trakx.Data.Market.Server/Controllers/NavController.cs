using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CryptoCompare;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Trakx.Data.Market.Common.Indexes;

namespace Trakx.Data.Market.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NavController : ControllerBase
    {
        private readonly ILogger<NavController> _logger;
        private readonly CryptoCompareClient _cryptoCompareClient;

        public NavController(ILogger<NavController> logger)
        {
            _logger = logger;
            _cryptoCompareClient = new CryptoCompareClient("5f95e17ff4599da5bc6f4b309c2e0b27d3a73ddfaba843a63be66be7ebc3e79e");

        }

        [HttpGet]
        public async Task<ActionResult<string>> NetAssetValue([FromQuery] string indexSymbol, [FromQuery]string quoteSymbol = "USDC")
        {
            if (!Enum.TryParse(indexSymbol, out KnownIndexes symbol))
                return $"Known index symbols are [{string.Join(", ", Enum.GetNames(typeof(KnownIndexes)))}]";

            if (!TrackerDetails.IndexDetails.TryGetValue(symbol, out var details))
                return $"failed to retrieve details for index {indexSymbol}";

            var components = details.Components.Select(c => c.Symbol);

            var prices = await _cryptoCompareClient.Prices.MultipleSymbolsPriceAsync(
                components, new[] { quoteSymbol }, null, null);
            var nav = details.Components.Sum(c => (ulong)c.Quantity * prices[c.Symbol][quoteSymbol]);
            return nav.ToString(CultureInfo.InvariantCulture);
        }
    }
}
