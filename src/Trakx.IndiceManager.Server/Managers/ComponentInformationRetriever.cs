using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nethereum.Web3;
using Trakx.Common.Core;
using Trakx.Common.Interfaces.Indice;
using Trakx.Persistence;
using Trakx.Common.Sources.CoinGecko;
using Trakx.Common.Sources.Web3.Client;

namespace Trakx.IndiceManager.Server.Managers
{
    public class ComponentInformationRetriever : IComponentInformationRetriever
    {
        private readonly IndiceRepositoryContext _dbContext;
        private readonly IWeb3Client _web3;
        private readonly ICoinGeckoClient _coinGeckoClient;
        public ComponentInformationRetriever(IndiceRepositoryContext dbContext, IWeb3Client web3, ICoinGeckoClient coinGeckoClient)
        {
            _dbContext = dbContext;
            _web3 = web3;
            _coinGeckoClient = coinGeckoClient;
        }

        public async Task<IComponentDefinition> GetComponentDefinitionFromAddress(string address)
        {
            var result = await _dbContext.ComponentDefinitions.SingleOrDefaultAsync(t => t.Address == address);

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
    }
}