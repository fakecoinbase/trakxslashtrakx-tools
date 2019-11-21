using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Trakx.Data.Market.Common.Indexes;
using Trakx.Data.Market.Common.Pricing;

namespace Trakx.Data.Market.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
        public async Task<ActionResult<string>> GetNetAssetValue([FromQuery] string indexSymbol)
        {
            if (!Enum.TryParse(indexSymbol, out KnownIndexes symbol))
                return $"Known index symbols are [{string.Join(", ", Enum.GetNames(typeof(KnownIndexes)))}]";

            var kaikoNav = await _navCalculator.CalculateKaikoNav(symbol, "usd").ConfigureAwait(false);
            if (kaikoNav != 0) return kaikoNav.ToString(CultureInfo.InvariantCulture);
            var messariNav = await _navCalculator.CalculateMessariNav(symbol).ConfigureAwait(false);
            return messariNav.ToString(CultureInfo.InvariantCulture);
        }
    }
}
