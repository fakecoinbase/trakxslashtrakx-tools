using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Trakx.Common.Interfaces;
using Trakx.Persistence.DAO;

namespace Trakx.Persistence
{
    public class UserAddressProvider : IUserAddressProvider
    {
        private readonly IndiceRepositoryContext _dbContext;

        public UserAddressProvider(IndiceRepositoryContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IUserAddress?> TryToGetUserAddressByAddress(string address)
        {
            return await _dbContext.UserAddresses.AsNoTracking().FirstOrDefaultAsync(a => a.Address == address);
        }

        public async Task<bool> ValidateMappingAddress(IUserAddress userAddressToValidate)
        {
            var retrievedUser =
                await _dbContext.UserAddresses.FirstOrDefaultAsync(i => i.Address == userAddressToValidate.Address);

            _dbContext.Entry(retrievedUser).Entity.IsVerified = true;
            _dbContext.Entry(retrievedUser).Entity.Balance += retrievedUser.VerificationAmount;
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateUserBalance(IUserAddress userAddress)
        {
            _dbContext.Entry(await _dbContext.UserAddresses.FirstOrDefaultAsync(i => i.Address == userAddress.Address)).CurrentValues.SetValues(userAddress); //modify entity of indiceDefinitionDao
            return await _dbContext.SaveChangesAsync() > 0;
        }
        
        public async Task<bool> AddNewMapping(IUserAddress userAddressToSave)
        {
            var retrievedUser = (UserAddressDao)await TryToGetUserAddressByAddress(userAddressToSave.Address);
            if (retrievedUser != null)
            {
                if (retrievedUser.IsVerified)
                    return false;

                _dbContext.Entry(retrievedUser).Entity.VerificationAmount=userAddressToSave.VerificationAmount;
                _dbContext.Entry(retrievedUser).Entity.UserId = userAddressToSave.UserId;
                return await _dbContext.SaveChangesAsync() > 0;
            }
            await _dbContext.UserAddresses.AddAsync(new UserAddressDao(userAddressToSave));
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
