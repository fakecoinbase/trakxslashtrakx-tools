using System;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.ABI.Encoders;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Trakx.Common.Extensions;
using Trakx.Common.Interfaces.Transaction;
using Trakx.Contracts.Wrapping.WrappedToken;

namespace Trakx.Common.Ethereum
{
    public class WrappingCreator
    {
        private readonly WrappedTokenService _wrappingOperator;

        public WrappingCreator(IWeb3 web3, string symbol)
        {
            _wrappingOperator= new WrappedTokenService(web3,symbol);
        }

        public async Task<TransactionReceipt> MintToken(IWrappingTransaction transaction)
        {
            var stringTypeEncoder = new StringTypeEncoder();
            var amount = new BigInteger(transaction.Amount * (await GetDecimal().ConfigureAwait(false)).AsAPowerOf10() );

            var result = await _wrappingOperator.MintRequestAndWaitForReceiptAsync(transaction.ReceiverAddress, amount, stringTypeEncoder.EncodePacked(""),
                stringTypeEncoder.EncodePacked(""));

            return result;
        }

        public async Task<TransactionReceipt> BurnToken(IWrappingTransaction transaction)
        {
            var stringTypeEncoder = new StringTypeEncoder();
            var amount = new BigInteger(transaction.Amount * (await GetDecimal().ConfigureAwait(false)).AsAPowerOf10() );
            var result = await _wrappingOperator.BurnRequestAndWaitForReceiptAsync(amount, stringTypeEncoder.EncodePacked(""));

            return result;
        }

        public async Task<decimal> TotalSupply()
        {
            var supply = await _wrappingOperator.TotalSupplyQueryAsync();
            return (decimal)supply * (-await GetDecimal().ConfigureAwait(false)).AsAPowerOf10();

        }

        public async Task<int> GetDecimal()
        {
            return Convert.ToInt32(await _wrappingOperator.DecimalsQueryAsync());
        }
    }
}
