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
using System.Text.Json;
using System.Threading.Tasks;

namespace Trakx.Data.Market.Common.Sources.CryptoCompare
{
    public class CoinDetailsProvider
    {
        public static IReadOnlyDictionary<string, CoinDetails> CoinDetailsBySymbol { get; } =
            ReadCoinDetailsFromResource().GetAwaiter().GetResult().Data;

        private static async Task<AllCoinsResponse> ReadCoinDetailsFromResource()
        {
            var assembly = typeof(CoinDetailsProvider).Assembly;
            await using var stream = assembly.GetManifestResourceStream(
                    $"{typeof(CoinDetailsProvider).Namespace}.coinDetails.json");
            var response = await JsonSerializer.DeserializeAsync<AllCoinsResponse>(stream);
            return response;
        }
    }
}