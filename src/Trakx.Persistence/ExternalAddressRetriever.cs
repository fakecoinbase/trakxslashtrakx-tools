using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Trakx.Common.Interfaces;
using Trakx.Persistence.DAO;

namespace Trakx.Persistence
{
    public class ExternalAddressRetriever : IExternalAddressRetriever
    {
        private readonly IndiceRepositoryContext _dbContext;
        private readonly Random _random;

        public ExternalAddressRetriever(IndiceRepositoryContext dbContext)
        {
            _dbContext = dbContext;
            _random = new Random();
        }

        #region Implementation of IExternalAddressRetriever

        /// <inheritdoc />
        public async Task<bool> UpdateExternalAddress(IExternalAddress addressToModify,
            CancellationToken cancellationToken = default)
        {
            var existingExternalAddress = await _dbContext.ExternalAddresses
                .FirstOrDefaultAsync(i => i.Id == addressToModify.Id, cancellationToken);

            if (existingExternalAddress == default) return false;

            var addressDao = new ExternalAddressDao(addressToModify);

            if (addressDao.UserDao != null)
            {
                if (existingExternalAddress.UserDao != null)
                {
                    existingExternalAddress.UserDao?.AddressDaos.Remove(existingExternalAddress);
                    _dbContext.Entry(existingExternalAddress.UserDao).State = EntityState.Modified;
                }
                existingExternalAddress.UserDao = addressDao.UserDao;
                
                if (await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == addressDao.UserDao.Id,
                    cancellationToken: cancellationToken) != null)
                {
                    _dbContext.Users.Attach(existingExternalAddress.UserDao);
                }
            }

            _dbContext.Entry(existingExternalAddress).CurrentValues.SetValues(addressDao);

            return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
        }

        /// <inheritdoc />
        public async Task<bool> AddNewAddress(IExternalAddress addressToSave,
            CancellationToken cancellationToken = default)
        {
            var addressExists = await _dbContext.ExternalAddresses.AsNoTracking()
                .AnyAsync(a => a.Id == addressToSave.Id, cancellationToken);

            if (addressExists) return false;

            var addressDao = new ExternalAddressDao(addressToSave);

            if (addressDao.UserDao != null)
            {
                addressDao.UserDao?.AddressDaos.Add(addressDao);
                _dbContext.Entry(addressDao.UserDao).State = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == addressDao.UserDao.Id, cancellationToken: cancellationToken) != null ? EntityState.Modified : EntityState.Added;
            }

            _dbContext.Entry(addressDao).State = EntityState.Added;
            return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
        }

        /// <inheritdoc />
        public async Task<IExternalAddress?> GetExternalAddressById(string externalAddressId,bool includeUser=false,
            CancellationToken cancellationToken = default)
        {

            var retrievedExternalAddress = !includeUser?await _dbContext.ExternalAddresses.AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == externalAddressId, cancellationToken):await _dbContext.ExternalAddresses.Include(d=>d.UserDao).AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == externalAddressId, cancellationToken);

            return retrievedExternalAddress;
        }

        /// <inheritdoc />
        public async Task<bool> AssociateCandidateUser(
            IExternalAddress claimedAddress,
            IUser candidate,
            int decimals,
            CancellationToken cancellationToken = default)
        {
            var existingExternalAddress = await GetExternalAddressById(claimedAddress.Id,cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            var verificationAmount = _random.Next(1, 100_000) * (decimal)Math.Pow(10, -decimals);
            var updatedAddress = new ExternalAddressDao(claimedAddress.Address, claimedAddress.CurrencySymbol, 0,
                verificationAmount, false, candidate);

            if (existingExternalAddress == null)
            {
                return await AddNewAddress(updatedAddress, cancellationToken).ConfigureAwait(false);
            }

            if (existingExternalAddress.IsVerified) return false;

            return await UpdateExternalAddress(updatedAddress, cancellationToken).ConfigureAwait(false);
        }

        #endregion
    }
}
