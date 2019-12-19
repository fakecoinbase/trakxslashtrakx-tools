using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Trakx.Data.Models.Index
{
    public class IndexDefinitionProvider : IIndexDefinitionProvider
    {
        private readonly IndexRepositoryContext _dbContext;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<IndexDefinitionProvider> _logger;

        public IndexDefinitionProvider(IndexRepositoryContext dbContext, 
            IMemoryCache memoryCache,
            ILogger<IndexDefinitionProvider> logger)
        {
            _dbContext = dbContext;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task<IndexDefinition> GetDefinitionFromSymbol(string indexSymbol)
        {
            try
            {
                var def = await _memoryCache.GetOrCreateAsync(indexSymbol, async entry =>
                {
                    var definitions = _dbContext.IndexDefinitions
                        .Include(d => d.ComponentDefinitions)
                        .Include(d => d.InitialValuation)
                            .ThenInclude(d => d.ComponentValuations);
                            
                    var result = await definitions.FirstAsync(d => d.Symbol.Equals(indexSymbol));
                    return result;
                });
                if (def != null && def != IndexDefinition.Default) 
                    _memoryCache.GetOrCreate(def.Id, _ => def);

                return def ?? IndexDefinition.Default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve definition for {0}", indexSymbol);
                return IndexDefinition.Default;
            }
        }
    }
}