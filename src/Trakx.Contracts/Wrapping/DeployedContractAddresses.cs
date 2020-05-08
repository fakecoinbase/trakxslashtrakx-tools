using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using Trakx.Contracts.Set;

namespace Trakx.Contracts.Wrapping
{
    public static class DeployedContractAddresses
    {
        public static readonly string wxrp = "0xbbD8689321a543510a67e4a9F9D5e7B0C2de5257"; //rinkeby network

        public static readonly ReadOnlyDictionary<string, string> AddressByName = Address.AddressByName;
    }
}
