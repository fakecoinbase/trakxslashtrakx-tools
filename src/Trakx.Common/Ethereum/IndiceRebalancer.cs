using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Microsoft.Extensions.Logging;
using Nethereum.Web3;
using Newtonsoft.Json;
using Trakx.Common.Interfaces.Indice;
using Trakx.Contracts.RebalancingSetToken;

namespace Trakx.Common.Ethereum
{
    public interface IIndiceRebalancer
    {
        Task<string> ProposeRebalancing(IIndiceComposition newComposition);
        Task<string> StartRebalancing();
        Task<string> SettleRebalancing();
    }

    public class IndiceRebalancer : IIndiceRebalancer
    {
        private const string LinearAuctionLibrary = "0x2048b012c6688996A25bCD9742e7dA1ff272b957";
        private readonly RebalancingSetTokenService _contract;
        private readonly ILogger<IndiceRebalancer> _logger;

        public IndiceRebalancer(IIndiceDefinition indiceDefinition, IWeb3 web3, ILogger<IndiceRebalancer> logger)
        {
            _logger = logger;
            _contract = new RebalancingSetTokenService(web3, indiceDefinition.Address);
        }

        public async Task<string> ProposeRebalancing(IIndiceComposition newComposition)
        {
            Guard.Against.Null(newComposition.IndiceDefinition, nameof(newComposition.IndiceDefinition));

            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(5));
            _logger.LogInformation("Proposing rebalancing on chain...");

            try
            {
                var receipt = await _contract.ProposeRequestAndWaitForReceiptAsync(newComposition.Address, LinearAuctionLibrary,
                    BigInteger.Zero, new BigInteger(100), new BigInteger(100), cancellationTokenSource);

                _logger.LogInformation("Proposed rebalancing indice on chain with transaction {0}", receipt.TransactionHash);

                return receipt.Logs[0].ToString(Formatting.Indented);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to propose rebalancing indice on chain.");
                throw;
            }
        }

        public async Task<string> StartRebalancing()
        {
            _logger.LogInformation("Starting rebalancing on chain...");

            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(5));
            try
            {
                var receipt = await _contract.StartRebalanceRequestAndWaitForReceiptAsync(cancellationTokenSource);

                _logger.LogInformation("Rebalancing started on chain with transaction {0}", receipt.TransactionHash);

                return receipt.Logs[0].ToString(Formatting.Indented);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start rebalancing on chain.");
                throw;
            }
        }

        public async Task<string> SettleRebalancing()
        {
            _logger.LogInformation("Settling rebalance on chain...");

            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(5));
            try
            {
                var receipt = await _contract.StartRebalanceRequestAndWaitForReceiptAsync(cancellationTokenSource);

                _logger.LogInformation("Starting to settle rebalancing on chain with transaction {0}", receipt.TransactionHash);

                return receipt.Logs[0].ToString(Formatting.Indented);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to settle rebalancing on chain.");
                throw;
            }
        }


    }
}