using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Trakx.Common.Interfaces.Index;
using Trakx.Common.Pricing;

namespace Trakx.Common.Interfaces.Pricing
{ 
    public interface IPriceProvider
    {
        Task<SourcedPrice> GetLatestPrice(IComponentDefinition componentDefinition, string quoteCurrency);
        Task<List<SourcedPrice>> GetLatestPrices(List<IComponentDefinition> componentDefinitions, string quoteCurrency);
        Task<SourcedPrice> GetHistoricalPrice(IComponentDefinition componentDefinition, DateTime asOf, string quoteCurrency);
        Task<List<SourcedPrice>> GetHistoricalPrices(List<IComponentDefinition> componentDefinitions, DateTime asOf, string quoteCurrency);
    }

    public interface ILivePriceCache
    {

    }

    public class PriceProvider : IPriceProvider
    {
        public PriceProvider(IMemoryCache memoryCache)
        {
            
        }

        #region Implementation of IPriceProvider

        /// <inheritdoc />
        public async Task<SourcedPrice> GetLatestPrice(IComponentDefinition componentDefinition, string quoteCurrency)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async Task<List<SourcedPrice>> GetLatestPrices(List<IComponentDefinition> componentDefinitions, string quoteCurrency)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async Task<SourcedPrice> GetHistoricalPrice(IComponentDefinition componentDefinition, DateTime asOf, string quoteCurrency)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async Task<List<SourcedPrice>> GetHistoricalPrices(List<IComponentDefinition> componentDefinitions, DateTime asOf, string quoteCurrency)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}