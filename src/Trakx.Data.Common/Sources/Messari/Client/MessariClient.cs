using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Trakx.Data.Common.Sources.Messari.DTOs;

namespace Trakx.Data.Common.Sources.Messari.Client
{
    public class MessariClient : IMessariClient
    {
        private readonly RequestHelperFactory _clientFactory;

        public MessariClient(RequestHelperFactory clientFactory)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        }

        public IAsyncEnumerable<Asset> GetAllAssetsAsync(CancellationToken cancellationToken = default)
        {
            var apiClient = _clientFactory.Create();
            var response = apiClient.GetAllAssets(cancellationToken);
            return response;
        }

        public async Task<decimal?> GetLatestPrice(string symbol,
            string quoteCurrency = Pricing.Constants.DefaultQuoteCurrency)
        {
            Guard.Against.NullOrWhiteSpace(symbol, nameof(quoteCurrency));
            var results = await GetLatestPrices(new[] {symbol}, quoteCurrency);
            return results.Values.SingleOrDefault();
        }

        public async Task<Dictionary<string, decimal?>> GetLatestPrices(IEnumerable<string> symbols, 
            string quoteCurrency = Pricing.Constants.DefaultQuoteCurrency)
        {
            Guard.Against.Default(symbols, nameof(symbols));
            Guard.Against.NullOrWhiteSpace(quoteCurrency, nameof(quoteCurrency));
            var symbolList = symbols as List<string> ?? symbols.ToList();

            var apiClient = _clientFactory.Create();
            var getPriceTasks = symbolList.Union(new[] {quoteCurrency}, StringComparer.InvariantCultureIgnoreCase)
                .Select(async s => await apiClient.GetMarketDataForSymbol(s).ConfigureAwait(false))
                .ToArray();
            var allResponses = await Task.WhenAll(getPriceTasks).ConfigureAwait(false);

            var quoteCurrencyPrice = allResponses.FirstOrDefault(r =>
                r.Data != null 
                && r.Data.Symbol.Equals(quoteCurrency, StringComparison.InvariantCultureIgnoreCase))
                .Data.MarketData.PriceUsd;

            var result = allResponses.Where(r => r?.Data?.MarketData != null)
                .ToDictionary(
                    r => r.Data.Symbol.ToLower(),
                    r => r.Data.MarketData.PriceUsd / quoteCurrencyPrice);

            var returnQuoteCurrency =
                symbolList.Contains(quoteCurrency, StringComparer.InvariantCultureIgnoreCase);
            if (returnQuoteCurrency) return result;

            result.Remove(quoteCurrency.ToLower());
            return result;
        }

        public async Task<List<Asset>> GetAllAssets(CancellationToken cancellationToken = default)
        {
            var result = new List<Asset>();
            var apiClient = _clientFactory.Create();
            var assets = apiClient.GetAllAssets(cancellationToken);
            await foreach (var asset in assets.ConfigureAwait(false))
            {
                result.Add(asset);
            }

            return result;
        }


        public async Task<AssetMetrics> GetMetricsForSymbol(string symbol)
        {
            var apiClient = _clientFactory.Create();
            var response = await apiClient.GetMetricsForSymbol(symbol).ConfigureAwait(false);
            return response?.Data ?? new AssetMetrics();
        }

        public async Task<AssetProfile> GetProfileForSymbol(string symbol)
        {
            var apiClient = _clientFactory.Create();
            var response = await apiClient.GetProfileForSymbol(symbol).ConfigureAwait(false);
            return response?.Data ?? new AssetProfile();
        }

        public List<string> SelectedSectors { get; } = new List<string>(){
            "Application Development", "Asset Management", "Centralized Exchanges", 
            "Decentralized Exchanges", "Interoperability", "IoT", "Lending", 
            "Smart Contract Platforms"
        };
    }
}
