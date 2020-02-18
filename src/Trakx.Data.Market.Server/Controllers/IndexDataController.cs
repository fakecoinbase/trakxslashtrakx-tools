using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Trakx.Data.Common.Interfaces;
using Trakx.Data.Common.Interfaces.Pricing;
using Trakx.Data.Market.Server.Models;

namespace Trakx.Data.Market.Server.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class IndexDataController : ControllerBase
    {
        private const string QuestionMarkIcon = "/crypto-icons/svg/local/_question-mark.svg";

        private readonly IIndexDataProvider _indexProvider;
        private readonly INavCalculator _navCalculator;
        private readonly IHostEnvironment _hostEnvironment;

        private readonly ILogger<IndexDataController> _logger;

        public IndexDataController(IIndexDataProvider indexProvider,
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
            var composition = await _indexProvider.GetCurrentComposition(indexSymbol);
            
            if (composition == default)
                return $"failed to retrieve details for index {indexSymbol}";

            var currentValuation = await _navCalculator.GetIndexValuation(composition)
                .ConfigureAwait(false);

            var issuanceValuation = await _indexProvider.GetInitialValuation(composition);

            var iconBySymbol = currentValuation.ComponentValuations
                .Select(d => d.ComponentQuantity.ComponentDefinition.Symbol.ToLower())
                .ToDictionary(
                    s => s,
                    s =>
                    {
                        var symbol = s;
                        var candidateImagePath = Path.Combine("wwwroot", "crypto-icons", "svg", "imported", $"{s}.svg");
                        var foundIcon = _hostEnvironment.ContentRootFileProvider.GetFileInfo(candidateImagePath).Exists;
                        if (foundIcon) return $"/crypto-icons/svg/imported/{s}.svg";

                        _logger.LogDebug("Failed to retrieve icon for symbol {0}", symbol);

                        return QuestionMarkIcon;
                    });

            var indexPriced = IndexPricedModel.FromIndexValuations(issuanceValuation, currentValuation);
            indexPriced.ComponentDefinitions.ForEach(d => d.IconUrl = iconBySymbol[d.Symbol.ToLower()]);

            return new JsonResult(indexPriced);
        }
    }
}
