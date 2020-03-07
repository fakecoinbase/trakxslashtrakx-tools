using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
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
using Trakx.Contracts.Set;
using Trakx.Contracts.Set.Core;
using Trakx.Data.Common.Extensions;
using Trakx.Data.Common.Interfaces.Index;
[assembly:InternalsVisibleTo("Trakx.Data.Tests")]

namespace Trakx.Data.Common.Ethereum
{
    public interface IIndexCreator
    {
        Task<string> SaveIndexOnChain(IIndexComposition initialComposition);
    }

    public class IndexCreator : IIndexCreator
    {
        private readonly ICoreService _coreService;
        private readonly ILogger<CompositionCreator> _logger;
        private readonly IWeb3 _web3;

        public IndexCreator(ICoreService coreService, IWeb3 web3, ILogger<CompositionCreator> logger)
        {
            _coreService = coreService;
            _logger = logger;
            _web3 = web3;
        }

        internal async Task<string> SaveIndexOnChain(IIndexComposition initialComposition, string rebalancingFactoryAddress)
        {
            Guard.Against.Null(initialComposition.IndexDefinition, nameof(initialComposition.IndexDefinition));
            var stringTypeEncoder = new StringTypeEncoder();

            var callData = GenerateRebalancingCallData(_web3.TransactionManager.Account.Address,
                TimeSpan.Zero, TimeSpan.FromDays(28));

            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(120));
            _logger.LogInformation("Saving rebalancing index on chain...");

            try
            {
                var receipt = await _coreService.CreateSetRequestAndWaitForReceiptAsync(rebalancingFactoryAddress, 
                    new []{ initialComposition.Address }.ToList(),
                    new[] { initialComposition.IndexDefinition.NaturalUnit.AsAPowerOf10() }.ToList(),
                    ((ushort)10).AsAPowerOf10(),
                    stringTypeEncoder.EncodePacked(initialComposition.IndexDefinition.Name),
                    stringTypeEncoder.EncodePacked(initialComposition.IndexDefinition.Symbol),
                    stringTypeEncoder.EncodePacked(callData),
                    cancellationTokenSource);

                _logger.LogInformation("Saved rebalancing index on chain with transaction {0}", receipt.TransactionHash);

                return receipt.TransactionHash;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save rebalancing index on chain.");
                return "";
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

        public async Task<string> SaveIndexOnChain(IIndexComposition initialComposition)
        {
            return await SaveIndexOnChain(initialComposition,
                    DeployedContractAddresses.RebalancingSetTokenFactory)
                .ConfigureAwait(false);
        }
    }
}