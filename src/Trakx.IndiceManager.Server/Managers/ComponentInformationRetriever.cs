using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trakx.Common.Core;
using Trakx.Common.Interfaces;
using Trakx.Common.Interfaces.Indice;
using Trakx.Common.Models;
using Trakx.Common.Sources.CoinGecko;
using Trakx.Common.Sources.Web3.Client;
using ComponentDetailModel = Trakx.IndiceManager.Server.Models.ComponentDetailModel;

namespace Trakx.IndiceManager.Server.Managers
{
    public class ComponentInformationRetriever : IComponentInformationRetriever
    {
        private readonly IWeb3Client _web3;
        private readonly ICoinGeckoClient _coinGeckoClient;
        private readonly IComponentDataProvider _componentDataProvider;
        private readonly IComponentDataCreator _componentDataCreator;
        public ComponentInformationRetriever(IWeb3Client web3, ICoinGeckoClient coinGeckoClient,IComponentDataProvider componentDataProvider,IComponentDataCreator componentDataCreator)
        {
            _web3 = web3;
            _coinGeckoClient = coinGeckoClient;
            _componentDataProvider = componentDataProvider;
            _componentDataCreator = componentDataCreator;
        }

        public async Task<IComponentDefinition> GetComponentDefinitionFromAddress(string address)
        {
            var result = await _componentDataProvider.GetComponentFromDatabaseByAddress(address);

            if (result!=null)
            {
                return new ComponentDefinition(result.Address,result.Name,result.Symbol,result.CoinGeckoId,result.Decimals);
            }
            try  //search thanks to nethereum and return the correct value
            {
                var decimals = await _web3.GetDecimalsFromContractAddress(address);
                var name = await _web3.GetNameFromContractAddress(address);
                var symbol = await _web3.GetSymbolFromContractAddress(address);
                
                var coinGeckoId = await _coinGeckoClient.GetCoinGeckoIdFromSymbol(symbol);
                return new ComponentDefinition(address,name,symbol,coinGeckoId,(ushort)decimals);
            }
            catch 
            {
                return null;
            }
        }

        public async Task<List<IComponentDefinition>> GetAllComponents()
        {
            var components = await _componentDataProvider.GetAllComponentsFromDatabase();

            return components;
        }

        public async Task<bool> TryToSaveComponentDefinition(ComponentDetailModel componentDefinition)
        {
            var result = await _componentDataCreator.TryAddComponentDefinition(componentDefinition.ConvertToIComponentDefinition());

            return result;
        }
    }
}
