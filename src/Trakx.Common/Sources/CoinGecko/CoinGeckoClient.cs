using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CoinGecko.Entities.Response.Coins;
using CoinGecko.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using Trakx.Common.Pricing;
using Trakx.Common.Utils;

namespace Trakx.Common.Sources.CoinGecko
{
    public class CoinGeckoClient : ICoinGeckoClient
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<CoinGeckoClient> _logger;
        private readonly ICoinsClient _coinsClient;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly ISimpleClient _simpleClient;
        private Dictionary<string, string> _symbolsByNames;
        private Dictionary<string, string> _idsBySymbolName;

        public Dictionary<string, string> IdsBySymbolName
        {
            get
            {
                if (_idsBySymbolName != null) return _idsBySymbolName;
                var idsBySymbolName = _idsBySymbolName ??= CoinList
                    .ToDictionary(c => GetSymbolNameKey(c.Symbol, c.Name), c => c.Id);
                //idsBySymbolName["lend|aave"] = "aave";
                return idsBySymbolName;
            }
        }

        private readonly Dictionary<string, CoinFullDataById> _coinFullDataByIds;
        public Dictionary<string, CoinFullDataById> CoinFullDataByIds => _coinFullDataByIds;

        private static readonly Regex etherscanTokenAddress = new Regex(@"https://etherscan.io/token/(?<address>0x\w{40})");

        public CoinGeckoClient(ClientFactory factory, IMemoryCache memoryCache, ILogger<CoinGeckoClient> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
            _retryPolicy = Policy.Handle<Exception>()
                .WaitAndRetryAsync(3, c => TimeSpan.FromSeconds(c*c));
            _coinsClient = factory.CreateCoinsClient();
            _simpleClient = factory.CreateSimpleClient();
            _coinFullDataByIds = new Dictionary<string, CoinFullDataById>();
        }

        /// <inheritdoc />
        public async Task<decimal?> GetLatestPrice(string coinGeckoId, string quoteCurrency = Constants.DefaultQuoteCurrency)
        {
            var quoteCurrencyId = quoteCurrency.ToLower() == Constants.Usd 
                ? Constants.Usd 
                : await GetCoinGeckoIdFromSymbol(quoteCurrency);
            if (quoteCurrencyId == default) return 0;

            var tickerDetails = await _retryPolicy.ExecuteAsync(
                () => _simpleClient.GetSimplePrice(new []{ coinGeckoId, quoteCurrencyId }, new []{ Constants.Usd }))
                .ConfigureAwait(false);
            var price = tickerDetails[coinGeckoId][Constants.Usd];
            var conversionToQuoteCurrency = tickerDetails[quoteCurrencyId][Constants.Usd];
            return (decimal?)(price / conversionToQuoteCurrency) ?? 0m;
        }

        private async Task<string?> GetCoinGeckoIdFromSymbol(string symbol)
        {
            var coinList = await GetCoinList();

            var id = coinList.FirstOrDefault(c =>
                c.Symbol.Equals(symbol, StringComparison.InvariantCultureIgnoreCase))?.Id;
            return id;
        }

        /// <inheritdoc />
        public async Task<decimal?> GetPriceAsOfFromId(string id, DateTime asOf, string quoteCurrencyId = "usd-coin")
        {
            try
            {
                var date = asOf.ToString("dd-MM-yyyy");

                var fxRate = await GetUsdFxRate(quoteCurrencyId, date);

                var historicalPrice = await _retryPolicy.ExecuteAsync(() =>
                        _coinsClient.GetHistoryByCoinId(id, date, false.ToString()));

                return (decimal?)historicalPrice.MarketData.CurrentPrice[Constants.Usd] / fxRate;
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Failed to retrieve price for {0} as of {1:yyyyMMdd}", id, asOf);
                return null;
            }
        }

        private async Task<decimal> GetUsdFxRate(string quoteCurrencyId, string date)
        {
            var conversion = 1m;
            if (quoteCurrencyId != default)
            {
                var quoteResponse = await _memoryCache.GetOrCreateAsync($"{date}|{quoteCurrencyId}",
                    async entry => await _retryPolicy.ExecuteAsync(() =>
                        _coinsClient.GetHistoryByCoinId(quoteCurrencyId, date, false.ToString())));
                conversion = (decimal?) quoteResponse.MarketData.CurrentPrice[Constants.Usd] ?? 1m;
            }

            return conversion;
        }

