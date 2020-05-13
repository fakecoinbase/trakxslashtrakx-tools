using System.Collections.Generic;
using System.Threading.Tasks;
using Trakx.Common.Interfaces;
using Trakx.Common.Interfaces.Indice;

namespace Trakx.IndiceManager.Server.Managers
{
    /// <inheritdoc />
    public class IndiceInformationRetriever : IIndiceInformationRetriever
    {
        private readonly IIndiceDataProvider _dataProvider;
        
        public IndiceInformationRetriever(IIndiceDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        /// <inheritdoc />
        public async Task<List<IIndiceDefinition>> GetAllIndicesFromDatabase()
        {
            return await _dataProvider.GetAllIndices();
        }


        /// <inheritdoc />
        public async Task<List<IIndiceComposition>?> GetAllCompositionForIndiceFromDatabase(string symbol)
        {
            return await _dataProvider.GetAllCompositionForIndice(symbol);
        }
        

        /// <inheritdoc />
        public async Task<bool> SearchIndiceByAddress(string? indiceAddress)
        {
            return await _dataProvider.TryToGetIndiceByAddress(indiceAddress);
        }
        

        /// <inheritdoc />
        public async Task<bool> SearchCompositionByAddress(string compositionAddress)
        {
            return await _dataProvider.TryToGetCompositionByAddress(compositionAddress);
        }
    }
}
