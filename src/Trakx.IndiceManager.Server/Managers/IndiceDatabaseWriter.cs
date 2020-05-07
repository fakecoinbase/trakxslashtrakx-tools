using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Trakx.Common.Interfaces;
using Trakx.IndiceManager.Server.Models;
using Trakx.Persistence;
using Trakx.Persistence.DAO;

namespace Trakx.IndiceManager.Server.Managers
{
    public class IndiceDatabaseWriter :IIndiceDatabaseWriter
    {
        private readonly IndiceRepositoryContext _dbContext;
        private readonly IIndiceDataModifier _indiceDataModifier;
        private readonly IIndiceDataCreator _indiceDataCreator;

        public IndiceDatabaseWriter(IndiceRepositoryContext dbContext,IIndiceDataModifier indiceDataModifier,IIndiceDataCreator indiceDataCreator)
        {
            _dbContext = dbContext;
            _indiceDataCreator = indiceDataCreator;
            _indiceDataModifier = indiceDataModifier;
        }

        /// <inheritdoc />
        public async Task<bool> TrySaveComposition(IndiceCompositionModel indiceCompositionModel)
        {
            if (indiceCompositionModel.IndiceDetail == null)
                return false;
            
            var indiceDefinitionInDatabase =
                await _dbContext.IndiceDefinitions.FirstOrDefaultAsync(i =>
                    i.Symbol == indiceCompositionModel.IndiceDetail.Symbol);
            
            var indiceComposition = indiceCompositionModel.FromModelToIndiceComposition(indiceDefinitionInDatabase);

            
            if (await _dbContext.IndiceCompositions.FirstOrDefaultAsync(c =>
                c.Id == $"{indiceComposition.IndiceDefinition.Symbol}|{indiceComposition.Version}" && c.Address == null) != null)
                return await _indiceDataModifier.ModifyComposition(indiceComposition).ConfigureAwait(false);


            if (await _dbContext.IndiceCompositions.FirstOrDefaultAsync(c =>
                c.Id == $"{indiceComposition.IndiceDefinition.Symbol}|{indiceComposition.Version}") == null)
                return await _indiceDataCreator.AddNewComposition(indiceComposition,indiceDefinitionInDatabase).ConfigureAwait(false);

            return false;
        }

        /// <inheritdoc />
        public async Task<bool> TrySaveIndice(IndiceDetailModel indice)
        {
            var indiceDao = new IndiceDefinitionDao(indice.Symbol, indice.Name, indice.Description, indice.NaturalUnit, indice.Address, indice.CreationDate);

            if (await _dbContext.IndiceDefinitions.FirstOrDefaultAsync(i =>
                i.Symbol == indice.Symbol && i.Address == null) != null)
                return await _indiceDataModifier.ModifyIndice(indiceDao).ConfigureAwait(false);

            if (await _dbContext.IndiceDefinitions.FirstOrDefaultAsync(i => i.Symbol == indice.Symbol) == null)
                return await _indiceDataCreator.AddNewIndice(indiceDao).ConfigureAwait(false);

            return false;
        }
    }
}