        /// <inheritdoc />
        public async Task<MarketData> GetMarketDataAsOfFromId(string id, DateTime asOf, string quoteCurrencyId = "usd-coin")
        {
            var date = asOf.ToString("dd-MM-yyyy");
            var fullData = await _coinsClient.GetHistoryByCoinId(id, date, false.ToString())
                .ConfigureAwait(false);
            var fxRate = await GetUsdFxRate(quoteCurrencyId, date);
            var marketData = new MarketData()
            {
                AsOf = asOf,
                CoinId = fullData.Id,
                MarketCap = (decimal?) fullData.MarketData?.MarketCap[Constants.Usd] / fxRate,
                Volume = (decimal?) fullData.MarketData?.TotalVolume[Constants.Usd] / fxRate,
                Price = (decimal?)fullData.MarketData?.CurrentPrice[Constants.Usd] / fxRate,
                QuoteCurrency = fullData.Symbol
                
            };
            
            return marketData;
        }

        public bool TryRetrieveSymbol(string coinName, out string? symbol)
        {
            _symbolsByNames ??= CoinList.ToDictionary(c => c.Name, c => c.Symbol);
            var bestMatch = coinName.FindBestLevenshteinMatch(_symbolsByNames.Keys);
            symbol = bestMatch != null ? _symbolsByNames[bestMatch] : null;

            return bestMatch != null;
        }

        private string GetSymbolNameKey(string symbol, string name)
        {
            return $"{symbol.ToLower()}|{name.ToLower()}";
        }

        public string RetrieveCoinGeckoId(string symbol, string name)
        {
            var symbolNameKey = GetSymbolNameKey(symbol, name);
            if (IdsBySymbolName.TryGetValue(symbolNameKey, out var coinGeckoId))
                return coinGeckoId;

            var bestMatch = symbolNameKey.FindBestLevenshteinMatch(IdsBySymbolName.Keys);
            coinGeckoId = IdsBySymbolName[bestMatch];
            IdsBySymbolName.Add(symbolNameKey, coinGeckoId);
            return coinGeckoId;
        }

        public CoinFullDataById RetrieveCoinFullData(string symbol, string name)
        {
            var coinId = RetrieveCoinGeckoId(symbol, name);
            if (CoinFullDataByIds.TryGetValue(coinId, out var data)) return data;

            try
            {
                data = _coinsClient.GetAllCoinDataWithId(coinId, "false",
                false, false, false, false, false)
                    .GetAwaiter().GetResult();

                CoinFullDataByIds[coinId] = data;
                return data;
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, "Failed to retrieve coin data for {0}|{1}", symbol, name);
                return default;
            }
        }

        public void RetrieveContractDetailsFromCoinSymbolName(string searchSymbol, string searchName, 
            out string? coinGeckoId, out string? symbol, out string? contractAddress)
        {
            var data = RetrieveCoinFullData(searchSymbol, searchName);
            coinGeckoId = data?.Id;
            symbol = data?.Symbol;
            var etherscanLink = data?.Links?.BlockchainSite?
                .FirstOrDefault(b => b != null && etherscanTokenAddress.IsMatch(b));
            contractAddress = etherscanLink != null
                ? etherscanTokenAddress.Match(etherscanLink).Groups["address"].Value
                : default;
        }
        
        public async Task<IReadOnlyList<CoinList>> GetCoinList()
        {
            var coinList = await _memoryCache.GetOrCreateAsync("CoinGecko.CoinList", async entry =>
                await _retryPolicy.ExecuteAsync(() => _coinsClient.GetCoinList()).ConfigureAwait(false));
            return coinList;
        }

        public IReadOnlyList<CoinList> CoinList  => _memoryCache.GetOrCreate("CoinGecko.CoinList", 
            entry => _retryPolicy.ExecuteAsync(() => _coinsClient.GetCoinList())
                .ConfigureAwait(false).GetAwaiter().GetResult());
    }
}