using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Trakx.MarketApi.Indexes;

namespace Trakx.MarketApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IndexDataController : ControllerBase
    {
        private readonly ILogger<IndexDataController> _logger;

        public IndexDataController(ILogger<IndexDataController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<string> IndexDetails([FromQuery] string indexSymbol)
        {
            if (!Enum.TryParse(indexSymbol, out KnownIndexes symbol))
                return $"Known index symbols are [{string.Join(", ", Enum.GetNames(typeof(KnownIndexes)))}]";

            if (!TrackerDetails.IndexDetails.TryGetValue(symbol, out var details))
                return $"failed to retrieve details for index {indexSymbol}";

            return new JsonResult(details);
        }
    }
}
