using System;
using System.Collections.Generic;
using System.Text;

namespace Trakx.Data.Market.Common.Sources.Messari.DTOs
{
    public enum CategoryElement { Currency, Financial, Infrastructure, Services };

    public enum ProfileCategory { Financial, Infrastructure, Payments, Services };

    public enum LegalStructure { Corporation, Foundation, Llc, Other };

    public enum EmissionTypeGeneral { Deflationary, FixedSupply, Inflationary };

    public enum OnChainGovernanceStructure { DelegatedDirectOnChainVote, DelegatedOnChainVote, NoOnChainGovernance, Upcoming };

    public enum Unit { Btc, Usd };

    public enum TypeEnum { EosioErc20, Erc20, Erc20OmniTrc20, Native };
}
