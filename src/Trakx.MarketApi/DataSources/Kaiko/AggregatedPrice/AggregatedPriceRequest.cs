using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trakx.MarketApi.DataSources.Kaiko.Client;

namespace Trakx.MarketApi.DataSources.Kaiko.AggregatedPrice
{
    public class AggregatedPriceRequest
    {
        public class QueryParameters
        {
            public string DataVersion { get; set; }
            public string Commodity { get; set; }
            public DateTimeOffset EndTime { get; set; }
            public DateTimeOffset StartTime { get; set; }
            public long PageSize { get; set; }
            public string Interval { get; set; }
            public string QuoteAsset { get; set; }
            public string BaseAsset { get; set; }
            public List<string> Exchanges { get; set; }
            public bool Sources { get; set; }
            public List<string> Instruments { get; set; }
        }

        private readonly KaikoApiClientFactory _clientFactory;

        public AggregatedPriceRequest(KaikoApiClientFactory clientFactory)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        }

        public async Task<IReadOnlyCollection<AggregatedPrice>> Execute(QueryParameters query)
        {
            var marketDataClient = _clientFactory.Create();
            var response = await marketDataClient.GetAggregatedPrice(query).ConfigureAwait(false);
            if(response?.Result != Constants.SuccessResponse || response?.Data == null) return new AggregatedPrice[0];
            return response.Data;
        }
    }
}
