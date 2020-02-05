using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Trakx.Data.Common.Interfaces;
using Trakx.Data.Common.Interfaces.Index;
using Trakx.Data.Common.Interfaces.Pricing;

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

        [HttpGet]
        public async Task<ActionResult<string>> GetUsdNetAssetValue([FromQuery] string indexSymbol)
        {
            var currentComposition = await _indexProvider.GetCurrentComposition(indexSymbol);

            if (currentComposition == default(IIndexDefinition))
                return $"failed to retrieve composition for index {indexSymbol}";

            var currentValuation = await _navCalculator.GetIndexValuation(currentComposition)
                .ConfigureAwait(false);

            return new JsonResult(currentValuation.NetAssetValue);
        }
    }
}
