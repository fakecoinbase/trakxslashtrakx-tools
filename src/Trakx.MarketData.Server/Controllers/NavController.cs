using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Trakx.Common.Interfaces;
using Trakx.Common.Interfaces.Indice;
using Trakx.Common.Interfaces.Pricing;
using Trakx.Common.Pricing;
using Trakx.Common.Utils;
using Trakx.MarketData.Server.Models;

namespace Trakx.MarketData.Server.Controllers
{
    /// <summary>
    /// Provides methods to get information about the Net Asset Value (NAV) of indices.
    /// For more detailed information about the definitions of the indices themselves, please
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

        /// <summary>
        /// Calculates index or composition valuations for several points in past time.
        /// </summary>
        /// <param name="indiceOrCompositionSymbol">The symbol of the index or composition for which the valuations will be returned.
        /// If an index symbol is provided, for each point in time, the composition used for calculations will be the one that
        /// was trading at that moment.</param>
        /// <param name="startTime">Earliest time for which the valuations are requested.</param>
        /// <param name="period">Period with which the valuations will be calculated.</param>
        /// <param name="endTime">Latest time for which the valuations are requested.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DetailedHistoricalNavsByTimestampModel>> GetHistoricalUsdcNetAssetValues(
            [FromQuery] string indiceOrCompositionSymbol,
            [FromQuery] DateTime startTime,
            [FromQuery] Period period,
            [FromQuery] DateTime? endTime = default,
            CancellationToken cancellationToken = default)
        {
            if (!indiceOrCompositionSymbol.IsIndiceSymbol() && !indiceOrCompositionSymbol.IsCompositionSymbol())
                return BadRequest($"{indiceOrCompositionSymbol} is not a valid symbol.");

            IEnumerable<IIndiceValuation> valuations;
            if (indiceOrCompositionSymbol.IsCompositionSymbol())
            {
                var composition = await _indiceProvider.GetCompositionFromSymbol(indiceOrCompositionSymbol, cancellationToken);
                if(composition == default) return BadRequest($"Unknown composition {indiceOrCompositionSymbol}.");
                valuations = await _navCalculator.GetCompositionValuations(composition, startTime, period, endTime, cancellationToken);
            }
            else
            {
                var index = await _indiceProvider.GetDefinitionFromSymbol(indiceOrCompositionSymbol, cancellationToken);
                if (index == default) return BadRequest($"Unknown index {indiceOrCompositionSymbol}.");
                valuations = await _navCalculator.GetIndexValuations(index, startTime, period, endTime, cancellationToken);
            }

            var result = new DetailedHistoricalNavsByTimestampModel(
                indiceOrCompositionSymbol, startTime, period, valuations, endTime);

            return Ok(result);
        }
    }
}
