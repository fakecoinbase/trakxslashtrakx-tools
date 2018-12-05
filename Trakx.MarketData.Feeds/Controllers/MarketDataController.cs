using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Trakx.MarketData.Feeds.Common;
using Trakx.MarketData.Feeds.Common.ApiClients;
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

        [HttpGet(ApiConstants.CryptoCompare.AllCoins)]

        public async Task<ActionResult<ICryptoCompareResponse<ICoin>>> GetAllCoins()
        {
            return null;
        }

        [HttpGet(ApiConstants.CryptoCompare.Price)]

        public async Task<ActionResult<ICryptoCompareResponse<ICoin>>> GetPrice()
        {
            return null;
        }

        [HttpGet(ApiConstants.CryptoCompare.PriceHistorical)]

        public async Task<ActionResult<ICryptoCompareResponse<ICoin>>> GetPriceHistorical()
        {
            return null;
        }

        [HttpGet(ApiConstants.CryptoCompare.PriceMultifull)]

        public async Task<ActionResult<ICryptoCompareResponse<ICoin>>> GetPriceMultifull()
        {
            return null;
        }

        [HttpGet(ApiConstants.CryptoCompare.TopMarketCap)]

        public async Task<ActionResult<ICryptoCompareResponse<ICoin>>> GetTopMarketCap()
        {
            return null;
        }

        [HttpGet(ApiConstants.CryptoCompare.TopPair)]

        public async Task<ActionResult<ICryptoCompareResponse<ICoin>>> GetTopPair()
        {
            return null;
        }

        [HttpGet(ApiConstants.CryptoCompare.TopTotalVol)]

        public async Task<ActionResult<ICryptoCompareResponse<ICoin>>> GetTopTotalVol()
        {
            return null;
        }

        [HttpGet(ApiConstants.CryptoCompare.TopVolumes)]

        public async Task<ActionResult<ICryptoCompareResponse<ICoin>>> GetTopVolumes()
        {
            return null;
        }
    }
}
