using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nethereum.StandardTokenEIP20;
using Nethereum.Web3;

namespace Trakx.Common.Sources.Web3.Client
{
    public class Web3Client : IWeb3Client
    {
        private readonly ILogger<Web3Client> _logger;
        private readonly IWeb3 _web3;

        public Web3Client(ILogger<Web3Client> logger, IWeb3 web3)
        {
            _logger = logger;
            _web3 = web3;
        }

        /// <inheritdoc />
        public IWeb3 Web3 => _web3;

        public async Task<ushort?> GetDecimalsFromContractAddress(string contractAddress)
        {
            try
            {
                var contractService = new StandardTokenService((Nethereum.Web3.Web3)_web3, contractAddress);
                var bytes = await contractService.DecimalsQueryAsync();
                return (ushort)bytes;
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Failed to get decimals for contract {0}", contractAddress);
                return default;
            }
        }

        public async Task<string?> GetNameFromContractAddress(string contractAddress)
        {
            try
            {
                var contractService = new StandardTokenService((Nethereum.Web3.Web3)_web3, contractAddress);
                var name = await contractService.NameQueryAsync();
                return name;
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Failed to get name for contract {0}", contractAddress);
                return default;
            }
        }
        public async Task<string?> GetSymbolFromContractAddress(string contractAddress)
        {
            try
            {
                var contractService = new StandardTokenService((Nethereum.Web3.Web3)_web3, contractAddress);
                var symbol = await contractService.SymbolQueryAsync();
                return symbol;
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Failed to get symbol for contract {0}", contractAddress);
                return default;
            }
        }
    }
}
