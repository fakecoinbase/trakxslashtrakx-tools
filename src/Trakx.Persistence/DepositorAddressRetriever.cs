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
        public async Task<bool> UpdateDepositorAddress(IDepositorAddress addressToModify,
            CancellationToken cancellationToken = default)
        {
            var existingDepositorAddress = await _dbContext.DepositorAddresses
                .FirstOrDefaultAsync(i => i.Id == addressToModify.Id, cancellationToken);

            if (existingDepositorAddress == default) return false;

            var addressDao = new DepositorAddressDao(addressToModify);

            if (addressDao.UserDao != null)
            {
                if (existingDepositorAddress.UserDao != null)
                {
                    existingDepositorAddress.UserDao?.AddressDaos.Remove(existingDepositorAddress);
                    _dbContext.Entry(existingDepositorAddress.UserDao).State = EntityState.Modified;
                }
                existingDepositorAddress.UserDao = addressDao.UserDao;
                
                if (await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == addressDao.UserDao.Id,
                    cancellationToken: cancellationToken) != null)
                {
                    _dbContext.Users.Attach(existingDepositorAddress.UserDao);
                }
            }

            _dbContext.Entry(existingDepositorAddress).CurrentValues.SetValues(addressDao);

            return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
        }

        /// <inheritdoc />
        public async Task<bool> AddNewAddress(IDepositorAddress addressToSave,
            CancellationToken cancellationToken = default)
        {
            var addressExists = await _dbContext.DepositorAddresses.AsNoTracking()
                .AnyAsync(a => a.Id == addressToSave.Id, cancellationToken);

            if (addressExists) return false;

            var addressDao = new DepositorAddressDao(addressToSave);

            if (addressDao.UserDao != null)
            {
                addressDao.UserDao?.AddressDaos.Add(addressDao);
                _dbContext.Entry(addressDao.UserDao).State = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == addressDao.UserDao.Id, cancellationToken: cancellationToken) != null ? EntityState.Modified : EntityState.Added;
            }

            _dbContext.Entry(addressDao).State = EntityState.Added;
            return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
        }

        /// <inheritdoc />
        public async Task<IDepositorAddress?> GetDepositorAddressById(string depositorAddressId,
            CancellationToken cancellationToken = default,bool includeUser=false)
        {

            var retrievedDepositorAddress = !includeUser?await _dbContext.DepositorAddresses.AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == depositorAddressId, cancellationToken):await _dbContext.DepositorAddresses.Include(d=>d.UserDao).AsNoTracking()
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
            var existingDepositorAddress = await GetDepositorAddressById(claimedAddress.Id, cancellationToken)
                .ConfigureAwait(false);
            var verificationAmount = _random.Next(1, 100_000) * (decimal)Math.Pow(10, -decimals);
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
