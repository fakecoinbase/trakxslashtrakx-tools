using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Trakx.Common.Interfaces;
using Trakx.Common.Interfaces.Indice;
using Trakx.Common.Interfaces.Pricing;
using Trakx.Common.Utils;

namespace Trakx.MarketData.Server.Controllers
{
    /// <summary>
    /// Provides methods to get information about the Net Asset Value (NAV) of indicees.
    /// For more detailed information about the definitions of the indicees themselves, please
    /// refer to <see cref="IndiceDataController"/>.
    /// </summary>
    [ApiController]
    [Route("[controller]/[action]")]
    public class NavController : ControllerBase
    {
        private readonly IIndiceDataProvider _indiceProvider;
        private readonly ILogger<NavController> _logger;
        private readonly INavCalculator _navCalculator;
        private readonly IDateTimeProvider _dateTimeProvider;

        public NavController(IIndiceDataProvider indiceProvider, 
            INavCalculator navCalculator,
            IDateTimeProvider dateTimeProvider,
            ILogger<NavController> logger)
        {
            _indiceProvider = indiceProvider;
            _logger = logger;
            _navCalculator = navCalculator;
            _dateTimeProvider = dateTimeProvider;
        }

        /// <summary>
        /// Returns the USDc Net Asset Value of a given indice.
        /// </summary>
        /// <param name="indiceOrCompositionSymbol">The symbol for the indice on which data is requested.</param>
        /// <param name="compositionAsOf">DateTime as of which the composition of the indice is retrieved.
        /// This is ignored if <see cref="indiceOrCompositionSymbol"/> is used to specify a given composition.</param>
        /// <param name="componentPricesAsOf">DateTime as of which the prices of the components are retrieved.</param>
        /// <param name="maxRandomVariation">
        /// [DEVELOPMENT ONLY] Adds a random variation to the NAV.
        /// This was created to allow trading to happen by getting different hummingbots to get slightly dissimilar prices.</param>
        /// <param name="cancellationToken">Token used to cancel the query.</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<string>> GetUsdNetAssetValue([FromQuery] string indiceOrCompositionSymbol, 
            [FromQuery]DateTime? componentPricesAsOf = default,
            [FromQuery]DateTime? compositionAsOf = default,
            [FromQuery]decimal maxRandomVariation = 0,
            CancellationToken cancellationToken = default)
        {
            if(!indiceOrCompositionSymbol.IsIndiceSymbol() && !indiceOrCompositionSymbol.IsCompositionSymbol())
                return new JsonResult($"{indiceOrCompositionSymbol} is not a valid symbol.");

            var utcNow = _dateTimeProvider.UtcNow;
            compositionAsOf ??= utcNow;
            var composition = indiceOrCompositionSymbol.IsCompositionSymbol()
                ? await _indiceProvider.GetCompositionFromSymbol(indiceOrCompositionSymbol, cancellationToken)
                : indiceOrCompositionSymbol.IsIndiceSymbol()
                    ? await _indiceProvider.GetCompositionAtDate(indiceOrCompositionSymbol, compositionAsOf.Value, cancellationToken)
                    : default;

            if (composition == default)
                return new JsonResult($"failed to retrieve composition for indice {indiceOrCompositionSymbol}.");

            var currentValuation = await _navCalculator.GetIndiceValuation(composition, componentPricesAsOf)
                .ConfigureAwait(false);

            return new JsonResult(currentValuation.NetAssetValue.AddRandomVariation(maxRandomVariation));
        }
    }
}
