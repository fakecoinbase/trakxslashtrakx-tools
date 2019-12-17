using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Trakx.Data.Market.Common.Pricing;
using Trakx.Data.Models.Index;

namespace Trakx.Data.Market.Server.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class NavController : ControllerBase
    {
        private readonly IIndexDefinitionProvider _indexProvider;
        private readonly ILogger<NavController> _logger;
        private readonly INavCalculator _navCalculator;

        public NavController(IIndexDefinitionProvider indexProvider, 
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
            var definition = await _indexProvider.GetDefinitionFromSymbol(indexSymbol);

            if (definition == IndexDefinition.Default)
                return $"failed to retrieve details for index {indexSymbol}";

            var pricedDetails = await _navCalculator.GetIndexPriced(definition)
                .ConfigureAwait(false);

            return new JsonResult(pricedDetails.CurrentValuation.NetAssetValue);
        }
    }
}
