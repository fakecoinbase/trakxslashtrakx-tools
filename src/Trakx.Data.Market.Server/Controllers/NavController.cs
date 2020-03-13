using System;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Trakx.Data.Common.Interfaces;
using Trakx.Data.Common.Interfaces.Index;
using Trakx.Data.Common.Interfaces.Pricing;
using Trakx.Data.Common.Utils;

namespace Trakx.Data.Market.Server.Controllers
{
    /// <summary>
    /// Provides methods to get information about the Net Asset Value (NAV) of indexes.
    /// For more detailed information about the definitions of the indexes themselves, please
    /// refer to <see cref="IndexDataController"/>.
    /// </summary>
    [ApiController]
    [Route("[controller]/[action]")]
    public class NavController : ControllerBase
    {
        private readonly IIndexDataProvider _indexProvider;
        private readonly ILogger<NavController> _logger;
        private readonly INavCalculator _navCalculator;
        private readonly IDateTimeProvider _dateTimeProvider;

        public NavController(IIndexDataProvider indexProvider, 
            INavCalculator navCalculator,
            IDateTimeProvider dateTimeProvider,
            ILogger<NavController> logger)
        {
            _indexProvider = indexProvider;
            _logger = logger;
            _navCalculator = navCalculator;
            _dateTimeProvider = dateTimeProvider;
        }

        /// <summary>
        /// Returns the USDc Net Asset Value of a given index.
        /// </summary>
        /// <param name="indexOrCompositionSymbol">The symbol for the index on which data is requested.</param>
        /// <param name="compositionAsOf">DateTime as of which the composition of the index is retrieved.
        /// This is ignored if <see cref="indexOrCompositionSymbol"/> is used to specify a given composition.</param>
        /// <param name="componentPricesAsOf">DateTime as of which the prices of the components are retrieved.</param>
        /// <param name="maxRandomVariation">
        /// [DEVELOPMENT ONLY] Adds a random variation to the NAV.
        /// This was created to allow trading to happen by getting different hummingbots to get slightly dissimilar prices.</param>
        /// <param name="cancellationToken">Token used to cancel the query.</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<string>> GetUsdNetAssetValue([FromQuery] string indexOrCompositionSymbol, 
            [FromQuery]DateTime? componentPricesAsOf = default,
            [FromQuery]DateTime? compositionAsOf = default,
            [FromQuery]decimal maxRandomVariation = 0,
            CancellationToken cancellationToken = default)
        {
            if(!indexOrCompositionSymbol.IsIndexSymbol() && !indexOrCompositionSymbol.IsCompositionSymbol())
                return new JsonResult($"{indexOrCompositionSymbol} is not a valid symbol.");

            var utcNow = _dateTimeProvider.UtcNow;
            componentPricesAsOf ??= utcNow;
            compositionAsOf ??= utcNow;
            var composition = indexOrCompositionSymbol.IsCompositionSymbol()
                ? await _indexProvider.GetCompositionFromSymbol(indexOrCompositionSymbol, cancellationToken)
                : indexOrCompositionSymbol.IsIndexSymbol()
                    ? await _indexProvider.GetCompositionAtDate(indexOrCompositionSymbol, compositionAsOf.Value, cancellationToken)
                    : default;

            if (composition == default)
                return new JsonResult($"failed to retrieve composition for index {indexOrCompositionSymbol}.");

            var currentValuation = await _navCalculator.GetIndexValuation(composition, componentPricesAsOf)
                .ConfigureAwait(false);

            return new JsonResult(currentValuation.NetAssetValue.AddRandomVariation(maxRandomVariation));
        }
    }
}
