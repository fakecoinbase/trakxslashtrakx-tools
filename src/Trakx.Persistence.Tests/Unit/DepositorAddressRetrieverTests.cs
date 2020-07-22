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
    public class DepositorAddressRetrieverTests
    {
        private readonly IndiceRepositoryContext _context;
        private readonly IDepositorAddressRetriever _depositorAddressRetriever;
        private readonly MockDaoCreator _mockDaoCreator;

        public DepositorAddressRetrieverTests(EmptyDbContextFixture fixture, ITestOutputHelper output)
        {
            _context = fixture.Context;
            _mockDaoCreator = new MockDaoCreator(output);
            _depositorAddressRetriever = new DepositorAddressRetriever(_context);
        }

        [Fact]
        public async Task UpdateDepositorAddress_should_update_the_DepositAddress_when_found()
        {
            var depositorAddressToSave = await SaveDepositorAddress()
                .ConfigureAwait(false);
            var addressToUpdate = new DepositorAddressDao(depositorAddressToSave);

            addressToUpdate.Balance += 1;
            addressToUpdate.IsVerified = true;
            var newAddressLastModified = addressToUpdate.LastModified?.AddHours(1);
            addressToUpdate.LastModified = newAddressLastModified;

            var updateSuccessful = await _depositorAddressRetriever.UpdateDepositorAddress(addressToUpdate);

            updateSuccessful.Should().BeTrue();

            var retrievedUserAddress =
                await _context.DepositorAddresses.FirstOrDefaultAsync(a => a.Id == depositorAddressToSave.Id);
            retrievedUserAddress.Balance.Should().Be(addressToUpdate.Balance);
            retrievedUserAddress.LastModified.Should().Be(newAddressLastModified);
        }

        [Fact]
        public async Task UpdateDepositorAddress_should_not_update_the_DepositAddress_when_not_found()
        {
            var unknownDepositorAddress = _mockDaoCreator.GetRandomDepositorAddressDao();

            var updateSuccessful = await _depositorAddressRetriever.UpdateDepositorAddress(unknownDepositorAddress);

            updateSuccessful.Should().BeFalse();
            (await _context.DepositorAddresses.AnyAsync(a => a.Id == unknownDepositorAddress.Id))
                .Should().BeFalse();
        }

        [Fact]
        public async Task AddNewAddress_should_add_new_Address_if_not_in_database()
        {
            var depositorAddress = _mockDaoCreator.GetRandomDepositorAddressDao();
            var isAdded = await _depositorAddressRetriever.AddNewAddress(depositorAddress);
            isAdded.Should().BeTrue();
            var retrievedUser = _context.Users.Find(depositorAddress.UserDao?.Id);
            retrievedUser.Should().NotBeNull();
            retrievedUser.AddressDaos.Count.Should().BeGreaterOrEqualTo(1);
        }

        [Fact]
        public async Task AddNewAddress_should_not_add_new_address_if_already_in_database()
        {
            var savedDepositorAddress = await SaveDepositorAddress()
                .ConfigureAwait(false);
            var isAdded = await _depositorAddressRetriever.AddNewAddress(savedDepositorAddress);
            isAdded.Should().BeFalse();
        }

        [Fact]
        public async Task GetDepositorAddressById_should_find_depositorAddress_in_database()
        {
            var savedDepositorAddress = await SaveDepositorAddress()
                .ConfigureAwait(false);
            var retrievedDepositorAddress = await
                _depositorAddressRetriever.GetDepositorAddressById(savedDepositorAddress.Id);

            retrievedDepositorAddress.Should().NotBeNull();
            retrievedDepositorAddress.Address.Should().Be(savedDepositorAddress.Address);
        }

        [Fact]
        public async Task GetDepositorAddressById_should_return_null_if_object_not_in_database()
        {
            var retrievedDepositorAddress = await
                _depositorAddressRetriever.GetDepositorAddressById(_mockDaoCreator.GetRandomString(10));
            retrievedDepositorAddress.Should().BeNull();
        }

        [Fact]
        public async Task AssociateCandidateUser_should_modify_verificationAmount_if_address_not_validate_yet()
        {
            var savedDepositorAddress = await SaveDepositorAddress()
                .ConfigureAwait(false);
            savedDepositorAddress.IsVerified.Should().BeFalse();
            var initialVerificationAmount = savedDepositorAddress.VerificationAmount;
            var user = savedDepositorAddress.User;

            var updated = await _depositorAddressRetriever.AssociateCandidateUser(savedDepositorAddress, user, 0)
                .ConfigureAwait(false);

            var retrievedAddress = await _context.DepositorAddresses.FirstAsync(a => a.Id == savedDepositorAddress.Id);
            retrievedAddress.VerificationAmount.Should().NotBe(initialVerificationAmount);
            updated.Should().BeTrue();
        }

        [Fact]
        public async Task AssociateCandidateUser_should_override_candidate_if_address_not_verified()
        {
            var savedDepositorAddress = await SaveDepositorAddress()
                .ConfigureAwait(false);
            savedDepositorAddress.IsVerified.Should().BeFalse();
            var previousUserId = savedDepositorAddress.User!.Id;
            var user = _mockDaoCreator.GetUserDao();

            var updated = await _depositorAddressRetriever.AssociateCandidateUser(savedDepositorAddress, user, 0)
                .ConfigureAwait(false);

            var retrievedAddress = await _context.DepositorAddresses.Include(a=>a.UserDao).FirstAsync(a => a.Id == savedDepositorAddress.Id);
            retrievedAddress.User!.Id.Should().NotBe(previousUserId);
            updated.Should().BeTrue();
        }

        [Fact]
        public async Task AssociateCandidateUser_should_override_candidate_with_user_in_database_if_address_not_verified()
        {
            var savedDepositorAddress = await SaveDepositorAddress()
                .ConfigureAwait(false);
            savedDepositorAddress.IsVerified.Should().BeFalse();
            var previousUserId = savedDepositorAddress.User!.Id;
            var savedUser = _mockDaoCreator.GetUserDao();
            _context.Users.Add(savedUser);
            _context.SaveChanges();

            var updated = await _depositorAddressRetriever.AssociateCandidateUser(savedDepositorAddress, savedUser, 0)
                .ConfigureAwait(false);

            var retrievedAddress = await _context.DepositorAddresses.Include(a=>a.UserDao).FirstAsync(a => a.Id == savedDepositorAddress.Id);
            retrievedAddress.User!.Id.Should().NotBe(previousUserId);
            retrievedAddress.UserDao.AddressDaos.Should().Contain(retrievedAddress);
            retrievedAddress.UserDao.Id.Should().Be(savedUser.Id);
            updated.Should().BeTrue();
        }

        [Fact]
        public async Task AssociateCandidateUser_should_add_depositor_address_to_database_if_unknown()
        {
            var unknownDepositorAddress = _mockDaoCreator.GetRandomDepositorAddressDao();

            var updated = await _depositorAddressRetriever.AssociateCandidateUser(unknownDepositorAddress,
                unknownDepositorAddress.User!, 2);

            updated.Should().BeTrue();
            var addedDepositorAddress = await _context.DepositorAddresses
                .SingleOrDefaultAsync(a => a.Id == unknownDepositorAddress.Id);
            addedDepositorAddress.Should().NotBeNull();
            addedDepositorAddress.UserDao!.Id.Should().Be(unknownDepositorAddress.User!.Id);
        }

        [Fact]
        public async Task AssociateCandidateUser_should_not_update_depositor_address_if_already_verified()
        {
            var savedDepositorAddress = await SaveDepositorAddress(true)
                .ConfigureAwait(false);
            var user = _mockDaoCreator.GetUserDao();

            var updated = await _depositorAddressRetriever.AssociateCandidateUser(savedDepositorAddress,
                user, 0);

            updated.Should().BeFalse();
            var existingAddress = await _context.DepositorAddresses.SingleAsync(a => a.Id == savedDepositorAddress.Id);
            existingAddress.UserDao!.Id.Should().NotBe(user.Id);
        }

        [Fact]
        public async Task AssociateCandidateUser_should_add_depositorAddress_on_existing_user()
        {
            var savedUser = _mockDaoCreator.GetUserDao();
            savedUser.AddressDaos.Count.Should().Be(0);
            _context.Users.Add(savedUser);
            _context.SaveChanges();
            var depositorAddressToAssociate = _mockDaoCreator.GetRandomDepositorAddressDao();
            depositorAddressToAssociate.UserDao = savedUser;
            var isAdded = await _depositorAddressRetriever.AssociateCandidateUser(depositorAddressToAssociate, savedUser, 10);
            isAdded.Should().BeTrue();

            var retrievedUser = _context.Users.Find(savedUser.Id);
            retrievedUser.AddressDaos.Count.Should().Be(1);
            var retrievedAddress = _context.DepositorAddresses.Find(depositorAddressToAssociate.Id);
            retrievedAddress.UserDao.Id.Should().Be(retrievedUser.Id);
        }

        private async Task<DepositorAddressDao> SaveDepositorAddress(bool isVerified = false)
        {
            var depositorAddressToSave = _mockDaoCreator.GetRandomDepositorAddressDao();
            depositorAddressToSave.IsVerified = isVerified;
            await _context.DepositorAddresses.AddAsync(depositorAddressToSave);
            await _context.SaveChangesAsync();
            return depositorAddressToSave;
        }
    }
}
