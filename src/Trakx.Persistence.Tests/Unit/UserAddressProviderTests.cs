using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Trakx.Common.Interfaces;
using Trakx.Persistence.DAO;
using Trakx.Persistence.Tests.Model;
using Trakx.Tests.Data;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Persistence.Tests.Unit
{
    [Collection(nameof(EmptyDbContextCollection))]
    public class UserAddressProviderTests
    {
        private readonly IndiceRepositoryContext _context;
        private readonly IUserAddressProvider _userAddressProvider;
        private readonly MockDaoCreator _mockDaoCreator;
        public UserAddressProviderTests(EmptyDbContextFixture fixture, ITestOutputHelper output)
        {
            _context = fixture.Context;
            _mockDaoCreator = new MockDaoCreator(output);
            _userAddressProvider=new UserAddressProvider(_context);
        }

        [Fact]
        public async Task GetUserAddressByAddress_should_return_correct_UserAddress()
        {
            var userAddressToSave = await SaveUserAddress();

            var retrievedUserAddress = await _userAddressProvider.TryToGetUserAddressByAddress(userAddressToSave.Address);

            retrievedUserAddress.Should().NotBeNull("It exists in the database.");
            retrievedUserAddress.UserId.Should().Be(userAddressToSave.UserId);
            retrievedUserAddress.Id.Should().Be(userAddressToSave.Id);
        }

        [Fact]
        public async Task GetUserAddressByAddress_should_return_null_if_userAddress_doesnt_exist()
        {
            var retrievedUserAddress = await _userAddressProvider.TryToGetUserAddressByAddress(_mockDaoCreator.GetRandomAddressEthereum());
            retrievedUserAddress.Should().BeNull();
        }

        [Fact]
        public async Task ValidateMappingAddress_should_validate_new_mapping()
        {
            var userAddressToSave = await SaveUserAddress();
            userAddressToSave.IsVerified.Should().BeFalse();

            await _userAddressProvider.ValidateMappingAddress(userAddressToSave);

            var retrievedUserAddress =
                await _context.UserAddresses.FirstOrDefaultAsync(u => u.Address == userAddressToSave.Address);
            retrievedUserAddress.IsVerified.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateUserBalance_should_update_the_balance()
        {
            var userAddressToSave = await SaveUserAddress();
            userAddressToSave.Balance.Should().Be(0);

            userAddressToSave.Balance += 20;
            await _userAddressProvider.UpdateUserBalance(userAddressToSave);

            var retrievedUserAddress =
                await _context.UserAddresses.FirstOrDefaultAsync(u => u.Address == userAddressToSave.Address);
            retrievedUserAddress.Balance.Should().Be(20);
        }

        [Fact]
        public async Task AddNewMapping_should_create_new_mapping_if_mapping_not_in_database_yet()
        {
            var newUserAddress = _mockDaoCreator.GetRandomUserAddressDao(5);

            var isSaved = await _userAddressProvider.AddNewMapping(newUserAddress);

            isSaved.Should().BeTrue();
            var retrievedUserAddress =
                await _context.UserAddresses.FirstOrDefaultAsync(u => u.Address == newUserAddress.Address);
            retrievedUserAddress.VerificationAmount.Should().Be(5);
        }

        [Fact]
        public async Task AddNewMapping_should_not_modify_userAddress_if_it_is_already_in_database_and_verified()
        {
            var userAddressToSave = await SaveUserAddress();
            await _userAddressProvider.ValidateMappingAddress(userAddressToSave);

            var isSaved = await _userAddressProvider.AddNewMapping(userAddressToSave);
            isSaved.Should().BeFalse();
        }

        [Fact]
        public async Task AddNewMapping_should_modify_User_Address_if_it_is_not_verified_yet()
        {
            var userAddressToSave = await SaveUserAddress(5);

            userAddressToSave.UserId = "NewName";
            userAddressToSave.VerificationAmount = 50;

            var isModified =await _userAddressProvider.AddNewMapping(userAddressToSave);
            isModified.Should().BeTrue();
            var retrievedUserAddress =
                await _context.UserAddresses.FirstOrDefaultAsync(u => u.Address == userAddressToSave.Address);
            retrievedUserAddress.VerificationAmount.Should().Be(50);
            retrievedUserAddress.UserId.Should().Be("NewName");
        }

        private async Task<UserAddressDao> SaveUserAddress(decimal verificationAmount=0)
        {
            var userAddressToSave = _mockDaoCreator.GetRandomUserAddressDao(verificationAmount);
            await _context.AddAsync(userAddressToSave);
            await _context.SaveChangesAsync();
            return userAddressToSave;
        }
    }
}
