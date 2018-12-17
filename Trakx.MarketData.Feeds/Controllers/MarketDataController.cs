using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CryptoCompare;

using JetBrains.Annotations;

using Microsoft.AspNetCore.Mvc;
using Trakx.MarketData.Feeds.Common.ApiClients;
using Trakx.MarketData.Feeds.Common.Cache;
using Trakx.MarketData.Feeds.Common.Helpers;
using Trakx.MarketData.Feeds.Common.Pricing;
using Trakx.MarketData.Feeds.Common.StaticData;
using Trakx.MarketData.Feeds.Common.Trackers;

namespace Trakx.MarketData.Feeds.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketDataController : ControllerBase
    {
        private readonly ICryptoCompareClient _cryptoCompareClient;

        private readonly ITrakxMemoryCache _memoryCache;
        private readonly ITrackerComponentProvider _componentProvider;
        private readonly IResponseBuilder _responseBuilder;

        public MarketDataController(ICryptoCompareClient cryptoCompareClient, 
                                    ITrakxMemoryCache memoryCache,
                                    ITrackerComponentProvider componentProvider,
                                    IResponseBuilder responseBuilder)
        {
            _cryptoCompareClient = cryptoCompareClient;
            _memoryCache = memoryCache;
            _componentProvider = componentProvider;
            _responseBuilder = responseBuilder;
        }

        [HttpGet(ApiConstants.CryptoCompare.AllCoins)]
        public ActionResult<CoinListResponse> GetAllCoins()
        {
            var trakxCoins = TrackerDetails.TrakxTrackersAsCoinList;
            return trakxCoins;
        }

        [HttpGet(ApiConstants.CryptoCompare.Price)]
        public async Task<ActionResult<PriceSingleResponse>> SingleSymbolPriceAsync(
            [FromQuery][NotNull] string fromSymbol,
            [FromQuery][NotNull] string toSymbolsAsCsvList,
            [FromQuery] bool? tryConversion = null,
            [FromQuery] string exchangeName = null)
        {
            Check.IsKnownNonNullOrWhiteSpaceKey(fromSymbol, TrackerDetails.TrakxTrackersAsCoinList.Coins, nameof(fromSymbol));
            var toSymbols = toSymbolsAsCsvList.FromCsvToDistinctNonNullOrWhiteSpaceUpperList();
            Check.NotEmpty(toSymbols, nameof(toSymbolsAsCsvList));

            var components = await _componentProvider.GetComponentTickers(fromSymbol);

            var componentsResponse = await _cryptoCompareClient.Prices.MultipleSymbolsPriceAsync(components, toSymbols, tryConversion, exchangeName);
            var response = _responseBuilder.CalculatePriceSingleResponse(fromSymbol, componentsResponse);

            return response;
        }

        [HttpGet(ApiConstants.CryptoCompare.PriceHistorical)]
        public async Task<ActionResult<PriceHistoricalReponse>> PriceHistoricalForTimestampAsync(
            [FromQuery] [NotNull] string fromSymbol,
            [FromQuery] [NotNull] string toSymbolsAsCsvList,
            [FromQuery] DateTimeOffset requestedDate,
            [FromQuery] string marketsAsCsvList = null,
            [FromQuery] CalculationType? calculationType = null,
            [FromQuery] bool? tryConversion = null)
        {
            Check.IsKnownNonNullOrWhiteSpaceKey(fromSymbol, TrackerDetails.TrakxTrackersAsCoinList.Coins, nameof(fromSymbol));
            
            var toSymbols = toSymbolsAsCsvList.FromCsvToDistinctNonNullOrWhiteSpaceUpperList();
            Check.NotEmpty(toSymbols, nameof(toSymbolsAsCsvList));

            var markets = marketsAsCsvList.FromCsvToDistinctNonNullOrWhiteSpaceUpperList();

            var components = await _componentProvider.GetComponentTickers(fromSymbol);

            var fetchComponentsResponses = components.Select(
                async (c,i) =>
                    {
                        var priceHistoricalReponse = await _cryptoCompareClient.History.HistoricalForTimestampAsync(
                                                         c,
                                                         toSymbols,
                                                         requestedDate,
                                                         markets,
                                                         calculationType,
                                                         tryConversion);
                        //current api key doesn't let us do too many queries per second :/
                        if (i != components.Count - 1) await Task.Delay(200);
                        return priceHistoricalReponse;
                    }).ToList();

            var componentsResponses = await Task.WhenAll(fetchComponentsResponses);

            var response = _responseBuilder.CalculatePriceHistoricalResponse(fromSymbol, componentsResponses.ToList());

            return response;
        }

        [HttpGet(ApiConstants.CryptoCompare.PriceMultifull)]

        public async Task<ActionResult<PriceMultiFullResponse>> PriceMultipleSymbolFullDataAsync(
            [FromQuery] [NotNull] string fromSymbolAsCsvList,
            [FromQuery] [NotNull] string toSymbolsAsCsvList,
            bool? tryConversion = null,
            string exchangeName = null)
        {
            var fromSymbols = fromSymbolAsCsvList.FromCsvToDistinctNonNullOrWhiteSpaceUpperList();
            Check.NotEmpty(fromSymbols, nameof(fromSymbolAsCsvList));
            var toSymbols = toSymbolsAsCsvList.FromCsvToDistinctNonNullOrWhiteSpaceUpperList();
            Check.NotEmpty(toSymbols, nameof(toSymbolsAsCsvList));

            var fetchSymbolsByTracker = fromSymbols.Select(async s => (s, await _componentProvider.GetComponentTickers(s)));
            var symbolsByTracker = (await Task.WhenAll(fetchSymbolsByTracker)).ToDictionary(r => r.Item1, r => r.Item2);
            var allSymbols = symbolsByTracker.Values.SelectMany(c => c).Distinct().ToList();

            var pricesMultiFullResponses = await _cryptoCompareClient.Prices.MultipleSymbolFullDataAsync(
                                                       allSymbols,
                                                       toSymbols,
                                                       tryConversion,
                                                       exchangeName);

            var response = _responseBuilder.CalculatePriceMultiFullResponse(symbolsByTracker, pricesMultiFullResponses);

            return response;
        }

        [HttpGet(ApiConstants.CryptoCompare.TopMarketCap)]
        public async Task<ActionResult<TopMarketCapResponse>> GetTopMarketCap(
            [FromQuery][NotNull] string toSymbol,
            [FromQuery] int? limit = null,
            [FromQuery] int? page = null,
            [FromQuery] bool? sign = null)
        {
            Check.NotNullOrWhiteSpace(toSymbol, nameof(toSymbol));
            var response = await _cryptoCompareClient.Tops.CoinFullDataByMarketCap(toSymbol, limit, page, sign);
            return new ActionResult<TopMarketCapResponse>(response);
        }

        /// <summary>
        /// Simply uses BTC top pairs and returns of fraction of the corresponding volumes
        /// </summary>
        [HttpGet(ApiConstants.CryptoCompare.TopPairs)]
        public async Task<ActionResult<TopResponse>> TradingPairsAsync(
            [NotNull] string fromSymbol,
            int? limit = null)
        {
            Check.IsKnownNonNullOrWhiteSpaceKey(
                fromSymbol,
                TrackerDetails.TrakxTrackersAsCoinList.Coins,
                nameof(fromSymbol));

            var components = await _componentProvider.GetComponentTickers(fromSymbol);
            var btcPairs = await _cryptoCompareClient.Tops.TradingPairsAsync("BTC", limit);

            var allToSymbols = btcPairs.Data.Select(d => d.ToSymbol).Distinct().ToList();
            var trackerPrices = await SingleSymbolPriceAsync(fromSymbol, string.Join(",", allToSymbols));
            
            var response = _responseBuilder.CalculateTopPairResponse(fromSymbol, trackerPrices.Value, btcPairs);

            return response;
        }

        [HttpGet(ApiConstants.CryptoCompare.TopTotalVol)]
        public async Task<ActionResult<TopVolume24HResponse>> CoinFullDataBy24HVolume([NotNull] string toSymbol, int? limit = null)
        {
            Check.NotNull(toSymbol, nameof(toSymbol));
            var trakxCoins = TrackerDetails.TrakxTrackersAsCoinList;
            var coinsSymbolsCsv = string.Join(",", trakxCoins.Coins.Select(c => c.Value.Symbol));

            var baseResponse = await _cryptoCompareClient.Tops.CoinFullDataBy24HVolume(toSymbol, trakxCoins.Coins.Count);

            var prices = await PriceMultipleSymbolFullDataAsync(coinsSymbolsCsv, toSymbol);

            var response = _responseBuilder.CalculateTopVolumesResponse(trakxCoins, prices.Value, baseResponse);

            return response;
        }

        [HttpGet(ApiConstants.CryptoCompare.TopVolumes)]
        public async Task<ActionResult<TopVolumesResponse>> TopTotalVol([NotNull] string toSymbol, int? limit = null)
        {
            return null;
        }
    }
}
