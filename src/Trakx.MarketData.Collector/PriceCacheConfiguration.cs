using System.ComponentModel.DataAnnotations;

namespace Trakx.MarketData.Collector
{
    public class PriceCacheConfiguration
    {
        [Required]
        public int CryptoCompareRestApiPollingPeriodMs { get; set; }

        [Required]
        public int RetryDbConnectionPeriodMs { get; set; }
    }
}
