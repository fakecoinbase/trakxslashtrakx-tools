using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Trakx.Data.Common.Interfaces;
using Trakx.Data.Common.Interfaces.Index;
using Trakx.Data.Common.Interfaces.Pricing;
using Trakx.Data.Common.Utils;

namespace Trakx.Data.Market.Server.Controllers
{
    /// <summary>
    /// Provides methods to get information about the Net Asset Value (NAV) of indexes.
    /// For more detailed information about the definitions of the indexes themselves, please
    /// refer to <see cref="IndexDataController"/>.
    /// </summary>
    [ApiController]
    [Route("[controller]/[action]")]
    public class NavController : ControllerBase
    {
        private readonly IIndexDataProvider _indexProvider;
        private readonly ILogger<NavController> _logger;
        private readonly INavCalculator _navCalculator;

        public NavController(IIndexDataProvider indexProvider, 
            INavCalculator navCalculator,
            ILogger<NavController> logger)
        {
            _indexProvider = indexProvider;
            _logger = logger;
            _navCalculator = navCalculator;
        }

        /// <summary>
        /// Returns the USDc Net Asset Value of a given index.
        /// </summary>
        /// <param name="indexSymbol">The symbol for the index on which data is requested.</param>
        /// <param name="maxRandomVariation">
        /// [DEVELOPMENT ONLY] Adds a random variation to the NAV.
        /// This was created to allow trading to happen by getting different hummingbots to get slightly dissimilar prices.</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<string>> GetUsdNetAssetValue([FromQuery] string indexSymbol, [FromQuery]decimal maxRandomVariation = 0)
        {
            var currentComposition = await _indexProvider.GetCurrentComposition(indexSymbol);

            if (currentComposition == default(IIndexDefinition))
                return $"failed to retrieve composition for index {indexSymbol}";

            var currentValuation = await _navCalculator.GetIndexValuation(currentComposition)
                .ConfigureAwait(false);

            return new JsonResult(currentValuation.NetAssetValue.AddRandomVariation(maxRandomVariation));
        }
    }
}
