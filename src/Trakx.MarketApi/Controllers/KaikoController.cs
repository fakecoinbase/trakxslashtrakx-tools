using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CryptoCompare;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Trakx.MarketApi.DataSources.Kaiko;
using Trakx.MarketApi.Indexes;

namespace Trakx.MarketApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class KaikoController : ControllerBase
    {
        private readonly ILogger<KaikoController> _logger;
        private readonly KaikoApiClient _kaikoApiClient;

        public KaikoController(ILogger<KaikoController> logger)
        {
            _logger = logger;
            _kaikoApiClient = new KaikoApiClient();
        }

        [HttpGet]
        public async Task<ActionResult<string>> Assets()
        {
            var instruments = await _kaikoApiClient.GetAssets().ConfigureAwait(false);
            return JsonConvert.SerializeObject(instruments);
        }

        [HttpGet]
        public async Task<ActionResult<string>> Exchanges()
        {
            var instruments = await _kaikoApiClient.GetExchanges().ConfigureAwait(false);
            return JsonConvert.SerializeObject(instruments);
        }

        [HttpGet]
        public async Task<ActionResult<string>> Instruments()
        {
            var instruments = await _kaikoApiClient.GetInstruments().ConfigureAwait(false);
            return JsonConvert.SerializeObject(instruments);
        }
    }
}
