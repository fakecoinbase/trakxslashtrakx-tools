using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Trakx.Data.Market.Common.Indexes
{
    public class IndexDetailsProvider : IIndexDetailsProvider
    {
        public IDictionary<KnownIndexes, IndexDetails> IndexDetails { get; } = Enum.GetNames(typeof(KnownIndexes))
            .ToDictionary(Enum.Parse<KnownIndexes>,
                n => ReadIndexDetailsFromResource(n).ConfigureAwait(false).GetAwaiter().GetResult());

        private static async Task<IndexDetails> ReadIndexDetailsFromResource(string symbol)
        {
            try
            {
                var assembly = typeof(IndexDetailsProvider).Assembly;
                await using var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Indexes.{symbol}.json");
                var response = await JsonSerializer.DeserializeAsync<IndexDetails>(stream)
                    .ConfigureAwait(false);
                return response;
            }
            catch (Exception exception)
            {
                throw new InvalidDataException($"Unable to find definition for index {symbol}", exception);
            }
        }
    }
}
