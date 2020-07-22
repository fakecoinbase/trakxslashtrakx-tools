using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Trakx.Common.Interfaces;
using Trakx.Persistence.DAO;
using Trakx.Persistence.Tests.Model;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Persistence.Tests.Unit
{
    [Collection(nameof(EmptyDbContextCollection))]
    public class UserAddressProviderTests
    {
        private readonly IndiceRepositoryContext _context;
        private readonly IUserDataProvider _userDataProvider;
        private readonly MockDaoCreator _mockDaoCreator;

        public UserAddressProviderTests(EmptyDbContextFixture fixture, ITestOutputHelper output)
        {
            _context = fixture.Context;
            _mockDaoCreator = new MockDaoCreator(output);
            _userDataProvider = new UserDataProvider(_context);
        }

        [Fact]
        public async Task TryToGetUserAddressByUserId_should_return_correct_UserAddress()
        {
            var userAddressToSave = await SaveUserAddress();

            var retrievedUserAddress = await _userDataProvider.TryGetUserById(userAddressToSave.Id);

            retrievedUserAddress.Should().NotBeNull("It exists in the database.");
            retrievedUserAddress.Id.Should().Be(userAddressToSave.Id);
            retrievedUserAddress.Created.Should().Be(userAddressToSave.Created);
            retrievedUserAddress.Id.Should().Be(userAddressToSave.Id);
        }

        [Fact]
        public async Task TryToGetUserAddressByUserId_should_return_null_if_userAddress_doesnt_exist()
        {
            var retrievedUserAddress = await _userDataProvider.TryGetUserById(_mockDaoCreator.GetRandomString(8));
            retrievedUserAddress.Should().BeNull();
        }

        [Fact]
        public async Task AddNewMapping_should_create_new_mapping_if_mapping_not_in_database_yet()
        {
            var newUserAddress = _mockDaoCreator.GetUserDao();

            var isSaved = await _userDataProvider.TryAddNewUser(newUserAddress);

            isSaved.Should().BeTrue();
            var retrievedUserAddress =
                await _context.Users.FirstOrDefaultAsync(u => u.Id == newUserAddress.Id);
            retrievedUserAddress.Id.Should().Be(newUserAddress.Id);
        }

        [Fact]
        public async Task AddNewMapping_should_not_add_UserAddress_if_it_is_already_in_database()
        {
            var userAddressToSave = await SaveUserAddress();

            var isModified = await _userDataProvider.TryAddNewUser(userAddressToSave);
            isModified.Should().BeFalse();
        }

        private async Task<UserDao> SaveUserAddress()
        {
            var userAddressToSave = _mockDaoCreator.GetUserDao();
            await _context.AddAsync(userAddressToSave);
            await _context.SaveChangesAsync();
            return userAddressToSave;
        }
    }
}
