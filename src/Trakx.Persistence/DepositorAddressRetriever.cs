using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Trakx.Common.Interfaces;
using Trakx.Persistence.DAO;

namespace Trakx.Persistence
{
    public class DepositorAddressRetriever : IDepositorAddressRetriever
    {
        private readonly IndiceRepositoryContext _dbContext;
        private readonly Random _random;

        public DepositorAddressRetriever(IndiceRepositoryContext dbContext)
        {
            _dbContext = dbContext;
            _random = new Random();
        }

        #region Implementation of IDepositorAddressRetriever

        /// <inheritdoc />
        public async Task<bool> UpdateDepositorAddress(IDepositorAddress address,
            CancellationToken cancellationToken = default)
        {
            var existingDepositorAddress = await _dbContext.DepositorAddresses
                .FirstOrDefaultAsync(i => i.Id == address.Id, cancellationToken);

            if (existingDepositorAddress == default) return false;

            _dbContext.Entry(existingDepositorAddress).CurrentValues.SetValues(address);
            existingDepositorAddress.UserDao =
                address.User == null ? null : address.User as UserDao ?? new UserDao(address.User);
            return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
        }

        /// <inheritdoc />
        public async Task<bool> AddNewAddress(IDepositorAddress addressToSave,
            CancellationToken cancellationToken = default)
        {
            var addressExists = await _dbContext.DepositorAddresses
                .AnyAsync(a => a.Id == addressToSave.Id, cancellationToken);

            if (addressExists) return false;

            await _dbContext.DepositorAddresses.AddAsync(new DepositorAddressDao(addressToSave), cancellationToken);
            return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
        }

        /// <inheritdoc />
        public async Task<IDepositorAddress?> GetDepositorAddressById(string depositorAddressId,
            CancellationToken cancellationToken = default)
        {
            var retrievedDepositorAddress = await _dbContext.DepositorAddresses
                .FirstOrDefaultAsync(d => d.Id == depositorAddressId, cancellationToken);
            return retrievedDepositorAddress;
        }

        /// <inheritdoc />
        public async Task<bool> AssociateCandidateUser(
            IDepositorAddress claimedAddress,
            IUser candidate,
            int decimals,
            CancellationToken cancellationToken = default)
        {
            await AddNewAddress(claimedAddress, cancellationToken).ConfigureAwait(false);
            
            var existingDepositorAddress = await GetDepositorAddressById(claimedAddress.Id, cancellationToken)
                .ConfigureAwait(false);
            var verificationAmount = _random.Next(1, 100_000) *(decimal)Math.Pow(10, -decimals);
            var updatedAddress = new DepositorAddressDao(claimedAddress.Address, claimedAddress.CurrencySymbol, 0,
                verificationAmount, false, candidate);
            
            if (existingDepositorAddress == null)
            {
                return await AddNewAddress(updatedAddress, cancellationToken).ConfigureAwait(false);
            }

            if (existingDepositorAddress.IsVerified) return false;

            return await UpdateDepositorAddress(updatedAddress, cancellationToken).ConfigureAwait(false);
        }

        #endregion
    }
}
