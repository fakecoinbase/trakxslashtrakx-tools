using System;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nethereum.ABI.Encoders;
using Newtonsoft.Json;
using Trakx.Common.Extensions;
using Trakx.Common.Interfaces.Index;
using Trakx.Contracts.Set;
using Trakx.Contracts.Set.Core;

[assembly: InternalsVisibleTo("Trakx.Tests")]

namespace Trakx.Common.Ethereum
{
    public interface ICompositionCreator
    {
        Task<string> SaveCompositionOnChain(IIndexComposition composition);
    }

    public class CompositionCreator : ICompositionCreator
    {
        private readonly ILogger<CompositionCreator> _logger;
        private readonly ICoreService _coreService;

        public CompositionCreator(ICoreService coreService, ILogger<CompositionCreator> logger)
        {
            _logger = logger;
            _coreService = coreService;
        }

        internal async Task<string> SaveCompositionOnChain(IIndexComposition composition, string setTokenFactoryAddress)
        {
            var stringTypeEncoder = new StringTypeEncoder();

            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(5));
            _logger.LogInformation("Saving composition on chain...");

            try
            {
                var units = composition.ComponentQuantities.Select(q =>
                    new BigInteger(q.Quantity.DescaleComponentQuantity(
                        q.ComponentDefinition.Decimals, composition.IndexDefinition.NaturalUnit)))
                    .ToList();

                var addresses = composition.ComponentQuantities.Select(q => q.ComponentDefinition.Address).ToList();

                var receipt = await _coreService.CreateSetRequestAndWaitForReceiptAsync(setTokenFactoryAddress,
                    addresses,
                    units,
                    composition.IndexDefinition.NaturalUnit.AsAPowerOf10(),
                    stringTypeEncoder.EncodePacked(composition.IndexDefinition.Name),
                    stringTypeEncoder.EncodePacked(composition.Symbol),
                    stringTypeEncoder.Encode("0x0"),
                    cancellationTokenSource);
                _logger.LogInformation("Saved composition on chain with transaction {0}", receipt.TransactionHash);

                return receipt.Logs[0].ToString(Formatting.Indented);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save composition on chain.");
                throw;
            }
        }

        public async Task<string> SaveCompositionOnChain(IIndexComposition composition)
        {
            return await SaveCompositionOnChain(composition,
                    DeployedContractAddresses.SetTokenFactory)
                .ConfigureAwait(false);
        }
    }
}