using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Trakx.Common.Interfaces;
using Trakx.Common.Interfaces.Indice;

namespace Trakx.Persistence
{
    public class ComponentDataProvider : IComponentDataProvider
    {
        private readonly IndiceRepositoryContext _dbContext;

        public ComponentDataProvider(IndiceRepositoryContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<IComponentDefinition>> GetAllComponentsFromDatabase(CancellationToken cancellationToken = default)
        {
            var components = await _dbContext.ComponentDefinitions.ToListAsync<IComponentDefinition>();
            return components;
        }

        public async Task<IComponentDefinition> GetComponentFromDatabaseByAddress(string componentAddress, CancellationToken cancellationToken = default)
        {
            var component = await _dbContext.ComponentDefinitions.SingleOrDefaultAsync(t => t.Address == componentAddress);
            return component;
        }
    }
}
