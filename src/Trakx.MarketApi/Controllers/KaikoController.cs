﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Trakx.MarketApi.DataSources.Kaiko.Client;
using Trakx.MarketApi.DataSources.Kaiko.DTOs;

namespace Trakx.MarketApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class KaikoController : ControllerBase
    {
        private readonly ILogger<KaikoController> _logger;
        private readonly RequestHelper _requestHelper;

        public KaikoController(RequestHelper requestHelper, ILogger<KaikoController> logger)
        {
            _logger = logger;
            _requestHelper = requestHelper;
        }

        [HttpGet]
        public async Task<ActionResult<string>> Assets()
        {
            var instruments = await _requestHelper.GetAssets().ConfigureAwait(false);
            return JsonConvert.SerializeObject(instruments);
        }

        [HttpGet]
        public async Task<ActionResult<string>> Exchanges()
        {
            var instruments = await _requestHelper.GetExchanges().ConfigureAwait(false);
            return JsonConvert.SerializeObject(instruments);
        }

        [HttpGet]
        public async Task<ActionResult<string>> Instruments()
        {
            var instruments = await _requestHelper.GetInstruments().ConfigureAwait(false);
            return JsonConvert.SerializeObject(instruments);
        }

        [HttpGet]
        public async Task<ActionResult<string>> AggregatedPrices(AggregatedPriceRequest request)
        {
            var prices = _requestHelper.GetAggregatedPrices(request);
            return JsonConvert.SerializeObject(prices);
        }
    }
}