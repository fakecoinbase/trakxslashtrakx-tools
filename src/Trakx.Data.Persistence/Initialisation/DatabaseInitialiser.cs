using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Trakx.Data.Common.Composition;
using Trakx.Data.Common.Interfaces.Index;
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
        private static readonly DateTime FirstJan = new DateTime(2020, 1, 1);
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

            await CreateComponentDefinitions(dbContext, cancellationToken);

            await CreateIndexDefinitions(dbContext, cancellationToken);

            await CreateComponentWeights(dbContext, cancellationToken);

            await CreateIndexCompositions(dbContext, mapper, cancellationToken);

            await CreateComponentQuantities(dbContext, cancellationToken);

            await CreateComponentValuations(dbContext, cancellationToken);

            await CreateIndexValuations(dbContext, cancellationToken);
        }

        private static async Task CreateComponentWeights(IndexRepositoryContext dbContext, CancellationToken cancellationToken)
        {
            var indexBySymbols = (await dbContext.IndexDefinitions.ToListAsync(cancellationToken)).ToDictionary(i => i.Symbol, i => i);
            var componentBySymbol = (await dbContext.ComponentDefinitions.ToListAsync(cancellationToken)).ToDictionary(c => c.Symbol, c => c);

            var l1cpu003 = indexBySymbols["l1cpu003"];

            var cpuComponentWeightDaos = new []
            {
                new ComponentWeightDao(l1cpu003, componentBySymbol["elf"], 0.33m),
                new ComponentWeightDao(l1cpu003, componentBySymbol["abt"], 0.33m),
                new ComponentWeightDao(l1cpu003, componentBySymbol["ankr"], 0.34m)
            };
            await dbContext.ComponentWeights.AddRangeAsync(cpuComponentWeightDaos, cancellationToken);

            var l1str004 = indexBySymbols["l1str004"];
            var strComponentWeightDaos = new []
            {
                new ComponentWeightDao(l1str004, componentBySymbol["storj"], 0.25m),
                new ComponentWeightDao(l1str004, componentBySymbol["ankr"], 0.25m),
                new ComponentWeightDao(l1str004, componentBySymbol["blz"], 0.25m),
                new ComponentWeightDao(l1str004, componentBySymbol["rlc"], 0.25m)
            };

            await dbContext.ComponentWeights.AddRangeAsync(strComponentWeightDaos, cancellationToken);

            var lendingWeights = new[]
            {
                new ComponentWeightDao(indexBySymbols["l1len"], componentBySymbol["lnd"], 0.0487275092314901m),
                new ComponentWeightDao(indexBySymbols["l1len"], componentBySymbol["akro"], 0.048267987428224m),
                new ComponentWeightDao(indexBySymbols["l1len"], componentBySymbol["bcpt"], 0.0514905531675755m),
                new ComponentWeightDao(indexBySymbols["l1len"], componentBySymbol["lba"], 0.075050561708752m),
                new ComponentWeightDao(indexBySymbols["l1len"], componentBySymbol["lend"], 0.0865759511672131m),
                new ComponentWeightDao(indexBySymbols["l1len"], componentBySymbol["mkr"], 0.3m),
                new ComponentWeightDao(indexBySymbols["l1len"], componentBySymbol["nexo"], 0.164041680816541m),
                new ComponentWeightDao(indexBySymbols["l1len"], componentBySymbol["ppt"], 0.0735535096292126m),
                new ComponentWeightDao(indexBySymbols["l1len"], componentBySymbol["rcn"], 0.0969502811373821m),
                new ComponentWeightDao(indexBySymbols["l1len"], componentBySymbol["salt"], 0.0553419657136083m),

            };
            await dbContext.ComponentWeights.AddRangeAsync(lendingWeights, cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        internal static async Task CreateComponentDefinitions(IndexRepositoryContext dbContext, CancellationToken cancellationToken)
        {
            var componentDefinitions = new List<ComponentDefinitionDao>()
                {
                    new ComponentDefinitionDao("0xbf2179859fc6D5BEE9Bf9158632Dc51678a4100e", "ELF Token", "elf", 18),
                    new ComponentDefinitionDao("0xB98d4C97425d9908E66E53A6fDf673ACcA0BE986", "ArcBlock", "abt", 18),
                    new ComponentDefinitionDao("0x8290333ceF9e6D528dD5618Fb97a76f268f3EDD4", "Ankr Network", "ankr", 18),
                    new ComponentDefinitionDao("0xB63B606Ac810a52cCa15e44bB630fd42D8d1d83d", "Monaco", "mco", 8),
                    new ComponentDefinitionDao("0xd26114cd6EE289AccF82350c8d8487fedB8A0C07", "OMGToken", "omg", 18),
                    new ComponentDefinitionDao("0xA15C7Ebe1f07CaF6bFF097D8a589fb8AC49Ae5B3", "Pundi X Token", "npxs", 18),
                    new ComponentDefinitionDao("0x6c6EE5e31d828De241282B9606C8e98Ea48526E2", "HoloToken", "hot", 18),
                    new ComponentDefinitionDao("0x744d70FDBE2Ba4CF95131626614a1763DF805B9E", "Status Network Token", "snt", 18),
                    new ComponentDefinitionDao("0x08f5a9235B08173b7569F83645d2c7fB55e8cCD8", "Tierion Network Token", "tnt", 8),
                    new ComponentDefinitionDao("0x7D1AfA7B718fb893dB30A3aBc0Cfc608AaCfeBB0", "Matic Token", "matic", 18),
                    new ComponentDefinitionDao("0x4F9254C83EB525f9FCf346490bbb3ed28a81C667", "CelerToken", "CELR", 18),
                    new ComponentDefinitionDao("0x4E15361FD6b4BB609Fa63C81A2be19d873717870", "Fantom Token", "ftm", 18),
                    new ComponentDefinitionDao("0xB64ef51C888972c908CFacf59B47C1AfBC0Ab8aC", "StorjToken", "storj", 8),
                    new ComponentDefinitionDao("0x5732046A883704404F284Ce41FfADd5b007FD668", "Bluzelle Token", "blz", 18),
                    new ComponentDefinitionDao("0x607F4C5BB672230e8672085532f7e901544a7375", "iEx.ec Network Token", "rlc", 9),

                    new ComponentDefinitionDao("0x0947b0e6d821378805c9598291385ce7c791a6b2", "Lendingblock", "lnd", 18),
                    new ComponentDefinitionDao("0x8ab7404063ec4dbcfd4598215992dc3f8ec853d7", "Akropolis", "akro", 18),
                    new ComponentDefinitionDao("0x1c4481750daa5ff521a2a7490d9981ed46465dbd", "Blockmason Credit Protocol", "bcpt", 18),
                    new ComponentDefinitionDao("0xfe5f141bf94fe84bc28ded0ab966c16b17490657", "Cred", "lba", 18),
                    new ComponentDefinitionDao("0x80fB784B7eD66730e8b1DBd9820aFD29931aab03", "Aave", "lend", 18),
                    new ComponentDefinitionDao("0x4de2573e27e648607b50e1cfff921a33e4a34405", "Lendroid Support Token", "lst", 18),
                    new ComponentDefinitionDao("0x9f8f72aa9304c8b593d555f12ef6589cc3a579a2", "Maker", "mkr", 18),
                    new ComponentDefinitionDao("0xb62132e35a6c13ee1ee0f84dc5d40bad8d815206", "nexo", "nexo", 18),
                    new ComponentDefinitionDao("0xd4fa1460f537bb9085d22c7bccb5dd450ef28e3a", "Populous", "ppt", 8),
                    new ComponentDefinitionDao("0xf970b8e36e23f7fc3fd752eea86f8be8d83375a6", "Ripio Credit Network", "rcn", 18),
                    new ComponentDefinitionDao("0x4156D3342D5c385a87D264F90653733592000581", "Salt", "salt", 8),

            };
            await dbContext.ComponentDefinitions.AddRangeAsync(componentDefinitions, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        internal static async Task CreateIndexDefinitions(IndexRepositoryContext dbCOntext, CancellationToken cancellationToken)
        {
            var indexDefinitions = new List<IndexDefinitionDao>
            {
                new IndexDefinitionDao("l1cpu003", "Top 3 Computation Services",
                    "This index is composed of the top 3 erc20 tokens for Computation Services. Created on the 1st of October as a POC.",
                    2,
                    "0x7210cc724480c85b893a9febbecc24a8dc4ff1de", FirstOctober),
                new IndexDefinitionDao("l1str004", "Top 4 Storage Services",
                    "This index is composed of the top 4 erc20 tokens for Storage Services. Created on the 1st of October as a POC.",
                    11,
                    "0xe05168c3fa30e93d3f1667b35e9456aac9b5519a", FirstOctober),
                new IndexDefinitionDao("l1len", "Lending",
                    "Index composed of token from the Messari Lending sector",
                    10,
                    "", FirstJan)
            };
            await dbCOntext.AddRangeAsync(indexDefinitions, cancellationToken);
            await dbCOntext.SaveChangesAsync(cancellationToken);
        }

        internal static async Task CreateComponentQuantities(IndexRepositoryContext dbContext, CancellationToken cancellationToken)
        {
            var componentDefinitions = dbContext.ComponentDefinitions.ToDictionary(d => d.Symbol, d => d);

            var l1cpu003 = await dbContext.IndexCompositions.SingleAsync(c => c.Id == "l1cpu003|1", cancellationToken);
            var l1str004 = await dbContext.IndexCompositions.SingleAsync(c => c.Id == "l1str004|1", cancellationToken);

            var oldIndexAdjustment = (ulong)10e2;
            var elfCpuQuantity = new ComponentQuantityDao(l1cpu003, componentDefinitions["elf"], 42 * oldIndexAdjustment);
            var abtCpuQuantity = new ComponentQuantityDao(l1cpu003, componentDefinitions["abt"], 17 * oldIndexAdjustment);
            var ankrCpuQuantity = new ComponentQuantityDao(l1cpu003, componentDefinitions["ankr"], 1010 * oldIndexAdjustment);

            var storjStrQuantity = new ComponentQuantityDao(l1str004, componentDefinitions["storj"], 2);
            var ankrStrQuantity = new ComponentQuantityDao(l1str004, componentDefinitions["ankr"], 836960160697 * oldIndexAdjustment);
            var blzStrQuantity = new ComponentQuantityDao(l1str004, componentDefinitions["blz"], 75688767787 * oldIndexAdjustment);
            var rlcStrQuantity = new ComponentQuantityDao(l1str004, componentDefinitions["rlc"], 12 * oldIndexAdjustment);

            await dbContext.ComponentQuantities.AddRangeAsync(new[]
            {
                elfCpuQuantity, abtCpuQuantity, ankrCpuQuantity,
                storjStrQuantity, ankrStrQuantity, blzStrQuantity, rlcStrQuantity
            });
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        private static async Task CreateIndexCompositions(IndexRepositoryContext dbContext, IMapper mapper, CancellationToken cancellationToken)
        {
            var indexBySymbols = (await dbContext.IndexDefinitions.ToListAsync(cancellationToken)).ToDictionary(i => i.Symbol, i => i);
            var componentsBySymbols = (await dbContext.ComponentDefinitions.ToListAsync(cancellationToken)).ToDictionary(i => i.Symbol, i => i);
            var l1cpu003 = indexBySymbols["l1cpu003"];
            var cpuComposition = new IndexCompositionDao(l1cpu003, 1, FirstOctober);

            var l1str004 = indexBySymbols["l1str004"];
            var strComposition = new IndexCompositionDao(l1str004, 1, FirstOctober);

            var pricesByDefintions = new Dictionary<IComponentDefinition, decimal>{
                { componentsBySymbols["lnd"], 0.00152077889740119m },
                { componentsBySymbols["akro"], 0.00106594967399099m },
                { componentsBySymbols["bcpt"], 0.0199456639115704m },
                { componentsBySymbols["lba"], 0.0198576158149738m },
                { componentsBySymbols["lend"], 0.0162103877518573m },
                //{ componentsBySymbols["lst"], 0.000341730020639167m },
                { componentsBySymbols["mkr"], 433.309542948559m },
                { componentsBySymbols["nexo"], 0.09443929731183m },
                { componentsBySymbols["ppt"], 0.339213629017299m },
                { componentsBySymbols["rcn"], 0.0448472125928383m },
                { componentsBySymbols["salt"], 0.0503968166330671m },
            };

            var lenComposition = IndexCompositionCalculator.CalculateIndexComposition(indexBySymbols["l1len"], 
                pricesByDefintions, 100m, 1,
                FirstJan);

            var lenCompositionDao = mapper.Map<IndexCompositionDao>(lenComposition);
            lenCompositionDao.ComponentQuantityDaos.ForEach(q => q.LinkToIndexComposition(lenCompositionDao));
            await dbContext.IndexCompositions.AddRangeAsync(new[] { cpuComposition, strComposition, lenCompositionDao });
            await dbContext.SaveChangesAsync(cancellationToken);

            var componentValuations = pricesByDefintions.Select(p => new ComponentValuationDao(
                lenCompositionDao.ComponentQuantityDaos.Single(q => q.ComponentDefinition.Symbol == p.Key.Symbol),
                FirstJan, Usdc, pricesByDefintions[componentsBySymbols[p.Key.Symbol]])).ToList();
            
            await dbContext.ComponentValuations.AddRangeAsync(componentValuations, cancellationToken);
            
            var lenValuation = new IndexValuationDao(componentValuations);
            await dbContext.IndexValuations.AddAsync(lenValuation, cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        private static async Task CreateComponentValuations(IndexRepositoryContext dbContext, CancellationToken cancellationToken)
        {
            var quantities = dbContext.ComponentQuantities.ToDictionary(d => d.Id, d => d);

            await dbContext.ComponentValuations.AddRangeAsync(new[]
            {
                new ComponentValuationDao(quantities["l1cpu003|1|elf"], FirstOctober, Usdc, 0.08069m),
                new ComponentValuationDao(quantities["l1cpu003|1|abt"], FirstOctober, Usdc, 0.202m),
                new ComponentValuationDao(quantities["l1cpu003|1|ankr"], FirstOctober, Usdc, 0.00225661m),

                new ComponentValuationDao(quantities["l1str004|1|storj"], FirstOctober, Usdc, 0.1493m),
                new ComponentValuationDao(quantities["l1str004|1|ankr"], FirstOctober, Usdc, 0.00225661m),
                new ComponentValuationDao(quantities["l1str004|1|blz"], FirstOctober, Usdc, 0.03303m),
                new ComponentValuationDao(quantities["l1str004|1|rlc"], FirstOctober, Usdc, 0.2181m),
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
            initialCpuValuations.ForEach(v => v.SetWeightFromTotalValue(initialCpuValuation.NetAssetValue));

            var initialStrValuations = dbContext.ComponentValuations
                .Include(c => c.ComponentQuantityDao)
                .ThenInclude(c => c.IndexCompositionDao)
                .Where(v => v.ComponentQuantityDao.IndexCompositionDao.Id == "l1str004|1").ToList();
            var initialStrValuation = new IndexValuationDao(initialStrValuations);
            initialStrValuations.ForEach(v => v.SetWeightFromTotalValue(initialStrValuation.NetAssetValue));

            await dbContext.AddRangeAsync(new[] {initialStrValuation, initialCpuValuation}, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

        }
    } 
}