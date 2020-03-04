using System.Linq;
using FluentAssertions;
using Trakx.Contracts.Set;
using Xunit;

namespace Trakx.Data.Tests.Unit.Contracts
{
    public class DeployedContractAddressesTests
    {
        [Fact]
        public void AddressByName_should_be_defined()
        {
            DeployedContractAddresses.AddressByName.Count.Should().Be(25);
            DeployedContractAddresses.AddressByName.ToList().ForEach(v =>
            {
                v.Value.Should().StartWith("0x", $"Address for {v.Key} should start with '0x'.");
                v.Value.Length.Should().Be(42, because: $"Address for {v.Key} should be 42 characters long.");
            });

            DeployedContractAddresses.AddressByName.Should()
                .ContainKey(nameof(DeployedContractAddresses.SetTokenFactory));
        }
    }
}