using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Trakx.Data.Market.Common.Indexes;
using Trakx.Data.Market.Common.Pricing;

namespace Trakx.Data.Market.Server.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class IndexDataController : ControllerBase
    {
        private readonly IIndexDetailsProvider _indexDetailsProvider;
        private readonly INavCalculator _navCalculator;

        private readonly ILogger<IndexDataController> _logger;

        public IndexDataController(IIndexDetailsProvider indexDetailsProvider,
            INavCalculator navCalculator,
            ILogger<IndexDataController> logger)
        {
            _indexDetailsProvider = indexDetailsProvider;
            _navCalculator = navCalculator;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<string> IndexDetails([FromQuery] string indexSymbol)
        {
            if (!Enum.TryParse(indexSymbol, out KnownIndexes symbol))
                return $"Known index symbols are [{string.Join(", ", Enum.GetNames(typeof(KnownIndexes)))}]";

            if (!_indexDetailsProvider.IndexDetails.TryGetValue(symbol, out var details))
                return $"failed to retrieve details for index {indexSymbol}";

            return new JsonResult(details);
        }

        [HttpGet]
        public async Task<ActionResult<string>> IndexDetailsPriced([FromQuery] string indexSymbol)
        {
            if (!Enum.TryParse(indexSymbol, out KnownIndexes symbol))
                return $"Known index symbols are [{string.Join(", ", Enum.GetNames(typeof(KnownIndexes)))}]";

            if (!_indexDetailsProvider.IndexDetails.TryGetValue(symbol, out var details))
                return $"failed to retrieve details for index {indexSymbol}";

            var pricedDetails = await _navCalculator.GetCryptoCompareIndexDetailsPriced(symbol)
                .ConfigureAwait(false);

            return new JsonResult(pricedDetails);
        }
    }
}
