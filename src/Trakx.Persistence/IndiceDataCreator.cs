using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Trakx.Common.Interfaces;
using Trakx.Common.Interfaces.Indice;
using Trakx.Persistence.DAO;

namespace Trakx.Persistence
{
    public class IndiceDataCreator : IIndiceDataCreator
    {
        private readonly IndiceRepositoryContext _dbContext;

        public IndiceDataCreator(IndiceRepositoryContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddNewIndice(IIndiceDefinition indiceDefinition)
        {
            await _dbContext.IndiceDefinitions.AddAsync((IndiceDefinitionDao)indiceDefinition);
            return await _dbContext.SaveChangesAsync() > 0;
        }


        public async Task<bool> AddNewComposition(IIndiceComposition indiceComposition)
        {
            if (indiceComposition.IndiceDefinition == null) return false;
            var definitionDao = await _dbContext.IndiceDefinitions.FindAsync(new[] {indiceComposition.IndiceDefinition.Symbol});

            if (definitionDao == default)
            {
                definitionDao = new IndiceDefinitionDao(indiceComposition.IndiceDefinition);
            }
            else
            {
                _dbContext.IndiceDefinitions.Attach(definitionDao);
            }
            
            var indiceCompositionDao = new IndiceCompositionDao(indiceComposition)
            {
                IndiceDefinitionDao = definitionDao
            };

            indiceCompositionDao.ComponentQuantityDaos.ForEach(o=>_dbContext.ComponentDefinitions.Attach(o.ComponentDefinitionDao));
            await _dbContext.IndiceCompositions.AddAsync(indiceCompositionDao);
        
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
