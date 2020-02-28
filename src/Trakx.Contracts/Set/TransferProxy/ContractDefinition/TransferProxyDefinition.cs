using System.Collections.Generic;
using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace Trakx.Contracts.Set.TransferProxy.ContractDefinition
{


    public partial class TransferProxyDeployment : TransferProxyDeploymentBase
    {
        public TransferProxyDeployment() : base(BYTECODE) { }
        public TransferProxyDeployment(string byteCode) : base(byteCode) { }
    }

    public class TransferProxyDeploymentBase : ContractDeploymentMessage
    {
        public static string BYTECODE = "6080604081905260008054600160a060020a0319163317808255600160a060020a0316917f8be0079c531659141344cd1fd0a4f28419497f9722a3daafe3b4186f6b6457e0908290a36112ac806100576000396000f3fe608060405234801561001057600080fd5b5060043610610107576000357c0100000000000000000000000000000000000000000000000000000000900480638da5cb5b116100a9578063a6c4e46711610083578063a6c4e467146102e6578063b918161114610322578063d39de6e914610348578063f2fde38b146103a057610107565b80638da5cb5b146102a55780638f32d59b146102ad5780639303b16f146102c957610107565b806370712939116100e5578063707129391461019c578063709a385e146101c2578063715018a61461029557806378446bc11461029d57610107565b80631766486d1461010c57806342f1181e1461013b578063494503d414610163575b600080fd5b6101296004803603602081101561012257600080fd5b50356103c6565b60408051918252519081900360200190f35b6101616004803603602081101561015157600080fd5b5035600160a060020a03166103d8565b005b6101806004803603602081101561017957600080fd5b5035610702565b60408051600160a060020a039092168252519081900360200190f35b610161600480360360208110156101b257600080fd5b5035600160a060020a0316610729565b610161600480360360808110156101d857600080fd5b8101906020810181356401000000008111156101f357600080fd5b82018360208201111561020557600080fd5b8035906020019184602083028401116401000000008311171561022757600080fd5b91939092909160208101903564010000000081111561024557600080fd5b82018360208201111561025757600080fd5b8035906020019184602083028401116401000000008311171561027957600080fd5b9193509150600160a060020a0381358116916020013516610878565b6101616109bb565b610129610a23565b610180610a29565b6102b5610a39565b604080519115158252519081900360200190f35b610161600480360360208110156102df57600080fd5b5035610a4a565b610161600480360360808110156102fc57600080fd5b50600160a060020a03813581169160208101359160408201358116916060013516610aa3565b6102b56004803603602081101561033857600080fd5b5035600160a060020a0316610d51565b610350610d66565b60408051602080825283518183015283519192839290830191858101910280838360005b8381101561038c578181015183820152602001610374565b505050509050019250505060405180910390f35b610161600480360360208110156103b657600080fd5b5035600160a060020a0316610dc8565b60026020526000908152604090205481565b6103e0610a39565b6103e957600080fd5b6001546104f757600160a060020a03811660009081526003602052604090205460ff161561044b5760405160e560020a62461bcd02815260040180806020018281038252603d81526020018061115c603d913960400191505060405180910390fd5b600160a060020a0381166000818152600360209081526040808320805460ff191660019081179091556004805491820181559093527f8a35acfbc15ff81a39ae7d344fd709f28e8600b4aa8c65c6b64bfe7fe36bd19b909201805473ffffffffffffffffffffffffffffffffffffffff191684179055815133815291517f8918da6429714f0e9c40ae7f270773e27fc8caf7a256e19807f859563b7514de9281900390910190a26106ff565b60008036604051602001808383808284376040805191909301818103601f190182528352805160209182012060008181526002909252929020549195509093505050811515905061059657600082815260026020908152604091829020429081905582518581529182015281517f0e0905d1a972d476e353bdcc3e06b19a71709054c8ba01eccb7e0691eca6d374929181900390910190a150506106ff565b6001546105aa90829063ffffffff610de216565b4210156105eb5760405160e560020a62461bcd0281526004018080602001828103825260348152602001806112146034913960400191505060405180910390fd5b6000828152600260209081526040808320839055600160a060020a0386168352600390915290205460ff16156106555760405160e560020a62461bcd02815260040180806020018281038252603d81526020018061115c603d913960400191505060405180910390fd5b600160a060020a0383166000818152600360209081526040808320805460ff191660019081179091556004805491820181559093527f8a35acfbc15ff81a39ae7d344fd709f28e8600b4aa8c65c6b64bfe7fe36bd19b909201805473ffffffffffffffffffffffffffffffffffffffff191684179055815133815291517f8918da6429714f0e9c40ae7f270773e27fc8caf7a256e19807f859563b7514de9281900390910190a250505b50565b6004818154811061070f57fe5b600091825260209091200154600160a060020a0316905081565b610731610a39565b61073a57600080fd5b600160a060020a03811660009081526003602052604090205460ff166107945760405160e560020a62461bcd02815260040180806020018281038252603c8152602001806111d8603c913960400191505060405180910390fd5b600160a060020a038116600090815260036020908152604091829020805460ff19169055600480548351818402810184019094528084526108219385939092919083018282801561080e57602002820191906000526020600020905b8154600160a060020a031681526001909101906020018083116107f0575b5050505050610dfd90919063ffffffff16565b80516108359160049160209091019061100b565b50604080513381529051600160a060020a038316917f1f32c1b084e2de0713b8fb16bd46bb9df710a3dbeae2f3ca93af46e016dcc6b0919081900360200190a250565b3360009081526003602052604090205460ff166108c95760405160e560020a62461bcd02815260040180806020018281038252603f815260200180611199603f913960400191505060405180910390fd5b84806109095760405160e560020a62461bcd0281526004018080602001828103825260358152602001806111276035913960400191505060405180910390fd5b80841461094a5760405160e560020a62461bcd0281526004018080602001828103825260438152602001806110e46043913960600191505060405180910390fd5b60005b818110156109b157600086868381811061096357fe5b9050602002013511156109a9576109a988888381811061097f57fe5b90506020020135600160a060020a031687878481811061099b57fe5b905060200201358686610aa3565b60010161094d565b5050505050505050565b6109c3610a39565b6109cc57600080fd5b60008054604051600160a060020a03909116907f8be0079c531659141344cd1fd0a4f28419497f9722a3daafe3b4186f6b6457e0908390a36000805473ffffffffffffffffffffffffffffffffffffffff19169055565b60015481565b600054600160a060020a03165b90565b600054600160a060020a0316331490565b610a52610a39565b610a5b57600080fd5b6001548111610a9e5760405160e560020a62461bcd0281526004018080602001828103825260398152602001806112486039913960400191505060405180910390fd5b600155565b3360009081526003602052604090205460ff16610af45760405160e560020a62461bcd02815260040180806020018281038252603f815260200180611199603f913960400191505060405180910390fd5b8215610d4b57604080517ff7888aec000000000000000000000000000000000000000000000000000000008152600160a060020a03808716600483015283166024820152905160009173eadada7c6943c223c0d4bea475a6dacc7368f8d69163f7888aec91604480820192602092909190829003018186803b158015610b7957600080fd5b505af4158015610b8d573d6000803e3d6000fd5b505050506040513d6020811015610ba357600080fd5b5051604080517f15dacbea000000000000000000000000000000000000000000000000000000008152600160a060020a03808916600483015280871660248301528516604482015260648101879052905191925073eadada7c6943c223c0d4bea475a6dacc7368f8d6916315dacbea91608480820192600092909190829003018186803b158015610c3357600080fd5b505af4158015610c47573d6000803e3d6000fd5b5050604080517ff7888aec000000000000000000000000000000000000000000000000000000008152600160a060020a03808a1660048301528616602482015290516000935073eadada7c6943c223c0d4bea475a6dacc7368f8d6925063f7888aec91604480820192602092909190829003018186803b158015610cca57600080fd5b505af4158015610cde573d6000803e3d6000fd5b505050506040513d6020811015610cf457600080fd5b50519050610d08828663ffffffff610de216565b8114610d485760405160e560020a62461bcd0281526004018080602001828103825260358152602001806110af6035913960400191505060405180910390fd5b50505b50505050565b60036020526000908152604090205460ff1681565b60606004805480602002602001604051908101604052809291908181526020018280548015610dbe57602002820191906000526020600020905b8154600160a060020a03168152600190910190602001808311610da0575b5050505050905090565b610dd0610a39565b610dd957600080fd5b6106ff81610e32565b600082820183811015610df457600080fd5b90505b92915050565b6060600080610e0c8585610ead565b9150915080610e1a57600080fd5b6060610e268684610f11565b509350610df792505050565b600160a060020a038116610e4557600080fd5b60008054604051600160a060020a03808516939216917f8be0079c531659141344cd1fd0a4f28419497f9722a3daafe3b4186f6b6457e091a36000805473ffffffffffffffffffffffffffffffffffffffff1916600160a060020a0392909216919091179055565b81516000908190815b81811015610f005784600160a060020a0316868281518110610ed457fe5b6020026020010151600160a060020a03161415610ef857925060019150610f0a9050565b600101610eb6565b5060009250829150505b9250929050565b606060008084519050606060018203604051908082528060200260200182016040528015610f49578160200160208202803883390190505b50905060005b85811015610f9757868181518110610f6357fe5b6020026020010151828281518110610f7757fe5b600160a060020a0390921660209283029190910190910152600101610f4f565b50600185015b82811015610fe857868181518110610fb157fe5b6020026020010151826001830381518110610fc857fe5b600160a060020a0390921660209283029190910190910152600101610f9d565b5080868681518110610ff657fe5b60200260200101519350935050509250929050565b82805482825590600052602060002090810192821561106d579160200282015b8281111561106d578251825473ffffffffffffffffffffffffffffffffffffffff1916600160a060020a0390911617825560209092019160019091019061102b565b5061107992915061107d565b5090565b610a3691905b8082111561107957805473ffffffffffffffffffffffffffffffffffffffff1916815560010161108356fe5472616e7366657250726f78792e7472616e736665723a20496e76616c696420706f7374207472616e736665722062616c616e63655472616e7366657250726f78792e62617463685472616e736665723a20546f6b656e7320616e64207175616e746974696573206c656e67746873206d69736d617463685472616e7366657250726f78792e62617463685472616e736665723a20546f6b656e73206d757374206e6f7420626520656d707479417574686f72697a61626c652e616464417574686f72697a6564416464726573733a204164647265737320616c72656164792072656769737465726564417574686f72697a61626c652e6f6e6c79417574686f72697a65643a2053656e646572206e6f7420696e636c7564656420696e20617574686f726974696573417574686f72697a61626c652e72656d6f7665417574686f72697a6564416464726573733a2041646472657373206e6f7420617574686f72697a656454696d654c6f636b557067726164653a2054696d65206c6f636b20706572696f64206d757374206861766520656c61707365642e54696d654c6f636b557067726164653a204e657720706572696f64206d7573742062652067726561746572207468616e206578697374696e67a165627a7a723058206389644523db4f6d00b2f004ff35abed0729de15379f07903b12b7e7bccf485a0029";
        public TransferProxyDeploymentBase() : base(BYTECODE) { }
        public TransferProxyDeploymentBase(string byteCode) : base(byteCode) { }

    }

    public partial class TimeLockedUpgradesFunction : TimeLockedUpgradesFunctionBase { }

    [Function("timeLockedUpgrades", "uint256")]
    public class TimeLockedUpgradesFunctionBase : FunctionMessage
    {
        [Parameter("bytes32", "", 1)]
        public virtual byte[] ReturnValue1 { get; set; }
    }

    public partial class AddAuthorizedAddressFunction : AddAuthorizedAddressFunctionBase { }

    [Function("addAuthorizedAddress")]
    public class AddAuthorizedAddressFunctionBase : FunctionMessage
    {
        [Parameter("address", "_authTarget", 1)]
        public virtual string AuthTarget { get; set; }
    }

    public partial class AuthoritiesFunction : AuthoritiesFunctionBase { }

    [Function("authorities", "address")]
    public class AuthoritiesFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }

    public partial class RemoveAuthorizedAddressFunction : RemoveAuthorizedAddressFunctionBase { }

    [Function("removeAuthorizedAddress")]
    public class RemoveAuthorizedAddressFunctionBase : FunctionMessage
    {
        [Parameter("address", "_authTarget", 1)]
        public virtual string AuthTarget { get; set; }
    }

    public partial class BatchTransferFunction : BatchTransferFunctionBase { }

    [Function("batchTransfer")]
    public class BatchTransferFunctionBase : FunctionMessage
    {
        [Parameter("address[]", "_tokens", 1)]
        public virtual List<string> Tokens { get; set; }
        [Parameter("uint256[]", "_quantities", 2)]
        public virtual List<BigInteger> Quantities { get; set; }
        [Parameter("address", "_from", 3)]
        public virtual string From { get; set; }
        [Parameter("address", "_to", 4)]
        public virtual string To { get; set; }
    }

    public partial class RenounceOwnershipFunction : RenounceOwnershipFunctionBase { }

    [Function("renounceOwnership")]
    public class RenounceOwnershipFunctionBase : FunctionMessage
    {

    }

    public partial class TimeLockPeriodFunction : TimeLockPeriodFunctionBase { }

    [Function("timeLockPeriod", "uint256")]
    public class TimeLockPeriodFunctionBase : FunctionMessage
    {

    }

    public partial class OwnerFunction : OwnerFunctionBase { }

    [Function("owner", "address")]
    public class OwnerFunctionBase : FunctionMessage
    {

    }

    public partial class IsOwnerFunction : IsOwnerFunctionBase { }

    [Function("isOwner", "bool")]
    public class IsOwnerFunctionBase : FunctionMessage
    {

    }

    public partial class SetTimeLockPeriodFunction : SetTimeLockPeriodFunctionBase { }

    [Function("setTimeLockPeriod")]
    public class SetTimeLockPeriodFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "_timeLockPeriod", 1)]
        public virtual BigInteger TimeLockPeriod { get; set; }
    }

    public partial class TransferFunction : TransferFunctionBase { }

    [Function("transfer")]
    public class TransferFunctionBase : FunctionMessage
    {
        [Parameter("address", "_token", 1)]
        public virtual string Token { get; set; }
        [Parameter("uint256", "_quantity", 2)]
        public virtual BigInteger Quantity { get; set; }
        [Parameter("address", "_from", 3)]
        public virtual string From { get; set; }
        [Parameter("address", "_to", 4)]
        public virtual string To { get; set; }
    }

    public partial class AuthorizedFunction : AuthorizedFunctionBase { }

    [Function("authorized", "bool")]
    public class AuthorizedFunctionBase : FunctionMessage
    {
        [Parameter("address", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }

    public partial class GetAuthorizedAddressesFunction : GetAuthorizedAddressesFunctionBase { }

    [Function("getAuthorizedAddresses", "address[]")]
    public class GetAuthorizedAddressesFunctionBase : FunctionMessage
    {

    }

    public partial class TransferOwnershipFunction : TransferOwnershipFunctionBase { }

    [Function("transferOwnership")]
    public class TransferOwnershipFunctionBase : FunctionMessage
    {
        [Parameter("address", "newOwner", 1)]
        public virtual string NewOwner { get; set; }
    }

    public partial class AddressAuthorizedEventDTO : AddressAuthorizedEventDTOBase { }

    [Event("AddressAuthorized")]
    public class AddressAuthorizedEventDTOBase : IEventDTO
    {
        [Parameter("address", "authAddress", 1, true )]
        public virtual string AuthAddress { get; set; }
        [Parameter("address", "authorizedBy", 2, false )]
        public virtual string AuthorizedBy { get; set; }
    }

    public partial class AuthorizedAddressRemovedEventDTO : AuthorizedAddressRemovedEventDTOBase { }

    [Event("AuthorizedAddressRemoved")]
    public class AuthorizedAddressRemovedEventDTOBase : IEventDTO
    {
        [Parameter("address", "addressRemoved", 1, true )]
        public virtual string AddressRemoved { get; set; }
        [Parameter("address", "authorizedBy", 2, false )]
        public virtual string AuthorizedBy { get; set; }
    }

    public partial class UpgradeRegisteredEventDTO : UpgradeRegisteredEventDTOBase { }

    [Event("UpgradeRegistered")]
    public class UpgradeRegisteredEventDTOBase : IEventDTO
    {
        [Parameter("bytes32", "_upgradeHash", 1, false )]
        public virtual byte[] UpgradeHash { get; set; }
        [Parameter("uint256", "_timestamp", 2, false )]
        public virtual BigInteger Timestamp { get; set; }
    }

    public partial class OwnershipTransferredEventDTO : OwnershipTransferredEventDTOBase { }

    [Event("OwnershipTransferred")]
    public class OwnershipTransferredEventDTOBase : IEventDTO
    {
        [Parameter("address", "previousOwner", 1, true )]
        public virtual string PreviousOwner { get; set; }
        [Parameter("address", "newOwner", 2, true )]
        public virtual string NewOwner { get; set; }
    }

    public partial class TimeLockedUpgradesOutputDTO : TimeLockedUpgradesOutputDTOBase { }

    [FunctionOutput]
    public class TimeLockedUpgradesOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }



    public partial class AuthoritiesOutputDTO : AuthoritiesOutputDTOBase { }

    [FunctionOutput]
    public class AuthoritiesOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("address", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }







    public partial class TimeLockPeriodOutputDTO : TimeLockPeriodOutputDTOBase { }

    [FunctionOutput]
    public class TimeLockPeriodOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }

    public partial class OwnerOutputDTO : OwnerOutputDTOBase { }

    [FunctionOutput]
    public class OwnerOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("address", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }

    public partial class IsOwnerOutputDTO : IsOwnerOutputDTOBase { }

    [FunctionOutput]
    public class IsOwnerOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bool", "", 1)]
        public virtual bool ReturnValue1 { get; set; }
    }





    public partial class AuthorizedOutputDTO : AuthorizedOutputDTOBase { }

    [FunctionOutput]
    public class AuthorizedOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bool", "", 1)]
        public virtual bool ReturnValue1 { get; set; }
    }

    public partial class GetAuthorizedAddressesOutputDTO : GetAuthorizedAddressesOutputDTOBase { }

    [FunctionOutput]
    public class GetAuthorizedAddressesOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("address[]", "", 1)]
        public virtual List<string> ReturnValue1 { get; set; }
    }


}
