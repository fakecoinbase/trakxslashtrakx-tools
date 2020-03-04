using System; 
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nethereum.ABI.Encoders;
using Trakx.Contracts.Set;
using Trakx.Contracts.Set.Core;
using Trakx.Data.Common.Interfaces.Index;
[assembly:InternalsVisibleTo("Trakx.Data.Tests")]

namespace Trakx.Data.Common.Ethereum
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

            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(120));
            _logger.LogInformation("Saving composition on chain...");

            try
            {
                var receipt = await _coreService.CreateSetRequestAndWaitForReceiptAsync(setTokenFactoryAddress,
            composition.ComponentQuantities.Select(q => q.ComponentDefinition.Address).ToList(),
                composition.ComponentQuantities.Select(q =>
                    new BigInteger(q.Quantity * (decimal)Math.Pow(10, q.ComponentDefinition.Decimals + composition.IndexDefinition.NaturalUnit - 18))).ToList(),
                    new BigInteger(Math.Pow(10, composition.IndexDefinition.NaturalUnit)),
                    stringTypeEncoder.EncodePacked(composition.IndexDefinition.Name),
                    stringTypeEncoder.EncodePacked(composition.Symbol),
                    stringTypeEncoder.Encode("0x0"),
                    cancellationTokenSource);
                _logger.LogInformation("Saved composition on chain with transaction {0}", receipt.TransactionHash);

                return receipt.TransactionHash;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save composition on chain.");
                return "";
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