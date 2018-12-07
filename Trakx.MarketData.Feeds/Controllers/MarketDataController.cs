using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CryptoCompare;

using Microsoft.AspNetCore.Mvc;

using Trakx.MarketData.Feeds.Common;
using Trakx.MarketData.Feeds.Common.ApiClients;
using Trakx.MarketData.Feeds.Common.Models.CryptoCompare;
using Trakx.MarketData.Feeds.Common.Models.Trakx;

namespace Trakx.MarketData.Feeds.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketDataController : ControllerBase
    {
        private readonly ICryptoCompareClient _cryptoCompareClient;

        public MarketDataController(ICryptoCompareClient cryptoCompareClient)
        {
            _cryptoCompareClient = cryptoCompareClient;
        }

        [HttpGet]
        public async Task<ActionResult<TopMarketCapResponse>> GetTop20Tickers()
        {
            var top20 = await _cryptoCompareClient.Tops.CoinFullDataByMarketCap("USD", 20);
            return new ActionResult<TopMarketCapResponse>(top20);
        }

        [HttpGet(ApiConstants.CryptoCompare.AllCoins)]

        public async Task<ActionResult<ICryptoCompareResponse>> GetAllCoins()
        {
            var exampleResponse = await _cryptoCompareClient.Coins.ListAsync();
            var response = new CoinListResponse();
            return null;
        }

        [HttpGet(ApiConstants.CryptoCompare.Price)]

        public async Task<ActionResult<ICryptoCompareResponse>> GetPrice()
        {
            return null;
        }

        [HttpGet(ApiConstants.CryptoCompare.PriceHistorical)]

        public async Task<ActionResult<ICryptoCompareResponse>> GetPriceHistorical()
        {
            return null;
        }

        [HttpGet(ApiConstants.CryptoCompare.PriceMultifull)]

        public async Task<ActionResult<ICryptoCompareResponse>> GetPriceMultifull()
        {
            return null;
        }

        [HttpGet(ApiConstants.CryptoCompare.TopMarketCap)]

        public async Task<ActionResult<ICryptoCompareResponse>> GetTopMarketCap()
        {
            return null;
        }

        [HttpGet(ApiConstants.CryptoCompare.TopPair)]

        public async Task<ActionResult<ICryptoCompareResponse>> GetTopPair()
        {
            return null;
        }

        [HttpGet(ApiConstants.CryptoCompare.TopTotalVol)]

        public async Task<ActionResult<ICryptoCompareResponse>> GetTopTotalVol()
        {
            return null;
        }

        [HttpGet(ApiConstants.CryptoCompare.TopVolumes)]

        public async Task<ActionResult<ICryptoCompareResponse>> GetTopVolumes()
        {
            return null;
        }
    }
}
