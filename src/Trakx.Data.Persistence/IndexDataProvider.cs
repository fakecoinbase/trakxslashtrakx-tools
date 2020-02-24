using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Trakx.Data.Common.Interfaces;
using Trakx.Data.Common.Interfaces.Index;
using Trakx.Data.Common.Pricing;

namespace Trakx.Data.Persistence
{
    public class IndexDataProvider : IIndexDataProvider
    {
        private readonly IndexRepositoryContext _dbContext;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<IndexDataProvider> _logger;

        private const string CurrentCompositionCacheKeyTemplate = "{0}|current";

        public IndexDataProvider(IndexRepositoryContext dbContext, 
            IMemoryCache memoryCache,
            ILogger<IndexDataProvider> logger)
        {
            _dbContext = dbContext;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        [Obsolete("Now we should only use the composition of the index to get the price.")]
        public async Task<IIndexDefinition?> GetDefinitionFromSymbol(string indexSymbol, CancellationToken cancellationToken = default)
        {
            try
            {
                var def = await _memoryCache.GetOrCreateAsync(indexSymbol, async entry =>
                {
                    var definitions = _dbContext.IndexDefinitions;
                            
                    var result = await definitions.FirstAsync(d => d.Symbol.Equals(indexSymbol), cancellationToken);
                    return result;
                });

                if (def == default) return default;

                _memoryCache.GetOrCreate(def.Address, _ => def);
                return def;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve definition for {0}", indexSymbol);
                return default;
            }
        }

        /// <inheritdoc />
        public async Task<IIndexComposition?> GetCompositionAtDate(string indexSymbol, DateTime asOfUtc,
            CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = $"{indexSymbol}|asOf|{asOfUtc:yyMMddHHmmss}";
                var def = await _memoryCache.GetOrCreateAsync(cacheKey, async entry =>
                {
                    entry.SetSlidingExpiration(TimeSpan.FromSeconds(10));
                    
                    var version = await GetVersionAtDate(indexSymbol, asOfUtc, cancellationToken);
                    var composition = await RetrieveFullComposition(indexSymbol, version.Value, cancellationToken);

                    entry.AbsoluteExpirationRelativeToNow = composition != null && composition.IsValid() 
                        ? TimeSpan.FromSeconds(100)
                        : TimeSpan.FromTicks(1);
                    return composition;
                });

                return def;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve composition for {0} at date {1}", indexSymbol, asOfUtc);
                return default;
            }
        }

        public string GetCacheKeyForCurrentComposition(string indexSymbol)
        {
            return string.Format(CurrentCompositionCacheKeyTemplate, indexSymbol);
        }

        public async Task<string?> CacheCurrentComposition(string indexSymbol, CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = GetCacheKeyForCurrentComposition(indexSymbol);
                await _memoryCache.GetOrCreateAsync(cacheKey, async entry =>
                {
                    entry.SetSlidingExpiration(TimeSpan.FromSeconds(20));

                    var version = await GetVersionAtDate(indexSymbol, DateTime.UtcNow, cancellationToken);
                    var composition = await RetrieveFullComposition(indexSymbol, version.Value, cancellationToken);
                    return composition;
                });

                return cacheKey;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve current composition for {0}", indexSymbol);
                return default;
            }
        }

        /// <inheritdoc />
        public async Task<uint?> GetVersionAtDate(string indexSymbol, DateTime asOfUtc, CancellationToken cancellationToken = default)
        {
            var compositions = _dbContext.IndexCompositions;
            var result = await compositions
                .Include(c => c.IndexDefinitionDao)
                //.Include(c => c.ComponentQuantityDaos)
                .Where(c => c.IndexDefinitionDao.Symbol == indexSymbol)
                .OrderByDescending(c => c.CreationDate)
                .FirstOrDefaultAsync(c => c.CreationDate <= asOfUtc, cancellationToken: cancellationToken);
            return result?.Version;
        }

        /// <inheritdoc />
        public async Task<IIndexComposition?> GetCurrentComposition(string indexSymbol, CancellationToken cancellationToken = default)
        {
            return await GetCompositionAtDate(indexSymbol, DateTime.UtcNow, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<uint?> GetCurrentVersion(string indexSymbol, CancellationToken cancellationToken = default)
        {
            return await GetVersionAtDate(indexSymbol, DateTime.UtcNow, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IIndexValuation> GetInitialValuation(IIndexComposition composition, 
            string quoteCurrency = Constants.DefaultQuoteCurrency,
            CancellationToken cancellationToken = default)
        {
            var issueDate = composition.CreationDate;
            var valuation = _dbContext.IndexValuations
                .Include(i => i.ComponentValuationDaos)
                .Include(i => i.IndexCompositionDao)
                .ThenInclude(c => c.ComponentQuantityDaos)
                .Include(i => i.IndexCompositionDao)
                .ThenInclude(c => c.IndexDefinitionDao)
                .ThenInclude(d => d.ComponentWeightDaos)
                .ThenInclude(c => c.ComponentDefinitionDao)
                .Where(c => c.IndexCompositionDao.Id == $"{composition.IndexDefinition.Symbol}|{composition.Version}"
                                      && c.QuoteCurrency == quoteCurrency
                                      && c.TimeStamp == issueDate);
            await valuation.LoadAsync(cancellationToken);
            return valuation.SingleOrDefault();
        }

        private async Task<IIndexComposition> RetrieveFullComposition(string indexSymbol, uint version, 
            CancellationToken cancellationToken = default)
        {
            var composition = await _dbContext.IndexCompositions
                .Include(c => c.ComponentQuantityDaos)
                .ThenInclude(c => c.ComponentDefinitionDao)
                .Include(c => c.IndexDefinitionDao)
                .ThenInclude(c => c.ComponentWeightDaos)
                .SingleOrDefaultAsync(c => c.IndexDefinitionDao.Symbol == indexSymbol 
                                           && c.Version == version, cancellationToken);
            return composition;
        }
    }
}