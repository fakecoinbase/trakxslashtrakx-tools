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