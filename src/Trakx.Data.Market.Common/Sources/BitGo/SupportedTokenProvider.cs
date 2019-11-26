using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Trakx.Data.Market.Common.Sources.BitGo
{
    public class SupportedTokenProvider
    {
        public ImmutableDictionary<string, SupportedToken> SupportedTokensBySymbol { get; }

        public SupportedTokenProvider()
        {
            var assembly = typeof(SupportedTokenProvider).Assembly;

            SupportedTokensBySymbol = ReadSupportedTokensFile()
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult()
                .Where(s => s?.Identifier != null)
                .ToImmutableDictionary(
                    s => s.Identifier ?? throw new NullReferenceException("Identifier cannot be null'"), 
                    s => s);

        }

        private static async Task<IReadOnlyCollection<SupportedToken>> ReadSupportedTokensFile()
        {
            var assembly = typeof(SupportedTokenProvider).Assembly;
            await using var fileStream = assembly.GetManifestResourceStream(
                $"{typeof(SupportedTokenProvider).Namespace}.supported.tokens.csv");
            var csvReader = new CsvHelper.CsvReader(new StreamReader(fileStream));
            csvReader.Configuration.RegisterClassMap<SupportedTokensClassMap>();
            var tokens = csvReader.GetRecords<SupportedToken>().ToList();
            return tokens.AsReadOnly();
        }
    }
}
