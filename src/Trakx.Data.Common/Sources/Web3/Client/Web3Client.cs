using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nethereum.RLP;
using Nethereum.StandardTokenEIP20;
using Nethereum.Web3;

namespace Trakx.Data.Common.Sources.Web3.Client
{
    public class Web3Client : IWeb3Client
    {
        private readonly ILogger<Web3Client> _logger;
        private readonly Nethereum.Web3.Web3 _web3;

        public Web3Client(string apiKey, ILogger<Web3Client> logger)
        {
            _logger = logger;
            _web3 = new Nethereum.Web3.Web3($"https://mainnet.infura.io/v3/{apiKey}");
        }
        public async Task<ushort?> GetDecimalsFromContractAddress(string contractAddress)
        {
            try
            {
                var contractService = new StandardTokenService(_web3, contractAddress);
                var bytes = await contractService.DecimalsQueryAsync();
                return (ushort)bytes;
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Failed to get decimals for contract {0}", contractAddress);
                return default;
            }
        }
    }
}
