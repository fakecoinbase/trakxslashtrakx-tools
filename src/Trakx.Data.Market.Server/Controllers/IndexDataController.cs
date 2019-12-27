using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Trakx.Data.Market.Common.Pricing;
using Trakx.Data.Models.Index;

namespace Trakx.Data.Market.Server.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class IndexDataController : ControllerBase
    {
        private const string QuestionMarkIcon = "_question_mark";
        private const string IconFileTemplate = "/crypto-icons/svg/icon/{0}.svg";

        private readonly IIndexDefinitionProvider _indexProvider;
        private readonly INavCalculator _navCalculator;
        private readonly IHostEnvironment _hostEnvironment;

        private readonly ILogger<IndexDataController> _logger;

        public IndexDataController(IIndexDefinitionProvider indexProvider,
            INavCalculator navCalculator,
            IHostEnvironment hostEnvironment,
            ILogger<IndexDataController> logger)
        {
            _indexProvider = indexProvider;
            _navCalculator = navCalculator;
            _hostEnvironment = hostEnvironment;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<string>> IndexDetailsPriced([FromQuery] string indexSymbol)
        {
            var definition = await _indexProvider.GetDefinitionFromSymbol(indexSymbol);

            if (definition == IndexDefinition.Default)
                return $"failed to retrieve details for index {indexSymbol}";

            var pricedDetails = await _navCalculator.GetIndexPriced(definition)
                .ConfigureAwait(false);
            
            pricedDetails.ComponentDefinitions.ForEach(d =>
            {
                var candidateImagePath = Path.Combine("wwwroot", "crypto-icons", "svg", "icon", $"{d.Symbol.ToLower()}.svg");
                var foundIcon = _hostEnvironment.ContentRootFileProvider.GetFileInfo(candidateImagePath).Exists;

                if (!foundIcon)
                {
                    _logger.LogDebug("Failed to retrieve icon for symbol {0}", d.Symbol);
                }

                d.IconUrl = string.Format(IconFileTemplate, foundIcon ? d.Symbol.ToLower() : QuestionMarkIcon);
            });

            return new JsonResult(pricedDetails);
        }
    }
}
