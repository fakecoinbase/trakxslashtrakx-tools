namespace Trakx.MarketApi.Indexes
{
    public partial class IndexDetails
    {
        public string Name { get; set; }
        public string Symbol { get; set; }
        public Component[] Components { get; set; }
        public string[] ComponentAddresses { get; set; }
        public double TargetUsdPrice { get; set; }
        public UsdBidAsk UsdBidAsk { get; set; }
        public string Description { get; set; }
    }

    public partial class Component
    {
        public string Address { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public long Decimals { get; set; }
        public UsdBidAsk UsdBidAsk { get; set; }
        public string Proportion { get; set; }
    }

    public partial class UsdBidAsk
    {
        public string Bid { get; set; }
        public string Ask { get; set; }
    }
}
