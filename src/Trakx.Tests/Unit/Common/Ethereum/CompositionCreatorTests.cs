using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Nethereum.ABI.Encoders;
using NSubstitute;
using Trakx.Common.Ethereum;
using Trakx.Common.Extensions;
using Trakx.Contracts.Set;
using Trakx.Contracts.Set.Core;
using Trakx.Tests.Data;
using Xunit;

namespace Trakx.Tests.Unit.Common.Ethereum
{
    public class CompositionCreatorTests
    {
        private readonly ICoreService _coreService;

        public CompositionCreatorTests()
        {
            _coreService = Substitute.For<ICoreService>();
            var transactionReceipt = new MockCreator().GetTransactionReceipt();
            _coreService.CreateSetRequestAndWaitForReceiptAsync(default, 
                    default, default, default, default, default, default)
                .ReturnsForAnyArgs(transactionReceipt);
        }

        [Fact]
        public async Task CreateCompositionOnChain_should_rescale_quantities()
        {
            var stringTypeEncoder = new StringTypeEncoder();

            var compositionCreator = new CompositionCreator(_coreService, Substitute.For<ILogger<CompositionCreator>>());

            var composition = new MockCreator().GetIndiceComposition(3);

            var expectedComponents = composition.ComponentQuantities.Select(q => q.ComponentDefinition.Address).ToList();
            var expectedQuantities = composition.ComponentQuantities.Select(q =>
                    new BigInteger(q.Quantity.DescaleComponentQuantity(
                                       q.ComponentDefinition.Decimals, composition.IndiceDefinition.NaturalUnit))).ToList();
            var expectedNaturalUnit = composition.IndiceDefinition.NaturalUnit.AsAPowerOf10();

            var _ = await compositionCreator.SaveCompositionOnChain(composition);

            var receivedCall = _coreService.ReceivedCalls().Single();

            var arguments = receivedCall.GetArguments();
            arguments[0].Should().Be(DeployedContractAddresses.SetTokenFactory);
            arguments[1].Should().BeEquivalentTo(expectedComponents);
            arguments[2].Should().BeEquivalentTo(expectedQuantities);
            arguments[3].Should().BeEquivalentTo(expectedNaturalUnit);
            arguments[4].Should().BeEquivalentTo(stringTypeEncoder.EncodePacked(composition.IndiceDefinition.Name));
            arguments[5].Should().BeEquivalentTo(stringTypeEncoder.EncodePacked(composition.Symbol));
        }
    }
}