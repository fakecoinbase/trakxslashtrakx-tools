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
            await _dbContext.Database.EnsureDeletedAsync();
            await _dbContext.Database.MigrateAsync().ConfigureAwait(false);
        }

        private async Task SeedIndexDefinitions()
        {
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

        internal static async Task CreateComponentDefinitions(IndexRepositoryContext dbContext, CancellationToken cancellationToken)
        {
            var componentDefinitions = new List<ComponentDefinitionDao>()
            {
                new ComponentDefinitionDao( "0xbf2179859fc6D5BEE9Bf9158632Dc51678a4100e", "ELF Token", "elf", 18),
                new ComponentDefinitionDao( "0xB98d4C97425d9908E66E53A6fDf673ACcA0BE986", "ArcBlock", "abt", 18),
                new ComponentDefinitionDao( "0x8290333ceF9e6D528dD5618Fb97a76f268f3EDD4", "Ankr Network", "ankr", 18),
                new ComponentDefinitionDao( "0xB64ef51C888972c908CFacf59B47C1AfBC0Ab8aC", "StorjToken", "storj", 8),
                new ComponentDefinitionDao( "0x5732046A883704404F284Ce41FfADd5b007FD668", "Bluzelle Token", "blz", 18),
                new ComponentDefinitionDao( "0x607F4C5BB672230e8672085532f7e901544a7375", "iEx.ec Network Token", "rlc", 9),
                
                #region Lending
                new ComponentDefinitionDao("0x0947b0e6d821378805c9598291385ce7c791a6b2", "Lendingblock", "lnd", 18),
                new ComponentDefinitionDao("0x8ab7404063ec4dbcfd4598215992dc3f8ec853d7", "Akropolis", "akro", 18),
                new ComponentDefinitionDao("0x1c4481750daa5ff521a2a7490d9981ed46465dbd", "Blockmason Credit Protocol", "bcpt", 18),
                new ComponentDefinitionDao("0xfe5f141bf94fe84bc28ded0ab966c16b17490657", "Cred", "lba", 18),
                new ComponentDefinitionDao("0x80fB784B7eD66730e8b1DBd9820aFD29931aab03", "Aave", "lend", 18),
                new ComponentDefinitionDao("0x4de2573e27e648607b50e1cfff921a33e4a34405", "Lendroid Support Token", "lst", 18),
                new ComponentDefinitionDao("0x9f8f72aa9304c8b593d555f12ef6589cc3a579a2", "Maker", "mkr", 18),
                new ComponentDefinitionDao("0xb62132e35a6c13ee1ee0f84dc5d40bad8d815206", "Nexo", "nexo", 18),
                new ComponentDefinitionDao("0xd4fa1460f537bb9085d22c7bccb5dd450ef28e3a", "Populous", "ppt", 8),
                new ComponentDefinitionDao("0xf970b8e36e23f7fc3fd752eea86f8be8d83375a6", "Ripio Credit Network", "rcn", 18),
                new ComponentDefinitionDao("0x4156D3342D5c385a87D264F90653733592000581", "Salt", "salt", 8),
                #endregion

                #region Asset Management
                new ComponentDefinitionDao("0xc80c5e40220172b36adee2c951f26f2a577810c5", "Bankera", "bnk", 8),
                new ComponentDefinitionDao("0x1014613e2b3cbc4d575054d4982e580d9b99d7b1", "BitCapitalVendor", "bcv", 8),
                new ComponentDefinitionDao("0x01ff50f8b7f74e4f00580d9596cd3d0d6d6e326f", "BnkToTheFuture", "bft", 18),
                new ComponentDefinitionDao("0xcb97e65f07da24d46bcdd078ebebd7c6e6e3d750", "Bytom", "btm", 8),
                new ComponentDefinitionDao("0x26e75307fc0c021472feb8f727839531f112f317", "Crypto20", "c20", 18),
                new ComponentDefinitionDao("0x177d39ac676ed1c67a2b268ad7f1e58826e5b0af", "Blox", "cdt", 18),
                new ComponentDefinitionDao("0xba9d4199fab4f26efe3551d490e3821486f135ba", "Swissborg", "chsb", 8),
                new ComponentDefinitionDao("0xd4c435f5b09f855c3317c8524cb1f586e42795fa", "Cindicator", "cnd", 18),
                new ComponentDefinitionDao("0x103c3a209da59d3e7c4a89307e66521e081cfdf0", "Genesis Vision", "gvt", 18),
                new ComponentDefinitionDao("0xec67005c4E498Ec7f55E092bd1d35cbC47C91892", "Melon", "mln", 18),
                new ComponentDefinitionDao("0x653430560be843c4a3d143d0110e896c2ab8ac0d", "Molecular Future", "mof", 16),
                new ComponentDefinitionDao("0x1776e1F26f98b1A5dF9cD347953a26dd3Cb46671", "Numeraire", "nmr", 18),
                new ComponentDefinitionDao("0xd26114cd6EE289AccF82350c8d8487fedB8A0C07", "OmiseGo", "omg", 18),
                new ComponentDefinitionDao("0x9992ec3cf6a55b00978cddf2b27bc6882d88d1ec", "Polymath Network", "poly", 18),
                new ComponentDefinitionDao("0xFc2C4D8f95002C14eD0a7aA65102Cac9e5953b5E", "Rublix", "rblx", 18),
                new ComponentDefinitionDao("0xc011a72400e58ecd99ee497cf89e3775d4bd732f", "Synthetix Network Token", "snx", 18),
                new ComponentDefinitionDao("0x3a92bd396aef82af98ebc0aa9030d25a23b11c6b", "Tokenbox", "tbx", 18),
                new ComponentDefinitionDao("0x4824a7b64e3966b0133f4f4ffb1b9d6beb75fff7", "TokenClub", "tct", 18),
                new ComponentDefinitionDao("0xf7920b0768ecb20a123fac32311d07d193381d6f", "Time New Bank  ", "tnb", 18),
            #endregion

                #region Decentralised Exchanges
                new ComponentDefinitionDao("0x27054b13b1b798b345b591a4d22e6562d47ea75a", "AirSwap", "ast", 4),
                new ComponentDefinitionDao("0x1f573d6fb3f13d689ff844b4ce37794d79a7ff1c", "Bancor Network Token", "bnt", 18),
                new ComponentDefinitionDao("0x1c83501478f1320977047008496dacbd60bb15ef", "Digitex Futures Exchange", "dgtx", 18),
                new ComponentDefinitionDao("0x9af839687f6c94542ac5ece2e317daae355493a1", "Hydro Protocol", "hot", 18),
                new ComponentDefinitionDao("0xB705268213D593B8FD88d3FDEFF93AFF5CbDcfAE", "Idex", "idex", 18),
                new ComponentDefinitionDao("0xdd974d5c2e2928dea5f71b9825b8b646686bd200", "Kyber Network", "knc", 18),
                new ComponentDefinitionDao("0xbbbbca6a901c926f240b89eacb641d8aec7aeafd", "Loopring", "lrc", 18),
                new ComponentDefinitionDao("0xcc80c051057b774cd75067dc48f8987c4eb97a5e", "Nectar Token", "nec", 18),
                new ComponentDefinitionDao("0x701c244b988a513c945973defa05de933b23fe1d", "Token OpenANX", "oax", 18),
                new ComponentDefinitionDao("0xa974c709cfb4566686553a20790685a47aceaa33", "Mixin", "xin", 18),
                new ComponentDefinitionDao("0xe41d2489571d322189246dafa5ebde1f4699f498", "0x", "zrx", 18),
                #endregion

                #region Centralised Exchanges
                new ComponentDefinitionDao("0xb3104b4b9da82025e8b9f8fb28b3553ce2f67069", "Bibox Token", "bix", 18),
                new ComponentDefinitionDao("0x4375e7ad8a01b8ec3ed041399f62d9cd120e0063", "Bit-Z Token", "bz", 18),
                new ComponentDefinitionDao("0xced4e93198734ddaff8492d525bd258d49eb388e", "Eidoo", "edo", 18),
                new ComponentDefinitionDao("0x50d1c9771902476076ecfc8b2a83ad6b9355a4c9", "FTX Token", "ftt", 18),
                new ComponentDefinitionDao("0x6f259637dcd74c767781e37bc6133cd6a68aa161", "Huobi Token", "ht", 18),
                new ComponentDefinitionDao("0x039b5649a59967e3e936d7471f9c3700100ee1ab", "Kucoin Shares", "kcs", 6),
                new ComponentDefinitionDao("0x2af5d2ad76741191d15dfe7bf6ac92d4bd912ca3", "LEO Token", "leo", 18),
                new ComponentDefinitionDao("0x75231f58b43240c9718dd58b4967c5114342a86c", "OKB", "okb", 18),
                new ComponentDefinitionDao("0x618e75ac90b12c6049ba3b27f5d5f8651b0037f6", "Qash", "qash", 6),
                new ComponentDefinitionDao("0xbd0793332e9fb844a52a205a233ef27a5b34b927", "ZB Token", "zb", 18),
                #endregion

                #region Scaling
                new ComponentDefinitionDao("0x4f9254c83eb525f9fcf346490bbb3ed28a81c667", "Celer Network", "celr", 18),
                new ComponentDefinitionDao("0xa4e8c3ec456107ea67d3075bf9e3df3a75823db0", "Loom Network", "loom", 18),
                new ComponentDefinitionDao("0x7d1afa7b718fb893db30a3abc0cfc608aacfebb0", "Matic Network", "matic", 18),
                new ComponentDefinitionDao("0x255aa6df07540cb5d3d297f0d0d4d84cb52bc8e6", "Raiden Network Token", "rdn", 18),
                #endregion
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
                    "Index composed of tokens from the Messari Lending sector",
                    10,
                    "", FirstJan),
                new IndexDefinitionDao("l1amg", "Asset Management",
                    "Index composed of tokens from the Messari Asset Management sector",
                    10,
                    "", FirstJan),
                new IndexDefinitionDao("l1dex", "Decentralised Exchanges",
                    "Index composed of tokens from the Messari Decentralised Exchange sector",
                    10,
                    "", FirstJan),
                new IndexDefinitionDao("l1cex", "Decentralised Exchanges",
                    "Index composed of tokens from the Messari Centralised Exchange sector",
                    10,
                    "", FirstJan),
                new IndexDefinitionDao("l1sca", "Scalability",
                    "Index composed of tokens from the Messari Scalability sector",
                    10,
                    "", FirstJan)
            };
            await dbCOntext.AddRangeAsync(indexDefinitions, cancellationToken);
            await dbCOntext.SaveChangesAsync(cancellationToken);
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

            var assetManagementWeights = new[]
            {
                new ComponentWeightDao(indexBySymbols["l1amg"], componentBySymbol["bnk"], 0.0283606057934474m),
                new ComponentWeightDao(indexBySymbols["l1amg"], componentBySymbol["bcv"], 0.0166189623155937m),
                new ComponentWeightDao(indexBySymbols["l1amg"], componentBySymbol["bft"], 0.00807837087989956m),
                new ComponentWeightDao(indexBySymbols["l1amg"], componentBySymbol["btm"], 0.208690588791667m),
                new ComponentWeightDao(indexBySymbols["l1amg"], componentBySymbol["c20"], 0.024586921579546m),
                new ComponentWeightDao(indexBySymbols["l1amg"], componentBySymbol["cdt"], 0.00929594714959045m),
                new ComponentWeightDao(indexBySymbols["l1amg"], componentBySymbol["chsb"], 0.0173546031026408m),
                new ComponentWeightDao(indexBySymbols["l1amg"], componentBySymbol["cnd"], 0.0230162273629083m),
                new ComponentWeightDao(indexBySymbols["l1amg"], componentBySymbol["gvt"], 0.00862752467038452m),
                new ComponentWeightDao(indexBySymbols["l1amg"], componentBySymbol["mln"], 0.0051752367973936m),
                new ComponentWeightDao(indexBySymbols["l1amg"], componentBySymbol["mof"], 0.153952468181762m),
                new ComponentWeightDao(indexBySymbols["l1amg"], componentBySymbol["nmr"], 0.0556137703188981m),
                new ComponentWeightDao(indexBySymbols["l1amg"], componentBySymbol["omg"], 0.173563437374606m),
                new ComponentWeightDao(indexBySymbols["l1amg"], componentBySymbol["poly"], 0.0184570876599013m),
                new ComponentWeightDao(indexBySymbols["l1amg"], componentBySymbol["rblx"], 0.00528442211290079m),
                new ComponentWeightDao(indexBySymbols["l1amg"], componentBySymbol["snx"], 0.220877626788818m),
                new ComponentWeightDao(indexBySymbols["l1amg"], componentBySymbol["tbx"], 0.000125557511683702m),
                new ComponentWeightDao(indexBySymbols["l1amg"], componentBySymbol["tct"], 0.0122867081588579m),
                new ComponentWeightDao(indexBySymbols["l1amg"], componentBySymbol["tnb"], 0.0100339334494979m),

            };

            await dbContext.ComponentWeights.AddRangeAsync(assetManagementWeights, cancellationToken);

            var decentralisedExchanges = new[]
            {
                new ComponentWeightDao(indexBySymbols["l1dex"], componentBySymbol["ast"], 0.0151997643954643m),
                new ComponentWeightDao(indexBySymbols["l1dex"], componentBySymbol["bnt"], 0.0637506397924764m),
                new ComponentWeightDao(indexBySymbols["l1dex"], componentBySymbol["dgtx"], 0.108510102162468m),
                new ComponentWeightDao(indexBySymbols["l1dex"], componentBySymbol["hot"], 0.00799719221776441m),
                new ComponentWeightDao(indexBySymbols["l1dex"], componentBySymbol["idex"], 0.0189086366603815m),
                new ComponentWeightDao(indexBySymbols["l1dex"], componentBySymbol["knc"], 0.106497089306184m),
                new ComponentWeightDao(indexBySymbols["l1dex"], componentBySymbol["lrc"], 0.0726976405180716m),
                new ComponentWeightDao(indexBySymbols["l1dex"], componentBySymbol["nec"], 0.0275884456947414m),
                new ComponentWeightDao(indexBySymbols["l1dex"], componentBySymbol["oax"], 0.0138485110144m),
                new ComponentWeightDao(indexBySymbols["l1dex"], componentBySymbol["xin"], 0.265001978238046m),
                new ComponentWeightDao(indexBySymbols["l1dex"], componentBySymbol["zrx"], 0.3m),
            };
            await dbContext.ComponentWeights.AddRangeAsync(decentralisedExchanges, cancellationToken);

            var centralisedExchanges = new[]
            {
                new ComponentWeightDao(indexBySymbols["l1cex"], componentBySymbol["bix"], 0.00929248780205452m),
                new ComponentWeightDao(indexBySymbols["l1cex"], componentBySymbol["bz"], 0.014216253755328m),
                new ComponentWeightDao(indexBySymbols["l1cex"], componentBySymbol["edo"], 0.00785614659446946m),
                new ComponentWeightDao(indexBySymbols["l1cex"], componentBySymbol["ftt"], 0.0305091553304733m),
                new ComponentWeightDao(indexBySymbols["l1cex"], componentBySymbol["ht"], 0.277485014341216m),
                new ComponentWeightDao(indexBySymbols["l1cex"], componentBySymbol["kcs"], 0.0358527808450112m),
                new ComponentWeightDao(indexBySymbols["l1cex"], componentBySymbol["leo"], 0.3m),
                new ComponentWeightDao(indexBySymbols["l1cex"], componentBySymbol["okb"], 0.3m),
                new ComponentWeightDao(indexBySymbols["l1cex"], componentBySymbol["qash"], 0.0116330485567578m),
                new ComponentWeightDao(indexBySymbols["l1cex"], componentBySymbol["zb"], 0.0131551127746894m),
            };
            await dbContext.ComponentWeights.AddRangeAsync(centralisedExchanges, cancellationToken);

            var scalability = new[]
            {
                new ComponentWeightDao(indexBySymbols["l1sca"], componentBySymbol["celr"], 0.268136705575649m),
                new ComponentWeightDao(indexBySymbols["l1sca"], componentBySymbol["loom"], 0.268496511387167m),
                new ComponentWeightDao(indexBySymbols["l1sca"], componentBySymbol["matic"], 0.3m),
                new ComponentWeightDao(indexBySymbols["l1sca"], componentBySymbol["rdn"], 0.163366783037182m),
            };
            await dbContext.ComponentWeights.AddRangeAsync(scalability, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        [Obsolete("only for the old indexes that will disappear")]
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

            await dbContext.IndexCompositions.AddRangeAsync(new[] { cpuComposition, strComposition }, cancellationToken);

            var lendingPricesByDefintions = new Dictionary<IComponentDefinition, decimal>{
                { componentsBySymbols["lnd"], 0.00152077889740119m },
                { componentsBySymbols["akro"], 0.00106594967399099m },
                { componentsBySymbols["bcpt"], 0.0199456639115704m },
                { componentsBySymbols["lba"], 0.0198576158149738m },
                { componentsBySymbols["lend"], 0.0162103877518573m },
                { componentsBySymbols["mkr"], 433.309542948559m },
                { componentsBySymbols["nexo"], 0.09443929731183m },
                { componentsBySymbols["ppt"], 0.339213629017299m },
                { componentsBySymbols["rcn"], 0.0448472125928383m },
                { componentsBySymbols["salt"], 0.0503968166330671m },
            };
            await AddCompositionAndInitialValuations(dbContext, mapper, indexBySymbols["l1len"], lendingPricesByDefintions, componentsBySymbols, cancellationToken);

            var assetManagementValuations = new Dictionary<IComponentDefinition, decimal>{
                { componentsBySymbols["bnk"], 0.00127682717125466m },
                { componentsBySymbols["bcv"], 0.00955292820960845m },
                { componentsBySymbols["bft"], 0.0119812064436263m },
                { componentsBySymbols["btm"], 0.0654961124175985m },
                { componentsBySymbols["c20"], 0.349519615487476m },
                { componentsBySymbols["cdt"], 0.00674435577567889m },
                { componentsBySymbols["chsb"], 0.00952294708958006m },
                { componentsBySymbols["cnd"], 0.00595943153957113m },
                { componentsBySymbols["gvt"], 0.950674194531802m },
                { componentsBySymbols["mln"], 2.8833896725712m },
                { componentsBySymbols["mof"], 1.69544454043322m },
                { componentsBySymbols["nmr"], 6.35947348950127m },
                { componentsBySymbols["omg"], 0.604808110884946m },
                { componentsBySymbols["poly"], 0.0160377918288668m },
                { componentsBySymbols["rblx"], 0.121976877211174m },
                { componentsBySymbols["snx"], 1.20176547271829m },
                { componentsBySymbols["tbx"], 0.00538368570118677m },
                { componentsBySymbols["tct"], 0.0104193472179932m },
                { componentsBySymbols["tnb"], 0.00158556400689227m },
            };
            await AddCompositionAndInitialValuations(dbContext, mapper, indexBySymbols["l1amg"], assetManagementValuations, componentsBySymbols, cancellationToken);

            var decentralisedExchangesValuations = new Dictionary<IComponentDefinition, decimal>{
                { componentsBySymbols["ast"], 0.0177339431816429m },
                { componentsBySymbols["bnt"], 0.256411484845533m },
                { componentsBySymbols["dgtx"], 0.0372852878856344m },
                { componentsBySymbols["hot"], 0.00217223568546613m },
                { componentsBySymbols["idex"], 0.00907486808710847m },
                { componentsBySymbols["knc"], 0.183783456399917m },
                { componentsBySymbols["lrc"], 0.0215384350199587m },
                { componentsBySymbols["nec"], 0.0793305206167633m },
                { componentsBySymbols["oax"], 0.0494125594465681m },
                { componentsBySymbols["xin"], 175.122315555066m },
                { componentsBySymbols["zrx"], 0.180131026909712m },
            };
            await AddCompositionAndInitialValuations(dbContext, mapper, indexBySymbols["l1dex"], decentralisedExchangesValuations, componentsBySymbols, cancellationToken);
            
            var centralisedExchangesValuations = new Dictionary<IComponentDefinition, decimal>{
                { componentsBySymbols["bix"], 0.089493515237409m },
                { componentsBySymbols["bz"], 0.173237660031493m },
                { componentsBySymbols["edo"], 0.18852616414119m },
                { componentsBySymbols["ftt"], 2.13790281590313m },
                { componentsBySymbols["ht"], 2.7370290011753m },
                { componentsBySymbols["kcs"], 0.905953652540513m },
                { componentsBySymbols["leo"], 0.814762934840878m },
                { componentsBySymbols["okb"], 2.60909080011118m },
                { componentsBySymbols["qash"], 0.0449816356152302m },
                { componentsBySymbols["zb"], 0.18506306899m },

            };
            await AddCompositionAndInitialValuations(dbContext, mapper, indexBySymbols["l1cex"], centralisedExchangesValuations, componentsBySymbols, cancellationToken);

            var scalabilityValuations = new Dictionary<IComponentDefinition, decimal>{
                { componentsBySymbols["celr"], 0.00354795542862913m },
                { componentsBySymbols["loom"], 0.0153843850233695m },
                { componentsBySymbols["matic"], 0.013721005734545m },
                { componentsBySymbols["rdn"], 0.114910875756317m },
            };
            await AddCompositionAndInitialValuations(dbContext, mapper, indexBySymbols["l1sca"], scalabilityValuations, componentsBySymbols, cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        private static async Task AddCompositionAndInitialValuations(IndexRepositoryContext dbContext, IMapper mapper,
            IndexDefinitionDao indexDefinitionDao, Dictionary<IComponentDefinition, decimal> pricesByDefinitions,
            Dictionary<string, ComponentDefinitionDao> componentsBySymbols,
            CancellationToken cancellationToken)
        {
            var compositions = IndexCompositionCalculator.CalculateIndexComposition(indexDefinitionDao,
                pricesByDefinitions, 100m, 1,
                FirstJan);

            var compositionDao = mapper.Map<IndexCompositionDao>(compositions);
            compositionDao.ComponentQuantityDaos.ForEach(q => q.LinkToIndexComposition(compositionDao));
            await dbContext.IndexCompositions.AddAsync(compositionDao, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            var componentValuations = pricesByDefinitions.Select(p => new ComponentValuationDao(
                compositionDao.ComponentQuantityDaos.Single(q => q.ComponentDefinition.Symbol == p.Key.Symbol),
                FirstJan, Usdc, pricesByDefinitions[componentsBySymbols[p.Key.Symbol]])).ToList();

            await dbContext.ComponentValuations.AddRangeAsync(componentValuations, cancellationToken);

            var lenValuation = new IndexValuationDao(componentValuations);
            await dbContext.IndexValuations.AddAsync(lenValuation, cancellationToken);
        }

        [Obsolete("ony for the old indexes")]
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

        [Obsolete("ony for the old indexes")]
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