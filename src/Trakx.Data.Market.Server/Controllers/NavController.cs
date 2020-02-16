using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Trakx.Data.Common.Interfaces;
using Trakx.Data.Common.Interfaces.Index;
using Trakx.Data.Common.Interfaces.Pricing;
using Trakx.Data.Common.Utils;

namespace Trakx.Data.Market.Server.Controllers
{
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
        /// <param name="indexSymbol"></param>
        /// <param name="maxRandomVariation">Adds a random variation to the NAV.
        /// ******** ONLY SET THIS IN DEVELOPMENT ********.
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
