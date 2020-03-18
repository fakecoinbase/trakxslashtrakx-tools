using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Nethereum.ABI.Encoders;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json.Linq;
using NSubstitute;
using Trakx.Contracts.Set;
using Trakx.Contracts.Set.Core;
using Trakx.Data.Common.Ethereum;
using Trakx.Data.Common.Extensions;
using Trakx.Data.Tests.Data;
using Xunit;

namespace Trakx.Data.Tests.Unit.Common.Ethereum
{
    public class CompositionCreatorTests
    {
        private readonly ICoreService _coreService;

        public CompositionCreatorTests()
        {
            _coreService = Substitute.For<ICoreService>();
            var transactionReceipt = new TransactionReceipt()
            {
                TransactionHash = "0x123456789",
                Logs = JArray.Parse(
                "[\r\n" +
                "  {\r\n" +
                "    \"address\": \"0xf55186cc537e7067ea616f2aae007b4427a120c8\",\r\n" +
                "    \"blockHash\": \"0x6c54a6c04c3971e4fb5e4a4c84ee25148ab776a15ff44ce8b79148e4a70ca4a9\",\r\n" +
                "    \"blockNumber\": \"0x93edb6\",\r\n" +
                "    \"data\": \"0x000000000000000000000000e1cd722575800055\",\r\n" +
                "    \"logIndex\": \"0x46\",\r\n" +
                "    \"removed\": false,\r\n" +
                "    \"topics\": [\r\n" +
                "      \"0xa31e381e140096a837a20ba16eb64e32a4011fda0697adbfd7a8f7341c56aa94\",\r\n" +
                "      \"0x000000000000000000000000ae81ae0179b38588e05f404e05882a3965d1b415\"\r\n" +
                "    ],\r\n" +
                "    \"transactionHash\": \"0x2e39c249e929b8d2dcd2560bd33e1ebd17570742972866b46060bc42bf7c4052\",\r\n" +
                "    \"transactionIndex\": \"0x80\"\r\n" +
                "  }" +
                "\r\n]")
            };
            _coreService.CreateSetRequestAndWaitForReceiptAsync(default, 
                    default, default, default, default, default, default)
                .ReturnsForAnyArgs(transactionReceipt);
        }

        [Fact]
        public async Task CreateCompositionOnChain_should_rescale_quantities()
        {
            var stringTypeEncoder = new StringTypeEncoder();

            var compositionCreator = new CompositionCreator(_coreService, Substitute.For<ILogger<CompositionCreator>>());

            var composition = new MockCreator().GetIndexComposition(3);

            var expectedComponents = composition.ComponentQuantities.Select(q => q.ComponentDefinition.Address).ToList();
            var expectedQuantities = composition.ComponentQuantities.Select(q =>
                    new BigInteger(q.Quantity.DescaleComponentQuantity(
                                       q.ComponentDefinition.Decimals, composition.IndexDefinition.NaturalUnit))).ToList();
            var expectedNaturalUnit = composition.IndexDefinition.NaturalUnit.AsAPowerOf10();

            var _ = await compositionCreator.SaveCompositionOnChain(composition);

            var receivedCall = _coreService.ReceivedCalls().Single();

            var arguments = receivedCall.GetArguments();
            arguments[0].Should().Be(DeployedContractAddresses.SetTokenFactory);
            arguments[1].Should().BeEquivalentTo(expectedComponents);
            arguments[2].Should().BeEquivalentTo(expectedQuantities);
            arguments[3].Should().BeEquivalentTo(expectedNaturalUnit);
            arguments[4].Should().BeEquivalentTo(stringTypeEncoder.EncodePacked(composition.IndexDefinition.Name));
            arguments[5].Should().BeEquivalentTo(stringTypeEncoder.EncodePacked(composition.Symbol));
        }
    }
}