using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Trakx.MarketApi.Indexes
{
    public class TrackerDetails
    {
        public static Dictionary<KnownIndexes, IndexDetails> IndexDetails { get; } = Enum.GetNames(typeof(KnownIndexes))
            .ToDictionary(Enum.Parse<KnownIndexes>,
                n => ReadIndexDetailsFromResource(n).ConfigureAwait(false).GetAwaiter().GetResult());

        private static async Task<IndexDetails> ReadIndexDetailsFromResource(string symbol)
        {
            var assembly = typeof(TrackerDetails).Assembly;
            using (var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Indexes.{symbol}.json"))
            using (var reader = new StreamReader(stream))
            {
                var jsonResult = await reader.ReadToEndAsync();
                var response = JsonConvert.DeserializeObject<IndexDetails>(jsonResult);
                return response;
            }
        }
    }
}
