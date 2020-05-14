﻿using System.Collections.ObjectModel;

namespace Trakx.Contracts.Set
{
    public class DeployedContractAddresses
    {
        protected DeployedContractAddresses() { }

        public static readonly string CommonValidationsLibrary = "0xC269E9396556B6AFB0C38eef4a590321FF9E8D3A";
        public static readonly string Core = "0xf55186CC537E7067EA616F2aaE007b4427a120C8";
        public static readonly string CoreIssuanceLibrary = "0x5f3F534D0C5Ea126150Ec8078d404464339503ca";
        public static readonly string ERC20Wrapper = "0xeaDadA7c6943c223C0d4bEA475a6DACC7368f8d6";
        public static readonly string ExchangeIssueLibrary = "0xbAFb2fEa7C1188D8fbAB070196d0aB77A131c71C";
        public static readonly string ExchangeIssueModule = "0x73dF03B5436C84Cf9d5A758fb756928DCEAf19d7";
        public static readonly string FailAuctionLibrary = "0xa2619134b0851744d6e5052392400df73b24d7Fc";
        public static readonly string KyberNetworkWrapper = "0x9B3Eb3B22DC2C29e878d7766276a86A8395fB56d";
        public static readonly string LinearAuctionPriceCurve = "0x2048b012c6688996A25bCD9742e7dA1ff272b957";
        public static readonly string PlaceBidLibrary = "0x4689051F4246630deb7C1C4cfb2ffA25643D886C";
        public static readonly string ProposeLibrary = "0xDD5825965A016D8bBbBDF4862A1Ac9D3Fb6d5382";
        public static readonly string ProtocolViewer = "0x589d4b4d311EFaAc93f0032238BecD6f4D397b0f";
        public static readonly string RebalanceAuctionModule = "0xe23FB31dD2edacEbF7d92720358bB92445F47fDB";
        public static readonly string RebalancingLibrary = "0x3Ac81153Ae6a096eaEA0990fa0366914C425eF85";
        public static readonly string RebalancingSetExchangeIssuanceModule = "0xd4240987D6F92B06c8B5068B1E4006A97c47392b";
        public static readonly string RebalancingSetIssuanceModule = "0xcEDA8318522D348f1d1aca48B24629b8FbF09020";
        public static readonly string RebalancingSetTokenFactory = "0x15518Cdd49d83471e9f85cdCFBD72c8e2a78dDE2";
        public static readonly string SetTokenFactory = "0xE1Cd722575801fE92EEef2CA23396557F7E3B967";
        public static readonly string SetTokenLibrary = "0xdC733EC262F32882F7C05525cc2D09F2C04D86Ac";
        public static readonly string SettleRebalanceLibrary = "0xC0aAFEE4B4edC54Dd3AEa0Bf4DBe7BddDe6365ca";
        public static readonly string StartRebalanceLibrary = "0xb2d113Cd923b763Bd4f2187233257DA57f3F1dDB";
        public static readonly string TransferProxy = "0x882d80D3a191859d64477eb78Cca46599307ec1C";
        public static readonly string Vault = "0x5B67871C3a857dE81A1ca0f9F7945e5670D986Dc";
        public static readonly string WhiteList = "0xc6449473BE76AB2a70329fA66Cbe504a25005338";
        public static readonly string ZeroExExchangeWrapper = "0xA2bb0b46960f24C9720F56639E08aD6C0E101C61";

        public static readonly ReadOnlyDictionary<string, string> AddressByName =
            ReflectionHelper.GetStaticStringPropertiesByNames<DeployedContractAddresses>();
    }
}
