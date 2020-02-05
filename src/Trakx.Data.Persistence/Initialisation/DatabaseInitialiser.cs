using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Trakx.Data.Common.Core;
using Trakx.Data.Common.Pricing;
using Trakx.Data.Persistence.DAO;

[assembly: InternalsVisibleTo("Trakx.Data.Tests")]

namespace Trakx.Data.Persistence.Initialisation
{
    public class DatabaseInitialiser : IDatabaseInitialiser
    {
        private readonly IndexRepositoryContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<DatabaseInitialiser> _logger;
        private static readonly DateTime FirstOctober = new DateTime(2019, 10, 1);
        private const string Usdc = Constants.DefaultQuoteCurrency;

        public DatabaseInitialiser(IndexRepositoryContext dbContext,
            IMapper mapper,
            ILogger<DatabaseInitialiser> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task SeedDatabase()
        {
            await Migrate().ConfigureAwait(false);
            await SeedIndexDefinitions().ConfigureAwait(false);
        }

        private async Task Migrate()
        {
            await _dbContext.Database.MigrateAsync().ConfigureAwait(false);
        }

        private async Task SeedIndexDefinitions()
        {
            if (await _dbContext.IndexDefinitions.AnyAsync()
                .ConfigureAwait(false)) return;

            await AddKnownIndexes(_dbContext, _mapper).ConfigureAwait(false);

            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        internal static async Task AddKnownIndexes(IndexRepositoryContext dbContext, IMapper mapper)
        {
            using var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            var componentDefinitions = CreateComponentDefinitions();
            var componentDefinitionDaos = componentDefinitions
                .ToDictionary(c => c.Key, c => mapper.Map<ComponentDefinitionDao>(c.Value));
            await dbContext.ComponentDefinitions.AddRangeAsync(componentDefinitionDaos.Values, cancellationToken);

            var indexDefinitions = CreateIndexDefinitions();
            var indexDefinitionDaos = indexDefinitions.Select(i => mapper.Map<IndexDefinitionDao>(i.Value)).ToList();
            await dbContext.IndexDefinitions.AddRangeAsync(indexDefinitionDaos, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            await CreateComponentWeights(dbContext, cancellationToken);

            await CreateIndexCompositions(dbContext, cancellationToken);

            await CreateComponentQuantities(dbContext, cancellationToken);

            await CreateComponentValuations(dbContext, cancellationToken);

            await CreateIndexValuations(dbContext, cancellationToken);
        }

        private static async Task CreateComponentWeights(IndexRepositoryContext dbContext, CancellationToken cancellationToken)
        {
            var l1cpu003 = await dbContext.IndexDefinitions.SingleAsync(c => c.Symbol == "l1cpu003", cancellationToken);
            var elf = await dbContext.ComponentDefinitions.SingleAsync(c => c.Symbol == "elf", cancellationToken);
            var abt = await dbContext.ComponentDefinitions.SingleAsync(c => c.Symbol == "abt", cancellationToken);
            var ankr = await dbContext.ComponentDefinitions.SingleAsync(c => c.Symbol == "ankr", cancellationToken);

            var cpuComponentWeightDaos = new []
            {
                new ComponentWeightDao(l1cpu003, elf, 0.33m),
                new ComponentWeightDao(l1cpu003, abt, 0.33m),
                new ComponentWeightDao(l1cpu003, ankr, 0.34m)
            };
            await dbContext.ComponentWeights.AddRangeAsync(cpuComponentWeightDaos);

            var l1str004 = await dbContext.IndexDefinitions.SingleAsync(c => c.Symbol == "l1str004", cancellationToken);
            var storj = await dbContext.ComponentDefinitions.SingleAsync(c => c.Symbol == "storj", cancellationToken);
            var blz = await dbContext.ComponentDefinitions.SingleAsync(c => c.Symbol == "blz", cancellationToken);
            var rlc = await dbContext.ComponentDefinitions.SingleAsync(c => c.Symbol == "rlc", cancellationToken);
            var strComponentWeightDaos = new []
            {
                new ComponentWeightDao(l1str004, storj, 0.25m),
                new ComponentWeightDao(l1str004, ankr, 0.25m),
                new ComponentWeightDao(l1str004, blz, 0.25m),
                new ComponentWeightDao(l1str004, rlc, 0.25m)
            };
            await dbContext.ComponentWeights.AddRangeAsync(strComponentWeightDaos);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        internal static Dictionary<string, ComponentDefinition> CreateComponentDefinitions()
        {
            var elfDefinition = new ComponentDefinition("0xbf2179859fc6D5BEE9Bf9158632Dc51678a4100e",
                "ELF Token",
                "elf",
                18);
            var abtDefinition = new ComponentDefinition("0xB98d4C97425d9908E66E53A6fDf673ACcA0BE986",
                "ArcBlock",
                "abt",
                18);
            var ankrDefinition = new ComponentDefinition("0x8290333ceF9e6D528dD5618Fb97a76f268f3EDD4",
                "Ankr Network",
                "ankr",
                18);
            var mcoDefinition = new ComponentDefinition("0xB63B606Ac810a52cCa15e44bB630fd42D8d1d83d",
                "Monaco",
                "mco",
                8);
            var omgDefinition = new ComponentDefinition("0xd26114cd6EE289AccF82350c8d8487fedB8A0C07",
                "OMGToken",
                "omg",
                18);
            var npxsDefinition = new ComponentDefinition("0xA15C7Ebe1f07CaF6bFF097D8a589fb8AC49Ae5B3",
                "Pundi X Token",
                "npxs",
                18);
            var hotDefinition = new ComponentDefinition("0x6c6EE5e31d828De241282B9606C8e98Ea48526E2",
                "HoloToken",
                "hot",
                18);
            var sntDefinition = new ComponentDefinition("0x744d70FDBE2Ba4CF95131626614a1763DF805B9E",
                "Status Network Token",
                "snt",
                18);
            var tntDefinition = new ComponentDefinition("0x08f5a9235B08173b7569F83645d2c7fB55e8cCD8",
                "Tierion Network Token",
                "tnt",
                8);
            var maticDefinition = new ComponentDefinition("0x7D1AfA7B718fb893dB30A3aBc0Cfc608AaCfeBB0",
                "Matic Token",
                "matic",
                18);
            var celrDefinition = new ComponentDefinition("0x4F9254C83EB525f9FCf346490bbb3ed28a81C667",
                "CelerToken",
                "CELR",
                18);
            var ftmDefinition = new ComponentDefinition("0x4E15361FD6b4BB609Fa63C81A2be19d873717870",
                "Fantom Token",
                "ftm",
                18);
            var storjDefinition = new ComponentDefinition("0xB64ef51C888972c908CFacf59B47C1AfBC0Ab8aC",
                "StorjToken",
                "storj",
                8);
            var blzDefinition = new ComponentDefinition("0x5732046A883704404F284Ce41FfADd5b007FD668",
                "Bluzelle Token",
                "blz",
                18);
            var rlcDefinition = new ComponentDefinition("0x607F4C5BB672230e8672085532f7e901544a7375",
                "iEx.ec Network Token",
                "rlc",
                9);

            var componentDefinitions = new List<ComponentDefinition>()
                {
                    elfDefinition,
                    abtDefinition,
                    ankrDefinition,
                    mcoDefinition,
                    omgDefinition,
                    npxsDefinition,
                    hotDefinition,
                    sntDefinition,
                    tntDefinition,
                    maticDefinition,
                    celrDefinition,
                    ftmDefinition,
                    storjDefinition,
                    blzDefinition,
                    rlcDefinition
                };
            var dictionary = componentDefinitions.ToDictionary(c => c.Symbol, c => c);
            return dictionary;
        }

        internal static Dictionary<string, IndexDefinitionDao> CreateIndexDefinitions()
        {
            var cpuIndexDefinition = new IndexDefinitionDao("l1cpu003", "Top 3 Computation Services",
                "This index is composed of the top 3 erc20 tokens for Computation Services. Created on the 1st of October as a POC.",
                2,
                "0x7210cc724480c85b893a9febbecc24a8dc4ff1de", FirstOctober);

            var strIndexDefinition = new IndexDefinitionDao("l1str004", "Top 4 Storage Services",
                "This index is composed of the top 4 erc20 tokens for Storage Services. Created on the 1st of October as a POC.",
                11,
                "0xe05168c3fa30e93d3f1667b35e9456aac9b5519a", FirstOctober);

            var indexDefinitions = new List<IndexDefinitionDao> { cpuIndexDefinition, strIndexDefinition };
            var dictionary = indexDefinitions.ToDictionary(d => d.Symbol, d => d);
            return dictionary;
        }

        internal static async Task CreateComponentQuantities(IndexRepositoryContext dbContext, CancellationToken cancellationToken)
        {
            var componentDefinitions = dbContext.ComponentDefinitions.ToDictionary(d => d.Symbol, d => d);

            var l1cpu003 = await dbContext.IndexCompositions.SingleAsync(c => c.Id == "l1cpu003|1", cancellationToken);
            var elfCpuQuantity = new ComponentQuantityDao(l1cpu003, componentDefinitions["elf"], 42);
            var abtCpuQuantity = new ComponentQuantityDao(l1cpu003, componentDefinitions["abt"], 17);
            var ankrCpuQuantity = new ComponentQuantityDao(l1cpu003, componentDefinitions["ankr"], 1010);

            var l1str004 = await dbContext.IndexCompositions.SingleAsync(c => c.Id == "l1str004|1", cancellationToken);
            var storjStrQuantity = new ComponentQuantityDao(l1str004, componentDefinitions["storj"], 2);
            var ankrStrQuantity = new ComponentQuantityDao(l1str004, componentDefinitions["ankr"], 836960160697);
            var blzStrQuantity = new ComponentQuantityDao(l1str004, componentDefinitions["blz"], 75688767787);
            var rlcStrQuantity = new ComponentQuantityDao(l1str004, componentDefinitions["rlc"], 12);

            await dbContext.ComponentQuantities.AddRangeAsync(new[]
            {
                elfCpuQuantity, abtCpuQuantity, ankrCpuQuantity,
                storjStrQuantity, ankrStrQuantity, blzStrQuantity, rlcStrQuantity
            });
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        private static async Task CreateIndexCompositions(IndexRepositoryContext dbContext, CancellationToken cancellationToken)
        {
            var l1cpu003 = await dbContext.IndexDefinitions.SingleAsync(c => c.Symbol == "l1cpu003", cancellationToken);
            var cpuComposition = new IndexCompositionDao(l1cpu003, 1, FirstOctober);

            var l1str004 = await dbContext.IndexDefinitions.SingleAsync(c => c.Symbol == "l1str004", cancellationToken);
            var strComposition = new IndexCompositionDao(l1str004, 1, FirstOctober);

            await dbContext.IndexCompositions.AddRangeAsync(new[] {cpuComposition, strComposition});
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        private static async Task CreateComponentValuations(IndexRepositoryContext dbContext, CancellationToken cancellationToken)
        {
            var quantities = dbContext.ComponentQuantities.ToDictionary(d => d.Id, d => d);

            var elfCpuValuation = new ComponentValuationDao(quantities["l1cpu003|1|elf"], FirstOctober, Usdc, 0.08069m);
            var abtCpuValuation = new ComponentValuationDao(quantities["l1cpu003|1|abt"], FirstOctober, Usdc, 0.202m);
            var ankrCpuValuation = new ComponentValuationDao(quantities["l1cpu003|1|ankr"], FirstOctober, Usdc, 0.00225661m);

            var storjStrValuation = new ComponentValuationDao(quantities["l1str004|1|storj"], FirstOctober, Usdc, 0.1493m);
            var ankrStrValuation = new ComponentValuationDao(quantities["l1str004|1|ankr"], FirstOctober, Usdc, 0.00225661m);
            var blzStrValuation = new ComponentValuationDao(quantities["l1str004|1|blz"], FirstOctober, Usdc, 0.03303m);
            var rlcStrValuation = new ComponentValuationDao(quantities["l1str004|1|rlc"], FirstOctober, Usdc, 0.2181m);

            await dbContext.ComponentValuations.AddRangeAsync(new[]
            {
                elfCpuValuation, ankrCpuValuation, abtCpuValuation,
                storjStrValuation, ankrStrValuation, blzStrValuation, rlcStrValuation
            });
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        private static async Task CreateIndexValuations(IndexRepositoryContext dbContext,
            CancellationToken cancellationToken)
        {
            var initialCpuValuations = dbContext.ComponentValuations
                .Include(c => c.ComponentQuantityDao)
                .ThenInclude(c => c.IndexCompositionDao)
                .Where(v => v.ComponentQuantityDao.IndexCompositionDao.Id == "l1cpu003|1").ToList();
            var initialCpuValuation = new IndexValuationDao(initialCpuValuations);

            var initialStrValuations = dbContext.ComponentValuations
                .Include(c => c.ComponentQuantityDao)
                .ThenInclude(c => c.IndexCompositionDao)
                .Where(v => v.ComponentQuantityDao.IndexCompositionDao.Id == "l1str004|1").ToList();
            var initialStrValuation = new IndexValuationDao(initialStrValuations);

            await dbContext.AddRangeAsync(new[] {initialStrValuation, initialCpuValuation}, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

        }
    }
}