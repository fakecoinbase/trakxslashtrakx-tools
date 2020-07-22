using System;
using FluentAssertions;
using Trakx.Coinbase.Custody.Client.Models;
using Xunit;

namespace Trakx.Coinbase.Custody.Client.Tests.Unit.Models
{
    public class CoinbaseTransactionTests
    {
        [Theory]
        [InlineData(23948, 2)]
        [InlineData(23948, 0)]
        [InlineData(23948, 18)]
        public void CoinbaseTransaction_should_have_correct_scaled_amount(ulong rawAmount, ushort decimals)
        {
            var coinbaseRawTransaction = new CoinbaseRawTransaction {UnscaledAmount = rawAmount};
            var scaledTransaction = new CoinbaseTransaction(coinbaseRawTransaction, decimals);

            scaledTransaction.ScaledAmount.Should().Be(rawAmount * (decimal) Math.Pow(10, -decimals));
        }
    }
}