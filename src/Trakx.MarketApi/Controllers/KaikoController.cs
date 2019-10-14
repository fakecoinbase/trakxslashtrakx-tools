using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Trakx.MarketApi.DataSources.Kaiko.Client;

namespace Trakx.MarketApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class KaikoController : ControllerBase
    {
        private readonly ILogger<KaikoController> _logger;
        private readonly KaikoApiClient _kaikoApiClient;

        public KaikoController(KaikoApiClient kaikoApiClient, ILogger<KaikoController> logger)
        {
            _logger = logger;
            _kaikoApiClient = kaikoApiClient;
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
