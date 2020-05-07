using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Trakx.Common.Interfaces;
using Trakx.Common.Interfaces.Indice;
using Trakx.Persistence.DAO;

namespace Trakx.Persistence
{
    public class IndiceDataModifier :IIndiceDataModifier
    {
        private readonly IndiceRepositoryContext _dbContext;

        public IndiceDataModifier(IndiceRepositoryContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> ModifyComposition(IIndiceComposition indiceComposition)
        {
            var indiceCompositionDao = (IndiceCompositionDao) indiceComposition;
            _dbContext.Entry(await _dbContext.IndiceCompositions.FirstOrDefaultAsync(i => i.Id == indiceCompositionDao.Id)).CurrentValues.SetValues(indiceCompositionDao); //modify entities of indiceCompositionDao
            _dbContext.Entry(await _dbContext.IndiceDefinitions.FirstOrDefaultAsync(i => i.Symbol == indiceCompositionDao.IndiceDefinitionDao.Symbol)).CurrentValues.SetValues(indiceCompositionDao.IndiceDefinitionDao); //modify entity of indiceDefinitionDao


            var compositionToSave =
                await _dbContext.IndiceCompositions.FirstOrDefaultAsync(i => i.Id == indiceCompositionDao.Id);

            if (compositionToSave.ComponentQuantityDaos.Count != 0)  //modify entities of ComponentQuantity in database
                indiceCompositionDao.ComponentQuantityDaos.ForEach(c =>
                    _dbContext.Entry(_dbContext.ComponentQuantities.FirstOrDefaultAsync(i => i.Id == c.Id))
                        .CurrentValues.SetValues(c));
            else //create new entities of ComponentQuantityDAO
            {
                compositionToSave.ComponentQuantityDaos = indiceCompositionDao.ComponentQuantityDaos;
            }


            if (compositionToSave.IndiceValuationDaos.Count != 0)
            {
                indiceCompositionDao.IndiceValuationDaos.ForEach(c => _dbContext.Entry(_dbContext.IndiceValuations.FirstOrDefaultAsync(i => i.Id == c.Id))
                    .CurrentValues.SetValues(c));
            }
            else
            {
                compositionToSave.IndiceValuationDaos = indiceCompositionDao.IndiceValuationDaos;
            }

            return await _dbContext.SaveChangesAsync() > 0;
        }


        public async Task<bool> ModifyIndice(IIndiceDefinition indiceDefinition)
        {
            _dbContext.Entry(await _dbContext.IndiceDefinitions.FirstOrDefaultAsync(i => i.Symbol == indiceDefinition.Symbol)).CurrentValues.SetValues(indiceDefinition);
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
