using System;
using System.Collections.Generic;

namespace Trakx.Data.Market.Common.Sources.Messari.DTOs.GetProfileBySymbol
{
    public partial class GetProfileBySymbolResponse
    {
        public Status Status { get; set; }
        public Profile Data { get; set; }
    }

    public partial class Profile
    {
        public Guid Id { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public bool IsVerified { get; set; }
        public string Tagline { get; set; }
        public string Overview { get; set; }
        public string Background { get; set; }
        public object Technology { get; set; }
        public string Category { get; set; }
        public string Sector { get; set; }
        public object Tag { get; set; }
        public object SfarScore { get; set; }
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
        public object FoundedDate { get; set; }
        public object Governance { get; set; }
        public object LegalStructure { get; set; }
        public string Jurisdiction { get; set; }
        public object OrgCharter { get; set; }
        public string Description { get; set; }
        public object PeopleCountEstimate { get; set; }
    }

    public partial class People
    {
        public object FoundingTeam { get; set; }
        public object Contributors { get; set; }
        public object Investors { get; set; }
        public object Advisors { get; set; }
    }

    public partial class RelevantResource
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }

    public partial class TokenDetails
    {
        public string Usage { get; set; }
        public string Type { get; set; }
        public List<SalesRound> SalesRounds { get; set; }
        public object BlockReward { get; set; }
        public object TargetedBlockTimeInSec { get; set; }
        public string OnChainGovernanceStructure { get; set; }
        public bool IsTreasuryDecentralized { get; set; }
        public object LaunchStyle { get; set; }
        public object InitialSupply { get; set; }
        public object PercentageAllocatedToInvestorsFromInitialSupply { get; set; }
        public object PercentageAllocatedToPreminedOrAirdropsFromInitialSupply { get; set; }
        public object PercentageAllocatedToOrganizationsOrFoundersSupply { get; set; }
        public string MiningAlgorithm { get; set; }
        public object NextHalvingDate { get; set; }
        public object GenesisBlockDate { get; set; }
        public bool IsVictimOf51_PercentAttack { get; set; }
        public object EmissionTypeGeneral { get; set; }
        public object EmissionTypePrecise { get; set; }
        public object IsCappedSupply { get; set; }
        public object MaxSupply { get; set; }
    }

    public partial class SalesRound
    {
        public string RoundName { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public double PricePerUnit { get; set; }
        public string Unit { get; set; }
        public long AmountCollected { get; set; }
        public object Restriction { get; set; }
    }

    public partial class TokenDistribution
    {
        public object SaleStart { get; set; }
        public object SaleEnd { get; set; }
        public object InitialDistribution { get; set; }
        public object CurrentSupply { get; set; }
        public object MaxSupply { get; set; }
        public string Description { get; set; }
    }

    public partial class Status
    {
        public long Elapsed { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }

}
