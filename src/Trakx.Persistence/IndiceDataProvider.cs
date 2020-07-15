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
using Trakx.Common.Utils;

namespace Trakx.Persistence
{
    public class IndiceDataProvider : IIndiceDataProvider
    {
        private readonly IndiceRepositoryContext _dbContext;
        private readonly IMemoryCache _memoryCache;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILogger<IndiceDataProvider> _logger;

        public IndiceDataProvider(IndiceRepositoryContext dbContext, 
            IMemoryCache memoryCache,
            IDateTimeProvider dateTimeProvider,
            ILogger<IndiceDataProvider> logger)
        {
            _dbContext = dbContext;
            _memoryCache = memoryCache;
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<List<string>> GetAllIndiceSymbols(CancellationToken cancellationToken = default)
        {
            var indiceSymbols = await _dbContext.IndiceDefinitions
                .AsNoTracking()
                .Select(i => i.Symbol)
                .ToListAsync(cancellationToken);
            return indiceSymbols;
        }

        /// <inheritdoc />
        public async Task<List<string>> GetCompositionSymbolsFromIndice(string indiceSymbol, CancellationToken cancellationToken = default)
        {
            var indiceDefinition = await _dbContext.IndiceDefinitions
                .Include(i => i.IndiceCompositionDaos)
                .AsNoTracking()
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
                var cacheKey = $"composition|{indiceSymbol}|asOf|{asOfUtc:yyMMddHHmm}";
                var def = await _memoryCache.GetOrCreateAsync(cacheKey, async entry =>
                {
                    var version = await GetVersionAtDate(indiceSymbol, asOfUtc, cancellationToken);
                    if (version == default)
                    {
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromTicks(1);
                        return default;
                    }

                    var composition = await RetrieveFullComposition(indiceSymbol, version.Value, cancellationToken);
                    
                    entry.AbsoluteExpirationRelativeToNow = composition != null && composition.IsValid()
                        ? TimeSpan.FromMinutes(1)
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
        public async Task<Dictionary<TimeInterval, IIndiceComposition>> GetCompositionsBetweenDates(string indiceSymbol, DateTime startTime, DateTime? endTime = default,
            CancellationToken cancellationToken = default)
        {
            endTime ??= _dateTimeProvider.UtcNow;
            var compositions = _dbContext.IndiceCompositions;

            var firstComposition = await GetCompositionAtDate(indiceSymbol, startTime, cancellationToken)
                .ConfigureAwait(false);
            
            var laterCompositions = await compositions
                .IncludeAllLinkedEntities()
                .Where(c => c.IndiceDefinitionDao.Symbol == indiceSymbol)
                .AsNoTracking()
                .Where(c => startTime < c.CreationDate && c.CreationDate < endTime.Value)
                .OrderBy(c => c.CreationDate)
                .ToListAsync(cancellationToken);

            var activeCompositions = new[] {firstComposition!}
                .Concat(laterCompositions).ToList();

            var compositionByTimeInterval = activeCompositions
                .Where((c, i) => i < activeCompositions.Count - 1)
                .Select((composition, index) => (composition, index))
                .ToDictionary(
                    p => new TimeInterval(p.index == 0 ? startTime : p.composition.CreationDate, activeCompositions[p.index + 1].CreationDate), 
                    p => p.composition);
            var lastComposition = activeCompositions.Last();
            compositionByTimeInterval.Add(
                new TimeInterval(compositionByTimeInterval.Any() ? lastComposition.CreationDate : startTime, endTime.Value), lastComposition);

            return compositionByTimeInterval;
        }

        /// <inheritdoc />
        public async Task<IIndiceDefinition?> GetDefinitionFromSymbol(string indexSymbol, CancellationToken cancellationToken = default)
        {
            try
            {
                var definition = await _dbContext.IndiceDefinitions.FindAsync(new [] {indexSymbol}, cancellationToken);
                return definition;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve index definition for {0}", indexSymbol);
                return default;
            }
        }

        /// <inheritdoc />
        public async Task<IIndiceComposition?> GetCompositionFromSymbol(string compositionSymbol, CancellationToken cancellationToken = default)
        {
            try
            {
                var cacheKey = $"composition|{compositionSymbol}";
                var def = await _memoryCache.GetOrCreateAsync(cacheKey, async entry =>
                {
                    var composition = await RetrieveFullComposition(compositionSymbol, cancellationToken);

                    entry.AbsoluteExpirationRelativeToNow = composition != null && composition.IsValid()
                        ? TimeSpan.FromDays(1)
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
            try
            {
                var cacheKey = $"initial-valuation|{composition.Symbol}|{quoteCurrency}";
                var intitialValuation = await _memoryCache.GetOrCreateAsync(cacheKey, async entry =>
                {
                    var valuation = await _dbContext.IndiceValuations
                        .IncludeAllLinkedEntities()
                        .Where(c => c.IndiceCompositionDao.Id == composition.GetCompositionId()
                                    && c.QuoteCurrency == quoteCurrency
                                    && c.TimeStamp == composition.CreationDate)
                        .AsNoTracking()
                        .SingleOrDefaultAsync(cancellationToken);

                    entry.AbsoluteExpirationRelativeToNow = valuation != null && valuation.IsValid()
                        ? TimeSpan.FromDays(1)
                        : TimeSpan.FromTicks(1);
                    return valuation;
                });
                return intitialValuation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve initial valuation for composition {0}", composition.IndiceDefinition);
                return default;
            }
        }

        /// <inheritdoc />
        public async Task<List<IComponentDefinition>> GetAllComponentsFromCurrentCompositions(CancellationToken cancellationToken = default)
        {
            //TODO simplify that to exclude components not in use when they start to happen.
            var components = await _dbContext.ComponentDefinitions
                .Select(c => (IComponentDefinition)c)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

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
                .FirstOrDefaultAsync(cancellationToken);
            return composition;
        }

        /// <inheritdoc />
        public async Task<List<IIndiceComposition>?> GetAllCompositionForIndice(string indiceSymbol, CancellationToken cancellationToken = default) 
        {
            var indice = await _dbContext.IndiceDefinitions
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Symbol == indiceSymbol, cancellationToken);

            if (indice == null)
                return null;

            return await _dbContext.IndiceCompositions
                .IncludeAllLinkedEntities()
                .AsNoTracking()
                .Where(c => c.IndiceDefinitionDao == indice)
                .ToListAsync<IIndiceComposition>(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<List<IIndiceDefinition>> GetAllIndices(CancellationToken cancellationToken = default)
        {
            var indices = await _dbContext.IndiceDefinitions.
                ToListAsync<IIndiceDefinition>(cancellationToken);
            return indices;
        } 

        /// <inheritdoc />
        public async Task<bool> TryToGetIndiceByAddress(string? indiceAddress, CancellationToken cancellationToken = default)
        {
            return await _dbContext.IndiceDefinitions
                .AnyAsync(i => i.Address == indiceAddress, cancellationToken);
        } 

        /// <inheritdoc />
        public async Task<bool> TryToGetCompositionByAddress(string compositionAddress, CancellationToken cancellationToken = default)
        {
            return await _dbContext.IndiceCompositions
                .AnyAsync(c => c.Address == compositionAddress,
                cancellationToken);
        } 
    }
}