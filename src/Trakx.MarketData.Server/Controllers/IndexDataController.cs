using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Trakx.Common.Interfaces;
using Trakx.Common.Interfaces.Pricing;
using Trakx.MarketData.Server.Models;

namespace Trakx.MarketData.Server.Controllers
{
    /// <summary>
    /// Provides endpoints related index information
    /// </summary>
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

        /// <summary>
        /// Use this endpoint to retrieve details about the composition of an index, such as issuance date, short description,
        /// component weights, valuations, associated icons, etc.
        /// </summary>
        /// <param name="indexSymbol">The symbol for the index on which data is requested.</param>
        /// <returns>Basic information about the index definition, including issuance and current valuation details.</returns>
        [HttpGet]
        public async Task<ActionResult<string>> IndexDetailsPriced([FromQuery] string indexSymbol)
        {
            var composition = await _indexProvider.GetCurrentComposition(indexSymbol);
            
            if (composition == default)
                return new JsonResult($"failed to retrieve details for index {indexSymbol}");

            var currentValuation = await _navCalculator.GetIndexValuation(composition)
                .ConfigureAwait(false);

            var issuanceValuation = await _indexProvider.GetInitialValuation(composition)
                .ConfigureAwait(false);

            var iconBySymbol = currentValuation.ComponentValuations
                .Select(d => d.ComponentQuantity.ComponentDefinition.Symbol.ToLower())
                .ToDictionary(
                    s => s,
                    s =>
                    {
                        var candidateImagePath = Path.Combine("wwwroot", "crypto-icons", "svg", "imported", $"{s}.svg");
                        var foundIcon = _hostEnvironment.ContentRootFileProvider.GetFileInfo(candidateImagePath).Exists;
                        var iconName = foundIcon ? s : "generic";
                        return $"/crypto-icons/svg/imported/{iconName}.svg";
                    });

            var indexPriced = IndexPricedModel.FromIndexValuations(issuanceValuation, currentValuation);
            indexPriced.ComponentDefinitions.ForEach(d => d.IconUrl = iconBySymbol[d.Symbol.ToLower()]);
            return new JsonResult(indexPriced);
        }
    }
}
