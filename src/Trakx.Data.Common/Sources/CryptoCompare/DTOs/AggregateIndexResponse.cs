namespace Trakx.Data.Common.Sources.CryptoCompare.DTOs
{
    public class WebSocketResponseMessage
    { 
        public ushort Type { get; set; }
    }

    public class AggregateIndexResponse : WebSocketResponseMessage
    {
        public string Market { get; set; }
        public string FromSymbol { get; set; }
        public string ToSymbol { get; set; }
        public long Flags { get; set; }
        public decimal Price { get; set; }
        public long LastUpdate { get; set; }
        public decimal Median { get; set; }
        public decimal LastVolume { get; set; }
        public decimal LastVolumeTo { get; set; }
        public long LastTradeId { get; set; }
        public decimal VolumeDay { get; set; }
        public decimal VolumeDayTo { get; set; }
        public decimal Volume24Hour { get; set; }
        public decimal Volume24HourTo { get; set; }
        public decimal OpenDay { get; set; }
        public decimal HighDay { get; set; }
        public decimal LowDay { get; set; }
        public decimal Open24Hour { get; set; }
        public decimal High24Hour { get; set; }
        public decimal Low24Hour { get; set; }
        public string LastMarket { get; set; }
        public decimal VolumeHour { get; set; }
        public decimal VolumeHourTo { get; set; }
        public decimal OpenHour { get; set; }
        public decimal HighHour { get; set; }
        public decimal LowHour { get; set; }
        public decimal TopTierVolume24Hour { get; set; }
        public decimal TopTierVolume24HourTo { get; set; }
    }
}