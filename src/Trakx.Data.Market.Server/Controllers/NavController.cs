using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CryptoCompare;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Trakx.Data.Market.Common.Indexes;
using Trakx.Data.Market.Common.Pricing;
using Trakx.Data.Market.Common.Sources.Messari.Client;

namespace Trakx.Data.Market.Server.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class NavController : ControllerBase
    {
        private readonly ILogger<NavController> _logger;
        private readonly NavCalculator _navCalculator;

        public NavController(IIndexDetailsProvider indexDetailsProvider, 
            NavCalculator navCalculator,
            ILogger<NavController> logger)
        {
            _logger = logger;
            _navCalculator = navCalculator;
        }

        [HttpGet]
        public async Task<ActionResult<string>> GetNetAssetValue([FromQuery] string indexSymbol, [FromQuery]string quoteSymbol = "USDC")
        {
            if (!Enum.TryParse(indexSymbol, out KnownIndexes symbol))
                return $"Known index symbols are [{string.Join(", ", Enum.GetNames(typeof(KnownIndexes)))}]";

            var nav = await _navCalculator.CalculateMessariNav(symbol).ConfigureAwait(false);
            return nav.ToString(CultureInfo.InvariantCulture);
        }
    }
}
