using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Trakx.Data.Market.Common.Indexes;
using Trakx.Data.Market.Server.Data;

namespace Trakx.Data.Market.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IndexDataController : ControllerBase
    {
        private readonly IIndexDetailsProvider _indexDetailsProvider;

        private readonly ILogger<IndexDataController> _logger;
        //private readonly JsInterop _interop;

        public IndexDataController(IIndexDetailsProvider indexDetailsProvider, ILogger<IndexDataController> logger)
        {
            _indexDetailsProvider = indexDetailsProvider;
            _logger = logger;
            //_interop = interop;
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

        //[HttpGet]
        //public async Task<ActionResult<string>> GetGreetings([FromQuery] string name)
        //{
        //    var greeting = await _interop.Greet(name);
        //    return new JsonResult(greeting);
        //}
    }
}
