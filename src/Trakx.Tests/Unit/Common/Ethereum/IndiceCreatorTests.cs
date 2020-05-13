using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Nethereum.ABI.Encoders;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Web3;
using NSubstitute;
using Trakx.Common.Ethereum;
using Trakx.Common.Extensions;
using Trakx.Common.Interfaces.Indice;
using Trakx.Contracts.Set;
using Trakx.Contracts.Set.Core;
using Trakx.Tests.Data;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Tests.Unit.Common.Ethereum
{
    public class IndiceCreatorTests
    {
        private readonly ICoreService _coreService;
        private readonly IWeb3 _web3;
        private readonly string _accountAddress;
        private readonly IIndiceComposition _indiceComposition;


        public IndiceCreatorTests(ITestOutputHelper output)
        {
            var mockCreator = new MockCreator(output);
            _web3 = Substitute.For<IWeb3>();
            _accountAddress = mockCreator.GetRandomAddressEthereum();
            _web3.TransactionManager.Account.Address.Returns(_accountAddress);
            _coreService = Substitute.For<ICoreService>();

            var transactionReceipt = mockCreator.GetTransactionReceipt();
            _coreService.CreateSetRequestAndWaitForReceiptAsync(default,
                    default, default, default, default, default, default)
                .ReturnsForAnyArgs(transactionReceipt);

            _indiceComposition = mockCreator.GetIndiceComposition(3);
        }

        [Fact]
        public async Task CreateIndiceOnChain_should_do_some_dirty_parameter_transformations_to_conform_with_set_js_api()
        {
            var stringTypeEncoder = new StringTypeEncoder();

            var compositionCreator = new IndiceCreator(_coreService, _web3, 
                Substitute.For<ILogger<CompositionCreator>>());
            
            var expectedComponents = new []{ _indiceComposition.Address }.ToList();
            var expectedQuantities = new[] { _indiceComposition.IndiceDefinition.NaturalUnit.AsAPowerOf10() }.ToList();
            var expectedNaturalUnit = ((ushort)10).AsAPowerOf10();

            var expectedCallData = $"0x000000000000000000000000{_accountAddress.Substring(2)}" +
                                      "0000000000000000000000000000000000000000000000000000000000000000" +
                                      "0000000000000000000000000000000000000000000000000000000000015180";

            _ = await compositionCreator.SaveIndiceOnChain(_indiceComposition);

            var receivedCall = _coreService.ReceivedCalls().Single();

            var arguments = receivedCall.GetArguments();
            arguments[0].Should().Be(DeployedContractAddresses.RebalancingSetTokenFactory);
            arguments[1].Should().BeEquivalentTo(expectedComponents);
            arguments[2].Should().BeEquivalentTo(expectedQuantities);
            arguments[3].Should().BeEquivalentTo(expectedNaturalUnit);
            arguments[4].Should().BeEquivalentTo(stringTypeEncoder.EncodePacked(_indiceComposition.IndiceDefinition.Name));
            arguments[5].Should().BeEquivalentTo(stringTypeEncoder.EncodePacked(_indiceComposition.IndiceDefinition.Symbol));
            arguments[6].Should().BeEquivalentTo(expectedCallData.HexToByteArray());
        }
    }
}