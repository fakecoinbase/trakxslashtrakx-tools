using System.Collections.ObjectModel;

namespace Trakx.Contracts.Wrapping
{
    public class RinkebyDeployedContractAddresses
    {
        protected RinkebyDeployedContractAddresses() { }

        public static readonly string wxrp = "0xbbD8689321a543510a67e4a9F9D5e7B0C2de5257";

        public static readonly ReadOnlyDictionary<string, string> AddressByName =
            ReflectionHelper.GetStaticStringPropertiesByNames<RinkebyDeployedContractAddresses>();
    }
}
