using System;
using Trakx.Coinbase.Custody.Client.Models;
using Xunit.Abstractions;

namespace Trakx.Coinbase.Custody.Client.Tests
{
    public class MockCreator : Trakx.Tests.Helpers.MockCreator
    {
        /// <inheritdoc />
        public MockCreator(ITestOutputHelper output) : base(output) {}

        public Wallet GetRandomWallet()
        {
            var decimals = GetRandomDecimals();
            var unscaledAmount = GetRandomUnscaledAmount();
            var scaledAmount = unscaledAmount / (decimal)Math.Pow(10, decimals);
            return new Wallet
            {
                UnscaledBalance = unscaledAmount,
                ColdAddress = GetRandomAddressEthereum(),
                CreatedAt = DateTimeOffset.Now,
                CurrencySymbol = GetRandomString(3),
                Name = "name " + GetRandomString(24),
                Balance = scaledAmount,
                Id = Guid.NewGuid().ToString(),
                UpdatedAt = GetRandomUtcDateTimeOffset(),
            };
        }

        public Currency GetRandomCurrency(string? symbol = default)
        {
            return new Currency
            {
                Decimals = GetRandomDecimals(),
                Name = "name " + GetRandomString(24),
                Symbol = symbol ?? GetRandomString(3)
            };
        }
    }
}
