using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Trakx.MarketData.Feeds.Common;
using Trakx.MarketData.Feeds.Common.Models.CoinMarketCap;
using Trakx.MarketData.Feeds.Common.Models.CryptoCompare;
using Trakx.MarketData.Feeds.Common.Models.Trakx;

namespace Trakx.MarketData.Feeds.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketDataController : ControllerBase
    {
        private readonly ITrackerComponentProvider _trackerComponentProvider;

        public MarketDataController(ITrackerComponentProvider trackerComponentProvider)
        {
            _trackerComponentProvider = trackerComponentProvider;
        }

        [HttpGet]
        public async Task<ActionResult<IList<ICryptoCompareCoinAndMarketCap>>> GetTop20Tickers()
        {
            var top20 = await _trackerComponentProvider.GetTopXMarketCapCoins(20);
            var result = new ActionResult<IList<ICryptoCompareCoinAndMarketCap>>(top20);
            return result;
        }

        
    }
}
