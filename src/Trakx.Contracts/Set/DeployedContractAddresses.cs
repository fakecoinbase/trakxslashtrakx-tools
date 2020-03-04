using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Trakx.Contracts.Set
{
    public static class DeployedContractAddresses
    {
        public const string CommonValidationsLibrary = "0xC269E9396556B6AFB0C38eef4a590321FF9E8D3A";
        public const string Core = "0xf55186CC537E7067EA616F2aaE007b4427a120C8";
        public const string CoreIssuanceLibrary = "0x5f3F534D0C5Ea126150Ec8078d404464339503ca";
        public const string ERC20Wrapper = "0xeaDadA7c6943c223C0d4bEA475a6DACC7368f8d6";
        public const string ExchangeIssueLibrary = "0xbAFb2fEa7C1188D8fbAB070196d0aB77A131c71C";
        public const string ExchangeIssueModule = "0x73dF03B5436C84Cf9d5A758fb756928DCEAf19d7";
        public const string FailAuctionLibrary = "0xa2619134b0851744d6e5052392400df73b24d7Fc";
        public const string KyberNetworkWrapper = "0x9B3Eb3B22DC2C29e878d7766276a86A8395fB56d";
        public const string LinearAuctionPriceCurve = "0x2048b012c6688996A25bCD9742e7dA1ff272b957";
        public const string PlaceBidLibrary = "0x4689051F4246630deb7C1C4cfb2ffA25643D886C";
        public const string ProposeLibrary = "0xDD5825965A016D8bBbBDF4862A1Ac9D3Fb6d5382";
        public const string ProtocolViewer = "0x589d4b4d311EFaAc93f0032238BecD6f4D397b0f";
        public const string RebalanceAuctionModule = "0xe23FB31dD2edacEbF7d92720358bB92445F47fDB";
        public const string RebalancingLibrary = "0x3Ac81153Ae6a096eaEA0990fa0366914C425eF85";
        public const string RebalancingSetExchangeIssuanceModule = "0xd4240987D6F92B06c8B5068B1E4006A97c47392b";
        public const string RebalancingSetIssuanceModule = "0xcEDA8318522D348f1d1aca48B24629b8FbF09020";
        public const string RebalancingSetTokenFactory = "0x15518Cdd49d83471e9f85cdCFBD72c8e2a78dDE2";
        public const string SetTokenFactory = "0xE1Cd722575801fE92EEef2CA23396557F7E3B967";
        public const string SetTokenLibrary = "0xdC733EC262F32882F7C05525cc2D09F2C04D86Ac";
        public const string SettleRebalanceLibrary = "0xC0aAFEE4B4edC54Dd3AEa0Bf4DBe7BddDe6365ca";
        public const string StartRebalanceLibrary = "0xb2d113Cd923b763Bd4f2187233257DA57f3F1dDB";
        public const string TransferProxy = "0x882d80D3a191859d64477eb78Cca46599307ec1C";
        public const string Vault = "0x5B67871C3a857dE81A1ca0f9F7945e5670D986Dc";
        public const string WhiteList = "0xc6449473BE76AB2a70329fA66Cbe504a25005338";
        public const string ZeroExExchangeWrapper = "0xA2bb0b46960f24C9720F56639E08aD6C0E101C61";

        public static readonly ReadOnlyDictionary<string, string> AddressByName = new ReadOnlyDictionary<string, string>(
                typeof(DeployedContractAddresses)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
                .ToDictionary(fi => fi.Name, fi => fi.GetValue(null).ToString()));
    }
}
