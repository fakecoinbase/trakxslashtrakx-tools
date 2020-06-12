using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Nethereum.RPC.Eth.DTOs;
using Trakx.Common.Core;
using Trakx.Common.Ethereum;
using Trakx.Common.Interfaces.Transaction;
using Trakx.Tests.Tools;
using Xunit;

namespace Trakx.Tests.Integration.Common.Ethereum
{
    public class WrappingCreatorTest
    {
        private readonly WrappingCreator _wrappingCreator;

        public WrappingCreatorTest()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddEthereumInteraction(Secrets.InfuraApiKey,Secrets.EthereumWalletSecret,"rinkeby");

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var web3 = serviceProvider.GetRequiredService<Nethereum.Web3.IWeb3>();
            _wrappingCreator = new WrappingCreator(web3,"wxrp");

        }

        
        [Fact(Skip = "Should have private key and infura key.")]
        public async Task MintToken_should_mint_token_and_return_TransactionHash()
        {
            var transaction = new WrappingTransaction(DateTime.Now, null,null,TransactionState.Pending,null,null,null,null,.20m,null, "0x9d9fFD857c0B1908C961D2FB7E5a4fc5871FFCE1", null,TransactionType.Wrap);

            var result = await _wrappingCreator.MintToken(transaction);

            VerifyTransactionReceipt(result);
        }

        [Fact(Skip = "Should have private key and infura key.")]
        public async Task BurnToken_should_burn_token_and_return_TransactionHash()
        {
            //You should have tokens in your account before burning.
            //To do that, call the MintTokenTest function with your public key in receiverAddress parameter, you will receive tokens to do this test (if you're minter)
            var transaction = new WrappingTransaction(DateTime.Now, null, null, TransactionState.Pending, null, null, null, null, 1.2456m,null, "0x191FC2305d6C98291E68a5bD23908D9036Aa0175", null,TransactionType.Wrap);

            var result = await _wrappingCreator.BurnToken(transaction);

            VerifyTransactionReceipt(result);
        }

        [Fact(Skip = "Should have private key and infura key.")]
        public async Task TotalSupply_should_return_correct_amount()
        {
            var initialTotalSupply = await _wrappingCreator.TotalSupply();

            var mintTransaction = new WrappingTransaction(DateTime.Now, null, null, TransactionState.Pending, null, null, null, null, 2.20m, null, "0x191FC2305d6C98291E68a5bD23908D9036Aa0175", null,TransactionType.Wrap);
            await _wrappingCreator.MintToken(mintTransaction);

            var expectedFinalTotalSupply = initialTotalSupply + mintTransaction.Amount;
            var finalTotalSupply = await _wrappingCreator.TotalSupply();

            finalTotalSupply.Should().Be(expectedFinalTotalSupply);
        }

        private static void VerifyTransactionReceipt(TransactionReceipt result)
        {
            result.Status.Value.Should().Be(1);
            result.TransactionHash.Should().NotBeNullOrEmpty();
            result.BlockNumber.Value.Should().NotBeNull();
        }

    }
}
