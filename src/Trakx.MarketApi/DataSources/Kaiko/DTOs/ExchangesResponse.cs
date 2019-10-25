using System.Collections.Generic;

namespace Trakx.MarketApi.DataSources.Kaiko.DTOs
{
    public partial class ExchangesResponse   
    {
        public string Result { get; set; }
        public List<Exchange> Data { get; set; }
    }

    public partial class Exchange
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string KaikoLegacySlug { get; set; }
    }

}
