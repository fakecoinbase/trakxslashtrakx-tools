using System.Threading.Tasks;

namespace Trakx.Data.Common.Sources.Web3.Client
{
    public interface IWeb3Client
    {
        Task<ushort?> GetDecimalsFromContractAddress(string contractAddress);
    }
}