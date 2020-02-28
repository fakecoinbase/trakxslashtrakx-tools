using System.Collections.Generic;
using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Trakx.Contracts.Set.RebalancingSetExchangeIssuanceModule.ContractDefinition
{
    public partial class ExchangeIssuanceParams : ExchangeIssuanceParamsBase { }

    public class ExchangeIssuanceParamsBase 
    {
        [Parameter("address", "setAddress", 1)]
        public virtual string SetAddress { get; set; }
        [Parameter("uint256", "quantity", 2)]
        public virtual BigInteger Quantity { get; set; }
        [Parameter("uint8[]", "sendTokenExchangeIds", 3)]
        public virtual List<byte> SendTokenExchangeIds { get; set; }
        [Parameter("address[]", "sendTokens", 4)]
        public virtual List<string> SendTokens { get; set; }
        [Parameter("uint256[]", "sendTokenAmounts", 5)]
        public virtual List<BigInteger> SendTokenAmounts { get; set; }
        [Parameter("address[]", "receiveTokens", 6)]
        public virtual List<string> ReceiveTokens { get; set; }
        [Parameter("uint256[]", "receiveTokenAmounts", 7)]
        public virtual List<BigInteger> ReceiveTokenAmounts { get; set; }
    }
}
