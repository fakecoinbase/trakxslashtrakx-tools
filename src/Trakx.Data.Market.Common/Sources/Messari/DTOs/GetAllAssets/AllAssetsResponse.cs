using System;
using System.Collections.Generic;

namespace Trakx.Data.Market.Common.Sources.Messari.DTOs.GetAllAssets
{
    public partial class AllAssetsResponse
    {
        public Status Status { get; set; }
        public List<Asset> Data { get; set; }
    }

    public partial class Asset
    {
        public Guid Id { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public Metrics Metrics { get; set; }
        public Profile Profile { get; set; }
    }

    public partial class Metrics
    {
        public MarketData MarketData { get; set; }
        public Marketcap Marketcap { get; set; }
        public Supply Supply { get; set; }
        public Dictionary<string, double?> BlockchainStats24_Hours { get; set; }
        public AllTimeHigh AllTimeHigh { get; set; }
        public CycleLow CycleLow { get; set; }
        public TokenSaleStats TokenSaleStats { get; set; }
        public StakingStats StakingStats { get; set; }
        public MiningStats MiningStats { get; set; }
        public DeveloperActivity DeveloperActivity { get; set; }
        public Dictionary<string, double?> RoiData { get; set; }
        public Dictionary<string, double?> RoiByYear { get; set; }
        public RiskMetrics RiskMetrics { get; set; }
        public MiscData MiscData { get; set; }
    }

    public partial class AllTimeHigh
    {
        public double? Price { get; set; }
        public DateTimeOffset? At { get; set; }
        public long? DaysSince { get; set; }
        public double? PercentDown { get; set; }
        public double? BreakevenMultiple { get; set; }
    }

    public partial class CycleLow
    {
        public double? Price { get; set; }
        public DateTimeOffset? At { get; set; }
        public double? PercentUp { get; set; }
        public long? DaysSince { get; set; }
    }

    public partial class DeveloperActivity
    {
        public long? Stars { get; set; }
        public long? Watchers { get; set; }
        public long? CommitsLast3_Months { get; set; }
        public long? CommitsLast1_Year { get; set; }
        public long? LinesAddedLast3_Months { get; set; }
        public long? LinesAddedLast1_Year { get; set; }
        public long? LinesDeletedLast3_Months { get; set; }
        public long? LinesDeletedLast1_Year { get; set; }
    }

    public partial class MarketData
    {
        public double PriceUsd { get; set; }
        public double PriceBtc { get; set; }
        public double VolumeLast24_Hours { get; set; }
        public double? RealVolumeLast24_Hours { get; set; }
        public double? VolumeLast24_HoursOverstatementMultiple { get; set; }
        public double PercentChangeUsdLast24_Hours { get; set; }
        public double PercentChangeBtcLast24_Hours { get; set; }
        public OhlcvLastHour OhlcvLast1_Hour { get; set; }
        public OhlcvLastHour OhlcvLast24_Hour { get; set; }
        public DateTimeOffset LastTradeAt { get; set; }
    }

    public partial class OhlcvLastHour
    {
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public double Volume { get; set; }
    }

    public partial class Marketcap
    {
        public double CurrentMarketcapUsd { get; set; }
        public double? Y2050_MarketcapUsd { get; set; }
        public double? YPlus10MarketcapUsd { get; set; }
        public double? LiquidMarketcapUsd { get; set; }
        public double? VolumeTurnoverLast24_HoursPercent { get; set; }
    }

    public partial class MiningStats
    {
        public string MiningAlgo { get; set; }
        public string NetworkHashRate { get; set; }
        public double? AvailableOnNicehashPercent { get; set; }
        public double? The1_HourAttackCost { get; set; }
        public double? The24_HoursAttackCost { get; set; }
        public double? AttackAppeal { get; set; }
    }

    public partial class MiscData
    {
        public object PrivateMarketPriceUsd { get; set; }
        public double? VladimirClubCost { get; set; }
        public double? BtcCurrentNormalizedSupplyPriceUsd { get; set; }
        public double? BtcY2050NormalizedSupplyPriceUsd { get; set; }
        public DateTimeOffset? AssetCreatedAt { get; set; }
        public long? AssetAgeDays { get; set; }
        public List<string> Categories { get; set; }
        public List<string> Sectors { get; set; }
    }

    public partial class RiskMetrics
    {
        public SharpeRatios SharpeRatios { get; set; }
        public VolatilityStats VolatilityStats { get; set; }
    }

    public partial class SharpeRatios
    {
        public double? Last30_Days { get; set; }
        public double? Last90_Days { get; set; }
        public double? Last1_Year { get; set; }
        public double? Last3_Years { get; set; }
    }

    public partial class VolatilityStats
    {
        public double? VolatilityLast30_Days { get; set; }
        public double? VolatilityLast90_Days { get; set; }
        public double? VolatilityLast1_Year { get; set; }
        public double? VolatilityLast3_Years { get; set; }
    }

