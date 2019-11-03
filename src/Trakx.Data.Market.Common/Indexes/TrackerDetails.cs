using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Trakx.Data.Market.Common.Indexes
{
    public class TrackerDetails
    {
        public static Dictionary<KnownIndexes, IndexDetails> IndexDetails { get; } = Enum.GetNames(typeof(KnownIndexes))
            .ToDictionary(Enum.Parse<KnownIndexes>,
                n => ReadIndexDetailsFromResource(n).ConfigureAwait(false).GetAwaiter().GetResult());

        private static async Task<IndexDetails> ReadIndexDetailsFromResource(string symbol)
        {
            var assembly = typeof(TrackerDetails).Assembly;
            await using var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Indexes.{symbol}.json");
            var response = await JsonSerializer.DeserializeAsync<IndexDetails>(stream)
                .ConfigureAwait(false);
            return response;
        }
    }
}
