using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nethereum.ABI.Encoders;
using Nethereum.Web3;
using NSubstitute;
using Trakx.Contracts.Set;
using Trakx.Contracts.Set.Core;
using Trakx.Data.Common.Ethereum;
using Trakx.Data.Common.Interfaces.Index;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Data.Tests.Unit.Common.Ethereum
{
    public class IndexCreatorTests
    {
        private readonly ICoreService _coreService;
        private readonly IWeb3 _web3;
        private readonly string _accountAddress;
        private readonly IIndexComposition _indexComposition;


        public IndexCreatorTests(ITestOutputHelper output)
        {
            _web3 = Substitute.For<IWeb3>();
            _accountAddress = "0x43cE8afa6985C86485640c7FEC81bc8FDd66E95f";
            _web3.TransactionManager.Account.Address.Returns(_accountAddress);
            _coreService = Substitute.For<ICoreService>();

            var mockCreator = new MockCreator();
            _indexComposition = mockCreator.GetIndexComposition();
        }

        [Fact]
        public async Task CreateIndexOnChain_should_do_some_dirty_parameter_transformations_to_conform_with_set_js_api()
        {
            var stringTypeEncoder = new StringTypeEncoder();

            var compositionCreator = new IndexCreator(_coreService, _web3, 
                Substitute.For<ILogger<CompositionCreator>>());
            
            var expectedComponents = new []{ _indexComposition.Address }.ToList();
            var expectedQuantities = new[] { new BigInteger(Math.Pow(10, _indexComposition.IndexDefinition.NaturalUnit)) }.ToList();
            var expectedNaturalUnit = new BigInteger(Math.Pow(10, 10));

            var expectedCallData = $"0x000000000000000000000000{_accountAddress.Substring(2)}" +
                                      "0000000000000000000000000000000000000000000000000000000000000000" +
                                      "000000000000000000000000000000000000000000000000000000000024ea00";

            _ = await compositionCreator.SaveIndexOnChain(_indexComposition);

            var receivedCall = _coreService.ReceivedCalls().Single();

            var arguments = receivedCall.GetArguments();
            arguments[0].Should().Be(DeployedContractAddresses.RebalancingSetTokenFactory);
            arguments[1].Should().BeEquivalentTo(expectedComponents);
            arguments[2].Should().BeEquivalentTo(expectedQuantities);
            arguments[3].Should().BeEquivalentTo(expectedNaturalUnit);
            arguments[4].Should().BeEquivalentTo(stringTypeEncoder.EncodePacked(_indexComposition.IndexDefinition.Name));
            arguments[5].Should().BeEquivalentTo(stringTypeEncoder.EncodePacked(_indexComposition.IndexDefinition.Symbol));
            arguments[6].Should().BeEquivalentTo(stringTypeEncoder.EncodePacked(expectedCallData));
        }
    }
}