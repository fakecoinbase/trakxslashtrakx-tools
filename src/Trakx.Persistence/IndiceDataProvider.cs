using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Trakx.Common.Interfaces;
using Trakx.Common.Interfaces.Indice;

namespace Trakx.Persistence
{
    public class IndiceDataProvider : IIndiceDataProvider
    {
        private readonly IndiceRepositoryContext _dbContext;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<IndiceDataProvider> _logger;

        public IndiceDataProvider(IndiceRepositoryContext dbContext, 
            IMemoryCache memoryCache,
            ILogger<IndiceDataProvider> logger)
        {
            _dbContext = dbContext;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<List<string>> GetAllIndiceSymbols(CancellationToken cancellationToken = default)
        {
            var indiceSymbols = await _dbContext.IndiceDefinitions.AsNoTracking().Select(i => i.Symbol)
                .ToListAsync(cancellationToken);
            return indiceSymbols;
        }

        /// <inheritdoc />
        public async Task<List<string>> GetCompositionSymbolsFromIndice(string indiceSymbol, CancellationToken cancellationToken = default)
        {
            var indiceDefinition = await _dbContext.IndiceDefinitions
                .Include(i => i.IndiceCompositionDaos)
                .SingleAsync(i => i.Symbol.Equals(indiceSymbol, StringComparison.InvariantCultureIgnoreCase), 
                    cancellationToken);

            var symbols = indiceDefinition.IndiceCompositionDaos.Select(c => c.Symbol).ToList();
            return symbols;
        }

        /// <inheritdoc />
        public async Task<IIndiceComposition?> GetCompositionAtDate(string indiceSymbol, DateTime asOfUtc,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var cacheKey = $"{indiceSymbol}|asOf|{asOfUtc:yyMMddHHmmss}";
                var def = await _memoryCache.GetOrCreateAsync(cacheKey, async entry =>
                {
                    entry.SetSlidingExpiration(TimeSpan.FromSeconds(10));
                    
                    var version = await GetVersionAtDate(indiceSymbol, asOfUtc, cancellationToken);
                    var composition = await RetrieveFullComposition(indiceSymbol, version.Value, cancellationToken);

                    entry.AbsoluteExpirationRelativeToNow = composition != null && composition.IsValid() 
                        ? TimeSpan.FromSeconds(100)
                        : TimeSpan.FromTicks(1);
                    return composition;
                });

                return def;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve composition for {0} at date {1}", indiceSymbol, asOfUtc);
                return default;
            }
        }

        /// <inheritdoc />
        public async Task<IIndiceComposition?> GetCompositionFromSymbol(string compositionSymbol, CancellationToken cancellationToken = default)
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
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "Failed to retrieve composition with symbol {0}", compositionSymbol);
                return default;
            }
        }

        private async Task<uint?> GetVersionAtDate(string indiceSymbol, DateTime asOfUtc, CancellationToken cancellationToken = default)
        {
            var compositions = _dbContext.IndiceCompositions;
            var result = await compositions
                .Include(c => c.IndiceDefinitionDao)
                .Where(c => c.IndiceDefinitionDao.Symbol == indiceSymbol)
                .OrderByDescending(c => c.CreationDate)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CreationDate <= asOfUtc, cancellationToken);
            return result?.Version;
        }

        /// <inheritdoc />
        public async Task<IIndiceComposition?> GetCurrentComposition(string indiceSymbol, CancellationToken cancellationToken = default)
        {
            return await GetCompositionAtDate(indiceSymbol, DateTime.UtcNow, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IIndiceValuation> GetInitialValuation(IIndiceComposition composition, 
            string quoteCurrency = "usdc",
            CancellationToken cancellationToken = default)
        {
            var issueDate = composition.CreationDate;
            var valuation = await _dbContext.IndiceValuations
                .IncludeAllLinkedEntities()
                .Where(c => c.IndiceCompositionDao.Id == $"{composition.IndiceDefinition.Symbol}|{composition.Version}"
                                      && c.QuoteCurrency == quoteCurrency
                                      && c.TimeStamp == issueDate)
                .AsNoTracking()
                .SingleOrDefaultAsync(cancellationToken);
            return valuation;
        }

        /// <inheritdoc />
        public async Task<List<IComponentDefinition>> GetAllComponentsFromCurrentCompositions(CancellationToken cancellationToken = default)
        {
            //TODO simplify that to exclude components not in use when they start to happen.
            var components = await _dbContext.ComponentDefinitions.Select(c => (IComponentDefinition)c).ToListAsync(cancellationToken);

            return components;
        }

        private async Task<IIndiceComposition> RetrieveFullComposition(string indiceSymbol, uint version, 
            CancellationToken cancellationToken = default)
        {
            var composition = await _dbContext.IndiceCompositions
                .IncludeAllLinkedEntities()
                .AsNoTracking()
                .SingleOrDefaultAsync(c => c.IndiceDefinitionDao.Symbol == indiceSymbol 
                                           && c.Version == version, cancellationToken);
            return composition;
        }

        private async Task<IIndiceComposition> RetrieveFullComposition(string compositionSymbol,
            CancellationToken cancellationToken = default)
        {
            var composition = await _dbContext.IndiceCompositions
                .IncludeAllLinkedEntities()
                .AsNoTracking()
                .Where(c => c.Symbol == compositionSymbol)
                .OrderByDescending(c => c.Version)
                .FirstAsync(cancellationToken);
            return composition;
        }

        /// <inheritdoc />
        public async Task<List<IIndiceComposition>?> GetAllCompositionForIndice(string indiceSymbol) 
        {
            var indice = await _dbContext.IndiceDefinitions.FirstOrDefaultAsync(i => i.Symbol == indiceSymbol);

            if (indice == null)
                return null;

            return await _dbContext.IndiceCompositions.Where(c => c.IndiceDefinitionDao == indice)
                .ToListAsync<IIndiceComposition>();
        }

        /// <inheritdoc />
        public async Task<List<IIndiceDefinition>> GetAllIndices()
        {
            var indices = await _dbContext.IndiceDefinitions.ToListAsync<IIndiceDefinition>();
            return indices;
        } 

        /// <inheritdoc />
        public async Task<bool> TryToGetIndiceByAddress(string? indiceAddress)
        {
            if (await _dbContext.IndiceDefinitions.FirstOrDefaultAsync(i => i.Address == indiceAddress) != null)
                return true;

            return false;
        } 

        /// <inheritdoc />
        public async Task<bool> TryToGetCompositionByAddress(string compositionAddress)
        {
            if (await _dbContext.IndiceCompositions.FirstOrDefaultAsync(c => c.Address == compositionAddress) != null)
                return true;

            return false;
        } 
    }
}