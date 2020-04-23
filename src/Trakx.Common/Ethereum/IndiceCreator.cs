using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Microsoft.Extensions.Logging;
using Nethereum.ABI.Encoders;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.Util;
using Nethereum.Web3;
using Newtonsoft.Json;
using Trakx.Common.Extensions;
using Trakx.Common.Interfaces.Indice;
using Trakx.Contracts.Set;
using Trakx.Contracts.Set.Core;

[assembly:InternalsVisibleTo("Trakx.Tests")]

namespace Trakx.Common.Ethereum
{
    public interface IIndiceCreator
    {
        Task<string> SaveIndiceOnChain(IIndiceComposition initialComposition);
    }

    public class IndiceCreator : IIndiceCreator
    {
        private readonly ICoreService _coreService;
        private readonly ILogger<CompositionCreator> _logger;
        private readonly IWeb3 _web3;

        public IndiceCreator(ICoreService coreService, IWeb3 web3, ILogger<CompositionCreator> logger)
        {
            _coreService = coreService;
            _logger = logger;
            _web3 = web3;
        }

        internal async Task<string> SaveIndiceOnChain(IIndiceComposition initialComposition, string rebalancingFactoryAddress)
        {
            Guard.Against.Null(initialComposition.IndiceDefinition, nameof(initialComposition.IndiceDefinition));
            var stringTypeEncoder = new StringTypeEncoder();

            var callData = GenerateRebalancingCallData(_web3.TransactionManager.Account.Address,
                TimeSpan.Zero, TimeSpan.FromDays(1));

            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(5));
            _logger.LogInformation("Saving rebalancing indice on chain...");

            try
            {
                var receipt = await _coreService.CreateSetRequestAndWaitForReceiptAsync(rebalancingFactoryAddress, 
                    new []{ initialComposition.Address }.ToList(),
                    new[] { initialComposition.IndiceDefinition.NaturalUnit.AsAPowerOf10() }.ToList(),
                    initialComposition.IndiceDefinition.NaturalUnit.AsAPowerOf10(),
                    stringTypeEncoder.EncodePacked(initialComposition.IndiceDefinition.Name),
                    stringTypeEncoder.EncodePacked(initialComposition.IndiceDefinition.Symbol),
                    callData.HexToByteArray(),
                    cancellationTokenSource);

                _logger.LogInformation("Saved rebalancing indice on chain with transaction {0}", receipt.TransactionHash);

                return receipt.Logs[0].ToString(Formatting.Indented);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save rebalancing indice on chain.");
                throw;
            }
        }

        internal string GenerateRebalancingCallData(string managerAddress, 
            TimeSpan proposalPeriod,
            TimeSpan minimumRebalanceInterval)
        {
            if(!managerAddress.IsValidEthereumAddressHexFormat()) throw new InvalidDataException("Invalid ethereum address.");
            var addressString = managerAddress.Substring(2).PadLeft(64, '0');
            var proposalPeriodString = ((int)proposalPeriod.TotalSeconds).ToHexBigInteger().HexValue.Substring(2).PadLeft(64, '0');
            var minimumRebalanceIntervalString = ((int)minimumRebalanceInterval.TotalSeconds).ToHexBigInteger()
                .HexValue.Substring(2).PadLeft(64, '0');

            return "0x" + addressString + proposalPeriodString + minimumRebalanceIntervalString;
        }

        public async Task<string> SaveIndiceOnChain(IIndiceComposition initialComposition)
        {
            return await SaveIndiceOnChain(initialComposition,
                    DeployedContractAddresses.RebalancingSetTokenFactory)
                .ConfigureAwait(false);
        }
    }
}