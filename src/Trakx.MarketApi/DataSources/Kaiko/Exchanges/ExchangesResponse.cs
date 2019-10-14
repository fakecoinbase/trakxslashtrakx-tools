using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Trakx.MarketApi.DataSources.Kaiko
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
