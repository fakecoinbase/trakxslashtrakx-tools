using System.Threading.Tasks;
using Nethereum.Web3;

namespace Trakx.Common.Sources.Web3.Client
{
    public interface IWeb3Client
    {
        IWeb3 Web3 { get; }
        Task<ushort?> GetDecimalsFromContractAddress(string contractAddress);
    }
}