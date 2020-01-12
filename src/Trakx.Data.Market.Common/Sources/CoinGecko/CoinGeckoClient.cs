#region LICENSE

// 
// Copyright (c) 2019 Catalyst Network
// 
// This file is part of Catalyst.Node <https://github.com/catalyst-network/Catalyst.Node>
// 
// Catalyst.Node is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Catalyst.Node is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Catalyst.Node. If not, see <https://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoinGecko.Entities.Response.Coins;
using CoinGecko.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Retry;
using Trakx.Data.Market.Common.Utils;

namespace Trakx.Data.Market.Common.Sources.CoinGecko
{
    public class CoinGeckoClient : ICoinGeckoClient
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ICoinsClient _coinsClient;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly ISimpleClient _simpleClient;

        public CoinGeckoClient(ClientFactory factory, IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _retryPolicy = Policy.Handle<Exception>()
                .WaitAndRetryAsync(3, c => TimeSpan.FromSeconds(c*c));
            _coinsClient = factory.CreateCoinsClient();
            _simpleClient = factory.CreateSimpleClient();
        }

        public async Task<decimal> GetLatestUsdPrice(string symbol)
        {
            var coinList = await GetCoinList();

            var id = coinList.FirstOrDefault(c =>
                c.Symbol.Equals(symbol, StringComparison.InvariantCultureIgnoreCase))?.Id;
            if (id == null) return 0;

            var tickerDetails = await _retryPolicy.ExecuteAsync(
                () => _simpleClient.GetSimplePrice(new []{id}, new []{"usd"})).ConfigureAwait(false);
            var price = tickerDetails[id]["usd"];
            return (decimal?)price ?? 0m;
        }

        public bool TryRetrieveSymbol(string coinName, out string? symbol)
        {
            var coinList = GetCoinList().ConfigureAwait(false).GetAwaiter().GetResult();
            var symbolsByNames = coinList.ToDictionary(c => c.Name, c => c.Symbol);
            var bestMatch = coinName.FindBestLevenshteinMatch(symbolsByNames.Keys);
            symbol = bestMatch != null ? symbolsByNames[bestMatch] : null;

            return bestMatch != null;
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