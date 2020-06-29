using System;
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
    /// Provides endpoints related indice information
    /// </summary>
    [ApiController]
    [Route("[controller]/[action]")]
    public class IndexDataController : ControllerBase
    {
        private readonly IIndiceDataProvider _indiceProvider;
        private readonly INavCalculator _navCalculator;
        private readonly IHostEnvironment _hostEnvironment;

        private readonly ILogger<IndexDataController> _logger;

        public IndexDataController(IIndiceDataProvider indiceProvider,
            INavCalculator navCalculator,
            IHostEnvironment hostEnvironment,
            ILogger<IndexDataController> logger)
        {
            _indiceProvider = indiceProvider;
            _navCalculator = navCalculator;
            _hostEnvironment = hostEnvironment;
            _logger = logger;
        }

        /// <summary>
        /// Use this endpoint to retrieve details about the composition of an indice, such as issuance date, short description,
        /// component weights, valuations, associated icons, etc.
        /// </summary>
        /// <param name="indexSymbol">The symbol for the indice on which data is requested.</param>
        /// <returns>Basic information about the indice definition, including issuance and current valuation details.</returns>
        [HttpGet]
        public async Task<ActionResult<string>> IndexDetailsPriced([FromQuery] string indexSymbol)
        {
            var composition = await _indiceProvider.GetCurrentComposition(indexSymbol);

            if (composition == default)
                return new JsonResult($"failed to retrieve details for indice {indexSymbol}");

            var currentValuation = await _navCalculator.GetIndiceValuation(composition)
                .ConfigureAwait(false);

            var issuanceValuation = await _indiceProvider.GetInitialValuation(composition)
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

            var indicePriced = IndicePricedModel.FromIndiceValuations(issuanceValuation, currentValuation);
            indicePriced.ComponentDefinitions.ForEach(d => d.IconUrl = iconBySymbol[d.Symbol.ToLower()]);
            return new JsonResult(indicePriced);
        }
    }
}