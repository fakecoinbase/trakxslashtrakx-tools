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
    public class ExternalAddressRetrieverTests
    {
        private readonly IndiceRepositoryContext _context;
        private readonly IExternalAddressRetriever _externalAddressRetriever;
        private readonly MockDaoCreator _mockDaoCreator;

        public ExternalAddressRetrieverTests(EmptyDbContextFixture fixture, ITestOutputHelper output)
        {
            _context = fixture.Context;
            _mockDaoCreator = new MockDaoCreator(output);
            _externalAddressRetriever = new ExternalAddressRetriever(_context);
        }

        [Fact]
        public async Task UpdateExternalAddress_should_update_the_ExternalAddress_when_found()
        {
            var externalAddressToSave = await SaveExternalAddress()
                .ConfigureAwait(false);
            var addressToUpdate = new ExternalAddressDao(externalAddressToSave);

            addressToUpdate.Balance += 1;
            addressToUpdate.IsVerified = true;
            var newAddressLastModified = addressToUpdate.LastModified?.AddHours(1);
            addressToUpdate.LastModified = newAddressLastModified;

            var updateSuccessful = await _externalAddressRetriever.UpdateExternalAddress(addressToUpdate);

            updateSuccessful.Should().BeTrue();

            var retrievedUserAddress =
                await _context.ExternalAddresses.FirstOrDefaultAsync(a => a.Id == externalAddressToSave.Id);
            retrievedUserAddress.Balance.Should().Be(addressToUpdate.Balance);
            retrievedUserAddress.LastModified.Should().Be(newAddressLastModified);
        }

        [Fact]
        public async Task UpdateExternalAddress_should_not_update_the_ExternalAddress_when_not_found()
        {
            var unknownExternalAddress = _mockDaoCreator.GetRandomExternalAddressDao();

            var updateSuccessful = await _externalAddressRetriever.UpdateExternalAddress(unknownExternalAddress);

            updateSuccessful.Should().BeFalse();
            (await _context.ExternalAddresses.AnyAsync(a => a.Id == unknownExternalAddress.Id))
                .Should().BeFalse();
        }

        [Fact]
        public async Task AddNewAddress_should_add_new_Address_if_not_in_database()
        {
            var externalAddress = _mockDaoCreator.GetRandomExternalAddressDao();
            var isAdded = await _externalAddressRetriever.AddNewAddress(externalAddress);
            isAdded.Should().BeTrue();
            var retrievedUser = _context.Users.Find(externalAddress.UserDao?.Id);
            retrievedUser.Should().NotBeNull();
            retrievedUser.AddressDaos.Count.Should().BeGreaterOrEqualTo(1);
        }

        [Fact]
        public async Task AddNewAddress_should_not_add_new_address_if_already_in_database()
        {
            var savedExternalAddress = await SaveExternalAddress()
                .ConfigureAwait(false);
            var isAdded = await _externalAddressRetriever.AddNewAddress(savedExternalAddress);
            isAdded.Should().BeFalse();
        }

        [Fact]
        public async Task GetExternalAddressById_should_find_externalAddress_in_database()
        {
            var savedExternalAddress = await SaveExternalAddress()
                .ConfigureAwait(false);
            var retrievedExternalAddress = await
                _externalAddressRetriever.GetExternalAddressById(savedExternalAddress.Id);

            retrievedExternalAddress.Should().NotBeNull();
            retrievedExternalAddress.Address.Should().Be(savedExternalAddress.Address);
        }

        [Fact]
        public async Task GetExternalAddressById_should_return_null_if_object_not_in_database()
        {
            var retrievedExternalAddress = await
                _externalAddressRetriever.GetExternalAddressById(_mockDaoCreator.GetRandomString(10));
            retrievedExternalAddress.Should().BeNull();
        }

        [Fact]
        public async Task AssociateCandidateUser_should_modify_verificationAmount_if_address_not_validate_yet()
        {
            var savedExternalAddress = await SaveExternalAddress()
                .ConfigureAwait(false);
            savedExternalAddress.IsVerified.Should().BeFalse();
            var initialVerificationAmount = savedExternalAddress.VerificationAmount;
            var user = savedExternalAddress.User;

            var updated = await _externalAddressRetriever.AssociateCandidateUser(savedExternalAddress, user, 0)
                .ConfigureAwait(false);

            var retrievedAddress = await _context.ExternalAddresses.FirstAsync(a => a.Id == savedExternalAddress.Id);
            retrievedAddress.VerificationAmount.Should().NotBe(initialVerificationAmount);
            updated.Should().BeTrue();
        }

        [Fact]
        public async Task AssociateCandidateUser_should_override_candidate_if_address_not_verified()
        {
            var savedExternalAddress = await SaveExternalAddress()
                .ConfigureAwait(false);
            savedExternalAddress.IsVerified.Should().BeFalse();
            var previousUserId = savedExternalAddress.User!.Id;
            var user = _mockDaoCreator.GetUserDao();

            var updated = await _externalAddressRetriever.AssociateCandidateUser(savedExternalAddress, user, 0)
                .ConfigureAwait(false);

            var retrievedAddress = await _context.ExternalAddresses.Include(a=>a.UserDao).FirstAsync(a => a.Id == savedExternalAddress.Id);
            retrievedAddress.User!.Id.Should().NotBe(previousUserId);
            updated.Should().BeTrue();
        }

        [Fact]
        public async Task AssociateCandidateUser_should_override_candidate_with_user_in_database_if_address_not_verified()
        {
            var savedExternalAddress = await SaveExternalAddress()
                .ConfigureAwait(false);
            savedExternalAddress.IsVerified.Should().BeFalse();
            var previousUserId = savedExternalAddress.User!.Id;
            var savedUser = _mockDaoCreator.GetUserDao();
            _context.Users.Add(savedUser);
            _context.SaveChanges();

            var updated = await _externalAddressRetriever.AssociateCandidateUser(savedExternalAddress, savedUser, 0)
                .ConfigureAwait(false);

            var retrievedAddress = await _context.ExternalAddresses.Include(a=>a.UserDao).FirstAsync(a => a.Id == savedExternalAddress.Id);
            retrievedAddress.User!.Id.Should().NotBe(previousUserId);
            retrievedAddress.UserDao.AddressDaos.Should().Contain(retrievedAddress);
            retrievedAddress.UserDao.Id.Should().Be(savedUser.Id);
            updated.Should().BeTrue();
        }

        [Fact]
        public async Task AssociateCandidateUser_should_add_external_address_to_database_if_unknown()
        {
            var unknownExternalAddress = _mockDaoCreator.GetRandomExternalAddressDao();

            var updated = await _externalAddressRetriever.AssociateCandidateUser(unknownExternalAddress,
                unknownExternalAddress.User!, 2);

            updated.Should().BeTrue();
            var addedExternalAddress = await _context.ExternalAddresses
                .SingleOrDefaultAsync(a => a.Id == unknownExternalAddress.Id);
            addedExternalAddress.Should().NotBeNull();
            addedExternalAddress.UserDao!.Id.Should().Be(unknownExternalAddress.User!.Id);
        }

        [Fact]
        public async Task AssociateCandidateUser_should_not_update_external_address_if_already_verified()
        {
            var savedExternalAddress = await SaveExternalAddress(true)
                .ConfigureAwait(false);
            var user = _mockDaoCreator.GetUserDao();

            var updated = await _externalAddressRetriever.AssociateCandidateUser(savedExternalAddress,
                user, 0);

            updated.Should().BeFalse();
            var existingAddress = await _context.ExternalAddresses.SingleAsync(a => a.Id == savedExternalAddress.Id);
            existingAddress.UserDao!.Id.Should().NotBe(user.Id);
        }

        [Fact]
        public async Task AssociateCandidateUser_should_add_externalAddress_on_existing_user()
        {
            var savedUser = _mockDaoCreator.GetUserDao();
            savedUser.AddressDaos.Count.Should().Be(0);
            _context.Users.Add(savedUser);
            _context.SaveChanges();
            var externalAddressToAssociate = _mockDaoCreator.GetRandomExternalAddressDao();
            externalAddressToAssociate.UserDao = savedUser;
            var isAdded = await _externalAddressRetriever.AssociateCandidateUser(externalAddressToAssociate, savedUser, 10);
            isAdded.Should().BeTrue();

            var retrievedUser = _context.Users.Find(savedUser.Id);
            retrievedUser.AddressDaos.Count.Should().Be(1);
            var retrievedAddress = _context.ExternalAddresses.Find(externalAddressToAssociate.Id);
            retrievedAddress.UserDao.Id.Should().Be(retrievedUser.Id);
        }

        private async Task<ExternalAddressDao> SaveExternalAddress(bool isVerified = false)
        {
            var externalAddressToSave = _mockDaoCreator.GetRandomExternalAddressDao();
            externalAddressToSave.IsVerified = isVerified;
            await _context.ExternalAddresses.AddAsync(externalAddressToSave);
            await _context.SaveChangesAsync();
            return externalAddressToSave;
        }
    }
}
