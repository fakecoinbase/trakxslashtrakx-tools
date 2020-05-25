using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Trakx.Coinbase.Custody.Client.Models;
using Xunit;

namespace Trakx.Coinbase.Custody.Client.Tests.Models
{
    public class AddressResponseTests
    {
        [Fact]
        public async Task AddressResponse_can_be_deserialised()
        {
            var type = typeof(AddressResponse);
            var shortAssemblyName = type.Assembly!.FullName!.Split(",")[0]; 
            var namespaceSuffix = type.Namespace!.Replace(shortAssemblyName+".", "");
            var sampleResponse = await File.ReadAllTextAsync(Path.Combine(namespaceSuffix, $"{type.Name}.json"))
                .ConfigureAwait(false);

            var deserialised = JsonSerializer.Deserialize<PagedResponse<AddressResponse>>(sampleResponse);

            var firstAddress = deserialised.Data[0];
            firstAddress.Address.Should().Be("fake_btc_cold_address");
            firstAddress.State.Should().Be(AddressState.Cold);
            firstAddress.Balance.Should().Be(12);
            firstAddress.BlockchainLink.Should().Be("https://live.blockcypher.com/btc/address/fake_btc_cold_address");
            firstAddress.CreatedAt.Should().Be(new DateTimeOffset(2019, 8, 19, 18, 15, 23, 494, TimeSpan.Zero));
            firstAddress.UpdatedAt.Should().Be(new DateTimeOffset(2019, 8, 19, 18, 15, 23, 520, TimeSpan.Zero));
            firstAddress.Currency.Should().Be("btc");
        }
    }
}
