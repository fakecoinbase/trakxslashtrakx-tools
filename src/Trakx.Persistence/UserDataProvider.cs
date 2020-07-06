using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Trakx.Common.Interfaces;
using Trakx.Persistence.DAO;

namespace Trakx.Persistence
{
    /// <inheritdoc />
    public class UserDataProvider : IUserDataProvider
    {
        private readonly IndiceRepositoryContext _dbContext;

        public UserDataProvider(IndiceRepositoryContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <inheritdoc />
        public async Task<IUser?> TryGetUserById(string userId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(a => a.Id == userId, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<bool> TryAddNewUser(IUser userToSave, CancellationToken cancellationToken = default)
        {
            var userExists = await _dbContext.Users
                .AnyAsync(u => u.Id == userToSave.Id, cancellationToken);
            
            if (userExists) return false;

            await _dbContext.Users.AddAsync(new UserDao(userToSave), cancellationToken);
            return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
