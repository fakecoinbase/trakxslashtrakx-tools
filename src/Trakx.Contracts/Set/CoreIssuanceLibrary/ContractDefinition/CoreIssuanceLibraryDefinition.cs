using System.Collections.Generic;
using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace Trakx.Contracts.Set.CoreIssuanceLibrary.ContractDefinition
{


    public partial class CoreIssuanceLibraryDeployment : CoreIssuanceLibraryDeploymentBase
    {
        public CoreIssuanceLibraryDeployment() : base(BYTECODE) { }
        public CoreIssuanceLibraryDeployment(string byteCode) : base(byteCode) { }
    }

    public class CoreIssuanceLibraryDeploymentBase : ContractDeploymentMessage
    {
        public static string BYTECODE = "61090a610026600b82828239805160001a60731461001957fe5b30600052607381538281f3fe7300000000000000000000000000000000000000003014608060405260043610610067577c010000000000000000000000000000000000000000000000000000000060003504635720a5d9811461006c5780636cc34bcc146100965780639b9f5c93146100b6575b600080fd5b61007f61007a366004610677565b6100c9565b60405161008d929190610867565b60405180910390f35b6100a96100a43660046106cc565b6101c2565b60405161008d9190610856565b61007f6100c43660046105df565b6102ac565b6040805183815260208085028201019091526060908190849082908280156100fb578160200160208202803883390190505b50905060608260405190808252806020026020018201604052801561012a578160200160208202803883390190505b50905060005b838110156101b45760006101456002836104af565b90508781161561017f5789898381811061015b57fe5b9050602002013584838151811061016e57fe5b6020026020010181815250506101ab565b89898381811061018b57fe5b9050602002013583838151811061019e57fe5b6020026020010181815250505b50600101610130565b509097909650945050505050565b60606101d4828463ffffffff6104f016565b15610214576040517f08c379a000000000000000000000000000000000000000000000000000000000815260040161020b9061088c565b60405180910390fd5b604080518581526020808702820101909152606090858015610240578160200160208202803883390190505b50905060005b858110156102a25761028387878381811061025d57fe5b90506020020135610277878761050d90919063ffffffff16565b9063ffffffff61052f16565b82828151811061028f57fe5b6020908102919091010152600101610246565b5095945050505050565b6040805186815260208088028201019091526060908190879082908280156102de578160200160208202803883390190505b50905060608260405190808252806020026020018201604052801561030d578160200160208202803883390190505b50905060005b8381101561049e5760008773ffffffffffffffffffffffffffffffffffffffff16631f98ade38e8e8581811061034557fe5b9050602002013573ffffffffffffffffffffffffffffffffffffffff168b6040518363ffffffff167c010000000000000000000000000000000000000000000000000000000002815260040161039c92919061083b565b60206040518083038186803b1580156103b457600080fd5b505afa1580156103c8573d6000803e3d6000fd5b505050506040513d601f19601f820116820180604052506103ec9190810190610735565b90508a8a838181106103fa57fe5b905060200201358110610437578a8a8381811061041357fe5b9050602002013584838151811061042657fe5b602002602001018181525050610495565b8015610457578084838151811061044a57fe5b6020026020010181815250505b61047c818c8c8581811061046757fe5b9050602002013561055d90919063ffffffff16565b83838151811061048857fe5b6020026020010181815250505b50600101610313565b50909a909950975050505050505050565b60008083116104bd57600080fd5b600160005b838110156104e657816104db818763ffffffff61052f16565b9250506001016104c2565b5090505b92915050565b6000816104fc57600080fd5b81838161050557fe5b069392505050565b600080821161051b57600080fd5b600082848161052657fe5b04949350505050565b60008261053e575060006104ea565b8282028284828161054b57fe5b041461055657600080fd5b9392505050565b60008282111561056c57600080fd5b50900390565b600061055682356108af565b60008083601f84011261059057600080fd5b50813567ffffffffffffffff8111156105a857600080fd5b6020830191508360208202830111156105c057600080fd5b9250929050565b600061055682356108cd565b600061055682516108cd565b600080600080600080608087890312156105f857600080fd5b863567ffffffffffffffff81111561060f57600080fd5b61061b89828a0161057e565b9650965050602087013567ffffffffffffffff81111561063a57600080fd5b61064689828a0161057e565b9450945050604061065989828a01610572565b925050606061066a89828a01610572565b9150509295509295509295565b60008060006040848603121561068c57600080fd5b833567ffffffffffffffff8111156106a357600080fd5b6106af8682870161057e565b935093505060206106c2868287016105c7565b9150509250925092565b600080600080606085870312156106e257600080fd5b843567ffffffffffffffff8111156106f957600080fd5b6107058782880161057e565b94509450506020610718878288016105c7565b9250506040610729878288016105c7565b91505092959194509250565b60006020828403121561074757600080fd5b600061075384846105d3565b949350505050565b60006107678383610832565b505060200190565b610778816108af565b82525050565b6000610789826108a2565b61079381856108a6565b935061079e8361089c565b60005b828110156107c9576107b486835161075b565b95506107bf8261089c565b91506001016107a1565b5093949350505050565b60006107e0603c836108a6565b7f436f726549737375616e63654c6962726172793a205175616e74697479206d7581527f73742062652061206d756c7469706c65206f66206e617420756e697400000000602082015260400192915050565b610778816108cd565b60408101610849828561076f565b610556602083018461076f565b60208082528101610556818461077e565b60408082528101610878818561077e565b90508181036020830152610753818461077e565b602080825281016104ea816107d3565b60200190565b5190565b90815260200190565b600073ffffffffffffffffffffffffffffffffffffffff82166104ea565b9056fea265627a7a723058207eb3f5f427bd880926623f810ead89f289668951b99de69c73c5b804750c0e146c6578706572696d656e74616cf50037";
        public CoreIssuanceLibraryDeploymentBase() : base(BYTECODE) { }
        public CoreIssuanceLibraryDeploymentBase(string byteCode) : base(byteCode) { }

    }

    public partial class CalculateWithdrawAndIncrementQuantitiesFunction : CalculateWithdrawAndIncrementQuantitiesFunctionBase { }

    [Function("calculateWithdrawAndIncrementQuantities", typeof(CalculateWithdrawAndIncrementQuantitiesOutputDTO))]
    public class CalculateWithdrawAndIncrementQuantitiesFunctionBase : FunctionMessage
    {
        [Parameter("uint256[]", "_componentQuantities", 1)]
        public virtual List<BigInteger> ComponentQuantities { get; set; }
        [Parameter("uint256", "_toExclude", 2)]
        public virtual BigInteger ToExclude { get; set; }
    }

    public partial class CalculateRequiredComponentQuantitiesFunction : CalculateRequiredComponentQuantitiesFunctionBase { }

    [Function("calculateRequiredComponentQuantities", "uint256[]")]
    public class CalculateRequiredComponentQuantitiesFunctionBase : FunctionMessage
    {
        [Parameter("uint256[]", "_componentUnits", 1)]
        public virtual List<BigInteger> ComponentUnits { get; set; }
        [Parameter("uint256", "_naturalUnit", 2)]
        public virtual BigInteger NaturalUnit { get; set; }
        [Parameter("uint256", "_quantity", 3)]
        public virtual BigInteger Quantity { get; set; }
    }

    public partial class CalculateDepositAndDecrementQuantitiesFunction : CalculateDepositAndDecrementQuantitiesFunctionBase { }

    [Function("calculateDepositAndDecrementQuantities", typeof(CalculateDepositAndDecrementQuantitiesOutputDTO))]
    public class CalculateDepositAndDecrementQuantitiesFunctionBase : FunctionMessage
    {
        [Parameter("address[]", "_components", 1)]
        public virtual List<string> Components { get; set; }
        [Parameter("uint256[]", "_componentQuantities", 2)]
        public virtual List<BigInteger> ComponentQuantities { get; set; }
        [Parameter("address", "_owner", 3)]
        public virtual string Owner { get; set; }
        [Parameter("address", "_vault", 4)]
        public virtual string Vault { get; set; }
    }

    public partial class CalculateWithdrawAndIncrementQuantitiesOutputDTO : CalculateWithdrawAndIncrementQuantitiesOutputDTOBase { }

    [FunctionOutput]
    public class CalculateWithdrawAndIncrementQuantitiesOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint256[]", "", 1)]
        public virtual List<BigInteger> ReturnValue1 { get; set; }
        [Parameter("uint256[]", "", 2)]
        public virtual List<BigInteger> ReturnValue2 { get; set; }
    }

    public partial class CalculateRequiredComponentQuantitiesOutputDTO : CalculateRequiredComponentQuantitiesOutputDTOBase { }

    [FunctionOutput]
    public class CalculateRequiredComponentQuantitiesOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint256[]", "", 1)]
        public virtual List<BigInteger> ReturnValue1 { get; set; }
    }

    public partial class CalculateDepositAndDecrementQuantitiesOutputDTO : CalculateDepositAndDecrementQuantitiesOutputDTOBase { }

    [FunctionOutput]
    public class CalculateDepositAndDecrementQuantitiesOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint256[]", "", 1)]
        public virtual List<BigInteger> ReturnValue1 { get; set; }
        [Parameter("uint256[]", "", 2)]
        public virtual List<BigInteger> ReturnValue2 { get; set; }
    }
}