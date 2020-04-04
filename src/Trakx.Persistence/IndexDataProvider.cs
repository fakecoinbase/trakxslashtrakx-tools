using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Trakx.Common.Interfaces;
using Trakx.Common.Interfaces.Index;

namespace Trakx.Persistence
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

        /// <inheritdoc />
        public async Task<List<string>> GetAllIndexSymbols(CancellationToken cancellationToken = default)
        {
            var indexSymbols = await _dbContext.IndexDefinitions.AsNoTracking().Select(i => i.Symbol)
                .ToListAsync(cancellationToken);
            return indexSymbols;
        }

        /// <inheritdoc />
        public async Task<List<string>> GetCompositionSymbolsFromIndex(string indexSymbol, CancellationToken cancellationToken = default)
        {
            var indexDefinition = await _dbContext.IndexDefinitions
                .Include(i => i.IndexCompositionDaos)
                .SingleAsync(i => i.Symbol.Equals(indexSymbol, StringComparison.InvariantCultureIgnoreCase), 
                    cancellationToken);

            var symbols = indexDefinition.IndexCompositionDaos.Select(c => c.Symbol).ToList();
            return symbols;
        }

        /// <inheritdoc />
        public async Task<IIndexComposition?> GetCompositionAtDate(string indexSymbol, DateTime asOfUtc,
            CancellationToken cancellationToken = default)
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

        /// <inheritdoc />
        public async Task<IIndexComposition?> GetCompositionFromSymbol(string compositionSymbol, CancellationToken cancellationToken = default)
        {
            try
            {
                var cacheKey = $"{compositionSymbol}";
                var def = await _memoryCache.GetOrCreateAsync(cacheKey, async entry =>
                {
                    entry.SetSlidingExpiration(TimeSpan.FromSeconds(10));

                    var composition = await RetrieveFullComposition(compositionSymbol, cancellationToken);

                    entry.AbsoluteExpirationRelativeToNow = composition != null && composition.IsValid()
                        ? TimeSpan.FromSeconds(100)
                        : TimeSpan.FromTicks(1);
                    return composition;
                });

                return def;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve composition with symbol {0}", compositionSymbol);
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
                .Where(c => c.IndexDefinitionDao.Symbol == indexSymbol)
                .OrderByDescending(c => c.CreationDate)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CreationDate <= asOfUtc, cancellationToken);
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
            string quoteCurrency = "usdc",
            CancellationToken cancellationToken = default)
        {
            var issueDate = composition.CreationDate;
            var valuation = await _dbContext.IndexValuations
                .IncludeAllLinkedEntities()
                .Where(c => c.IndexCompositionDao.Id == $"{composition.IndexDefinition.Symbol}|{composition.Version}"
                                      && c.QuoteCurrency == quoteCurrency
                                      && c.TimeStamp == issueDate)
                .AsNoTracking()
                .SingleOrDefaultAsync(cancellationToken);
            return valuation;
        }

        private async Task<IIndexComposition> RetrieveFullComposition(string indexSymbol, uint version, 
            CancellationToken cancellationToken = default)
        {
            var composition = await _dbContext.IndexCompositions
                .IncludeAllLinkedEntities()
                .AsNoTracking()
                .SingleOrDefaultAsync(c => c.IndexDefinitionDao.Symbol == indexSymbol 
                                           && c.Version == version, cancellationToken);
            return composition;
        }

        private async Task<IIndexComposition> RetrieveFullComposition(string compositionSymbol,
            CancellationToken cancellationToken = default)
        {
            var composition = await _dbContext.IndexCompositions
                .IncludeAllLinkedEntities()
                .AsNoTracking()
                .Where(c => c.Symbol == compositionSymbol)
                .OrderByDescending(c => c.Version)
                .FirstAsync(cancellationToken);
            return composition;
        }
    }
}