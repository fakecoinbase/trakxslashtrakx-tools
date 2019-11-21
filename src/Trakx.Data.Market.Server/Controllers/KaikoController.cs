using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Trakx.Data.Market.Common.Sources.Kaiko.Client;
using Trakx.Data.Market.Common.Sources.Kaiko.DTOs;

namespace Trakx.Data.Market.Server.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class KaikoController : ControllerBase
    {
        private readonly ILogger<KaikoController> _logger;
        private readonly IKaikoClient _kaikoClient;

        public KaikoController(IKaikoClient kaikoClient, ILogger<KaikoController> logger)
        {
            _logger = logger;
            _kaikoClient = kaikoClient;
        }

        [HttpGet]
        public async Task<ActionResult<string>> Assets()
        {
            var instruments = await _kaikoClient.GetAssets().ConfigureAwait(false);
            return JsonConvert.SerializeObject(instruments);
        }

        [HttpGet]
        public async Task<ActionResult<string>> Exchanges()
        {
            var instruments = await _kaikoClient.GetExchanges().ConfigureAwait(false);
            return JsonConvert.SerializeObject(instruments);
        }

        [HttpGet]
        public async Task<ActionResult<string>> Instruments()
        {
            var instruments = await _kaikoClient.GetInstruments().ConfigureAwait(false);
            return JsonConvert.SerializeObject(instruments);
        }

        [HttpGet]
        public async Task<ActionResult<string>> SpotExchangeRate(AggregatedPriceRequest request)
        {
            var prices = _kaikoClient.GetSpotExchangeRate(request);
            return JsonConvert.SerializeObject(prices);
        }
    }
}
