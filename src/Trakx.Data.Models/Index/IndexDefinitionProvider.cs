using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Trakx.Data.Models.Index
{
    public class IndexDefinitionProvider : IIndexDefinitionProvider
    {
        private readonly IndexRepositoryContext _dbContext;
        private readonly ILogger<IndexDefinitionProvider> _logger;

        public IndexDefinitionProvider(IndexRepositoryContext dbContext, ILogger<IndexDefinitionProvider> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IndexDefinition> GetDefinitionFromSymbol(string indexSymbol)
        {
            try
            {
                var definition = await _dbContext.IndexDefinitions.FindAsync(indexSymbol.ToLower());
                return definition;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve definition for {0}", indexSymbol);
                return IndexDefinition.Default;
            }
        }
    }
}