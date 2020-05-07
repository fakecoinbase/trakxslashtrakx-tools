using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Trakx.Common.Interfaces;
using Trakx.Common.Interfaces.Indice;
using Trakx.Persistence.DAO;

namespace Trakx.Persistence
{
    public class IndiceDataCreator :IIndiceDataCreator
    {
        private readonly IndiceRepositoryContext _dbContext;

        public IndiceDataCreator(IndiceRepositoryContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddNewIndice(IIndiceDefinition indiceDefinitionDao)
        {
            await _dbContext.IndiceDefinitions.AddAsync((IndiceDefinitionDao)indiceDefinitionDao);
            return await _dbContext.SaveChangesAsync() > 0;
        }


        public async Task<bool> AddNewComposition(IIndiceComposition indiceCompositionDao,IIndiceDefinition? indiceDefinitionDao)
        {
            var indiceComposition = (IndiceCompositionDao)indiceCompositionDao;

            await _dbContext.IndiceCompositions.AddAsync(indiceComposition);

            if (indiceDefinitionDao == null)
            {
                await _dbContext.IndiceDefinitions.AddAsync(indiceComposition.IndiceDefinitionDao);
            }
                
            else
            {
                _dbContext.Entry(await _dbContext.IndiceDefinitions.FirstOrDefaultAsync(i => i.Symbol == indiceComposition.IndiceDefinitionDao.Symbol)).CurrentValues.SetValues(indiceComposition.IndiceDefinitionDao); //modify entity of indiceDefinitionDao
            }

            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
