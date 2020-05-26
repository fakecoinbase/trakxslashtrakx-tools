using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Trakx.Common.Interfaces;
using Trakx.Common.Interfaces.Indice;
using Trakx.Persistence.DAO;

namespace Trakx.Persistence
{
    public class ComponentDataCreator : IComponentDataCreator
    {
        private readonly IndiceRepositoryContext _dbContext;
        private readonly IComponentDataProvider _componentDataProvider;

        public ComponentDataCreator(IndiceRepositoryContext dbContext)
        {
            _dbContext = dbContext;
            _componentDataProvider=new ComponentDataProvider(_dbContext);
        }

        public async Task<bool> TryAddComponentDefinition(IComponentDefinition componentDefinition)
        {
            if (await _componentDataProvider.GetComponentFromDatabaseByAddress(componentDefinition.Address) != null)
                return false;

            var componentDefinitionDao = new ComponentDefinitionDao(componentDefinition);
            await _dbContext.ComponentDefinitions.AddAsync(componentDefinitionDao);

            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