    public partial class StakingStats
    {
        public double? StakingYieldPercent { get; set; }
        public string StakingType { get; set; }
        public long? StakingMinimum { get; set; }
        public double? TokensStaked { get; set; }
        public double? TokensStakedPercent { get; set; }
        public double? RealStakingYieldPercent { get; set; }
    }

    public partial class Supply
    {
        public double? Y2050 { get; set; }
        public double? YPlus10 { get; set; }
        public double? Liquid { get; set; }
        public double Circulating { get; set; }
        public double? Y2050_IssuedPercent { get; set; }
        public double? AnnualInflationPercent { get; set; }
        public double? StockToFlow { get; set; }
        public double? YPlus10IssuedPercent { get; set; }
    }

    public partial class TokenSaleStats
    {
        public long? SaleProceedsUsd { get; set; }
        public DateTimeOffset? SaleStartDate { get; set; }
        public DateTimeOffset? SaleEndDate { get; set; }
        public double? RoiSinceSaleUsdPercent { get; set; }
        public double? RoiSinceSaleBtcPercent { get; set; }
        public object RoiSinceSaleEthPercent { get; set; }
    }

    public partial class Profile
    {
        public bool IsVerified { get; set; }
        public string Tagline { get; set; }
        public string Overview { get; set; }
        public string Background { get; set; }
        public string Technology { get; set; }
        public ProfileCategory Category { get; set; }
        public string Sector { get; set; }
        public string Tag { get; set; }
        public double? SfarScore { get; set; }
        public TokenDistribution TokenDistribution { get; set; }
        public TokenDetails TokenDetails { get; set; }
        public List<Organization> Organizations { get; set; }
        public People People { get; set; }
        public List<RelevantResource> RelevantResources { get; set; }
        public string ConsensusAlgorithm { get; set; }
    }

    public partial class Organization
    {
        public string Name { get; set; }
        public DateTimeOffset? FoundedDate { get; set; }
        public object Governance { get; set; }
        public string LegalStructure { get; set; }
        public string Jurisdiction { get; set; }
        public object OrgCharter { get; set; }
        public string Description { get; set; }
        public string PeopleCountEstimate { get; set; }
    }

    public partial class People
    {
        public object FoundingTeam { get; set; }
        public List<Advisor> Contributors { get; set; }
        public List<Advisor> Investors { get; set; }
        public List<Advisor> Advisors { get; set; }
    }

    public partial class Advisor
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Description { get; set; }
        public string AvatarUrl { get; set; }
        public Uri Twitter { get; set; }
        public Uri Github { get; set; }
        public Uri Medium { get; set; }
        public Uri Linkedin { get; set; }
    }

    public partial class RelevantResource
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }

    public partial class TokenDetails
    {
        public string Usage { get; set; }
        public TypeEnum Type { get; set; }
        public List<SalesRound> SalesRounds { get; set; }
        public double? BlockReward { get; set; }
        public double? TargetedBlockTimeInSec { get; set; }
        public string OnChainGovernanceStructure { get; set; }
        public bool? IsTreasuryDecentralized { get; set; }
        public string LaunchStyle { get; set; }
        public double? InitialSupply { get; set; }
        public double? PercentageAllocatedToInvestorsFromInitialSupply { get; set; }
        public double? PercentageAllocatedToPreminedOrAirdropsFromInitialSupply { get; set; }
        public double? PercentageAllocatedToOrganizationsOrFoundersSupply { get; set; }
        public string MiningAlgorithm { get; set; }
        public DateTimeOffset? NextHalvingDate { get; set; }
        public DateTimeOffset? GenesisBlockDate { get; set; }
        public bool? IsVictimOf51_PercentAttack { get; set; }
        public EmissionTypeGeneral? EmissionTypeGeneral { get; set; }
        public string EmissionTypePrecise { get; set; }
        public bool? IsCappedSupply { get; set; }
        public long? MaxSupply { get; set; }
    }

    public partial class SalesRound
    {
        public string RoundName { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public double? PricePerUnit { get; set; }
        public Unit Unit { get; set; }
        public double AmountCollected { get; set; }
        public object Restriction { get; set; }
    }

    public partial class TokenDistribution
    {
        public DateTimeOffset? SaleStart { get; set; }
        public DateTimeOffset? SaleEnd { get; set; }
        public long? InitialDistribution { get; set; }
        public long? CurrentSupply { get; set; }
        public long? MaxSupply { get; set; }
        public string Description { get; set; }
    }

    public partial class Status
    {
        public long Elapsed { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }

    public enum CategoryElement { Currency, Financial, Infrastructure, Services };

    public enum ProfileCategory { Financial, Infrastructure, Payments, Services };

    public enum EmissionTypeGeneral { Deflationary, FixedSupply, Inflationary };

    public enum Unit { Btc, Usd };

    public enum TypeEnum { EosioErc20, Erc20, Erc20OmniTrc20, Native };

}
