using System.Collections.Generic;
using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace Trakx.Contracts.Set.CommonValidationsLibrary.ContractDefinition
{


    public partial class CommonValidationsLibraryDeployment : CommonValidationsLibraryDeploymentBase
    {
        public CommonValidationsLibraryDeployment() : base(BYTECODE) { }
        public CommonValidationsLibraryDeployment(string byteCode) : base(byteCode) { }
    }

    public class CommonValidationsLibraryDeploymentBase : ContractDeploymentMessage
    {
        public static string BYTECODE = "6102a5610026600b82828239805160001a60731461001957fe5b30600052607381538281f3fe730000000000000000000000000000000000000000301460806040526004361061005c577c010000000000000000000000000000000000000000000000000000000060003504632c183f43811461006157806364cf166f14610125575b600080fd5b6101236004803603604081101561007757600080fd5b81019060208101813564010000000081111561009257600080fd5b8201836020820111156100a457600080fd5b803590602001918460208302840111640100000000831117156100c657600080fd5b9193909290916020810190356401000000008111156100e457600080fd5b8201836020820111156100f657600080fd5b8035906020019184602083028401116401000000008311171561011857600080fd5b509092509050610195565b005b6101236004803603602081101561013b57600080fd5b81019060208101813564010000000081111561015657600080fd5b82018360208201111561016857600080fd5b8035906020019184602083028401116401000000008311171561018a57600080fd5b509092509050610209565b82811461020357604080517f08c379a000000000000000000000000000000000000000000000000000000000815260206004820152601560248201527f496e707574206c656e677468206d69736d617463680000000000000000000000604482015290519081900360640190fd5b50505050565b8061027557604080517f08c379a000000000000000000000000000000000000000000000000000000000815260206004820181905260248201527f41646472657373206172726179206c656e677468206d757374206265203e2030604482015290519081900360640190fd5b505056fea165627a7a723058209aaae30bbe3eac44be0f373b9e396cf21e15811231f22b97c1bbff369197b9220029";
        public CommonValidationsLibraryDeploymentBase() : base(BYTECODE) { }
        public CommonValidationsLibraryDeploymentBase(string byteCode) : base(byteCode) { }

    }

    public partial class ValidateEqualLengthFunction : ValidateEqualLengthFunctionBase { }

    [Function("validateEqualLength")]
    public class ValidateEqualLengthFunctionBase : FunctionMessage
    {
        [Parameter("address[]", "_addressArray", 1)]
        public virtual List<string> AddressArray { get; set; }
        [Parameter("uint256[]", "_uint256Array", 2)]
        public virtual List<BigInteger> Uint256Array { get; set; }
    }

    public partial class ValidateNonEmptyFunction : ValidateNonEmptyFunctionBase { }

    [Function("validateNonEmpty")]
    public class ValidateNonEmptyFunctionBase : FunctionMessage
    {
        [Parameter("address[]", "_addressArray", 1)]
        public virtual List<string> AddressArray { get; set; }
    }




}
