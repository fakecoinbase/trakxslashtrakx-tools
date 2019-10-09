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

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Trakx.MarketApi.DataSources.CryptoCompare
{
    public class StaticCoinDetails
    {
        public static IReadOnlyDictionary<string, CoinDetails> CoinDetailsBySymbol { get; } =
            ReadCoinDetailsFromResource().GetAwaiter().GetResult().Data;

        private static async Task<AllCoinsResponse> ReadCoinDetailsFromResource()
        {
            var assembly = typeof(StaticCoinDetails).Assembly;
            using (var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.DataSources.CryptoCompare.CoinList.json"))
            using (var reader = new StreamReader(stream))
            {
                var jsonResult = await reader.ReadToEndAsync();
                var response = JsonConvert.DeserializeObject<AllCoinsResponse>(jsonResult);
                return response;
            }
        }
    }
}