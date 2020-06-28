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
            var indiceCompositionDao = new IndiceCompositionDao(indiceComposition);
            
            indiceCompositionDao.ComponentQuantityDaos.ForEach(o=>_dbContext.ComponentDefinitions.Attach(o.ComponentDefinitionDao));
            await _dbContext.IndiceCompositions.AddAsync(indiceCompositionDao);
        
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
