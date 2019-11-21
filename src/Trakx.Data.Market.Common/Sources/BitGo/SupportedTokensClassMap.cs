using CsvHelper.Configuration;

namespace Trakx.Data.Market.Common.Sources.BitGo
{
    public sealed class SupportedTokensClassMap : ClassMap<SupportedToken>
    {
        public SupportedTokensClassMap()
        {
            Map(m => m.Identifier).Name("Identifier");
            Map(m => m.DigitalCurrency).Name("Digital Currency");
            Map(m => m.Family).Name("Family");
            Map(m => m.BitGoEnvironment).Name("BitGo Environment");
            Map(m => m.ReleaseStatus).Name("Release status");
        }
    }

}
