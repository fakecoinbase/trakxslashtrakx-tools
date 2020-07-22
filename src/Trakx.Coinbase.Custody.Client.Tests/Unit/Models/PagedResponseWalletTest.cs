using System;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Trakx.Coinbase.Custody.Client.Models;
using Xunit;

namespace Trakx.Coinbase.Custody.Client.Tests.Unit.Models
{
    public class PagedResponseWalletTest
    {
        [Fact]
        public async Task WalletResponse_can_be_deserialised()
        {
            var sampleResponse = await SampleResponseHelper.GetSampleResponseContent("PagedResponseWallet");

            var deserialised = JsonSerializer.Deserialize<PagedResponse<Wallet>>(sampleResponse);

            var wallet = deserialised.Data[0];
            CheckFirstWalletFromResponse(wallet);
        }

        public static void CheckFirstWalletFromResponse(Wallet wallet)
        {
            wallet.Id.Should().Be("32b34b7f-1555-4701-b017-e4c93ff9eaf7");
            wallet.Name.Should().Be("USDC Wallet");
            wallet.CreatedAt.Should().Be(new DateTimeOffset(2020, 7, 8, 07, 42, 08, 860, TimeSpan.Zero));
            wallet.UpdatedAt.Should().Be(new DateTimeOffset(2020, 7, 8, 07, 42, 09, 84, TimeSpan.Zero));
            wallet.UnscaledBalance.Should().Be(12345600);
            wallet.Balance.Should().Be(12.345600m);
            wallet.UnscaledWithdrawableBalance.Should().Be(12345600);
            wallet.WithdrawableBalance.Should().Be(12.345600m);
            wallet.UnscaledUnvestedBalance.Should().Be(0);
            wallet.UnvestedBalance.Should().Be(0);
            wallet.ColdAddress.Should().Be("0xE68Af24BD324156C773C714E5137A532398B0A15");
            wallet.CurrencySymbol.Should().Be("USDC");
        }
    }
}
