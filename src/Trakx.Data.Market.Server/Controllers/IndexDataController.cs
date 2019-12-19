using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Trakx.Data.Market.Common.Pricing;
using Trakx.Data.Models.Index;

namespace Trakx.Data.Market.Server.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class IndexDataController : ControllerBase
    {
        private readonly IIndexDefinitionProvider _indexProvider;
        private readonly INavCalculator _navCalculator;

        private readonly ILogger<IndexDataController> _logger;

        public IndexDataController(IIndexDefinitionProvider indexProvider,
            INavCalculator navCalculator,
            ILogger<IndexDataController> logger)
        {
            _indexProvider = indexProvider;
            _navCalculator = navCalculator;
            _logger = logger;
            _logger.LogDebug("Instantiated");
        }

        [HttpGet]
        public async Task<ActionResult<string>> IndexDetails([FromQuery] string indexSymbol)
        {
            var definition = await _indexProvider.GetDefinitionFromSymbol(indexSymbol);

            if (definition == IndexDefinition.Default)
                return $"failed to retrieve details for index {indexSymbol}";

            return new JsonResult(definition);
        }

        [HttpGet]
        public async Task<ActionResult<string>> IndexDetailsPriced([FromQuery] string indexSymbol)
        {
            var definition = await _indexProvider. GetDefinitionFromSymbol(indexSymbol);

            if (definition == IndexDefinition.Default)
                return $"failed to retrieve details for index {indexSymbol}";

            var pricedDetails = await _navCalculator.GetIndexPriced(definition)
                .ConfigureAwait(false);

            return new JsonResult(pricedDetails);
        }
    }
}
