using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Trakx.Common.Composition;
using Trakx.Common.Interfaces.Indice;
using Trakx.Persistence.DAO;

[assembly: InternalsVisibleTo("Trakx.Tests")]

namespace Trakx.Persistence.Initialisation
{
    public class DatabaseInitialiser : IDatabaseInitialiser
    {
        private readonly IndiceRepositoryContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<DatabaseInitialiser> _logger;
        private const string Usdc = "usdc";

        public DatabaseInitialiser(IndiceRepositoryContext dbContext,
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
            if(!_dbContext.IndiceDefinitions.Any())
                await SeedIndiceDefinitions().ConfigureAwait(false);
        }

        private async Task Migrate()
        {
            _logger.LogInformation("Running Migrations.");
            await _dbContext.Database.MigrateAsync().ConfigureAwait(false);
        }

        private async Task SeedIndiceDefinitions()
        {
            _logger.LogInformation("Adding known indicees to database.");
            await AddKnownIndicees(_dbContext, _mapper).ConfigureAwait(false);

            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        internal static async Task AddKnownIndicees(IndiceRepositoryContext dbContext, IMapper mapper)
        {
            using var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            await CreateComponentDefinitions(dbContext, cancellationToken);

            await CreateIndiceDefinitions(dbContext, cancellationToken);

            await CreateIndiceCompositions(dbContext, mapper, cancellationToken);

            await AddOnChainAddresses(dbContext, cancellationToken);
        }

        private static async Task AddOnChainAddresses(IndiceRepositoryContext dbContext, CancellationToken cancellationToken)
        {
            var indiceBySymbols = (await dbContext.IndiceDefinitions.ToListAsync(cancellationToken)).ToDictionary(i => i.Symbol, i => i);
            var compositionsBySymbols = (await dbContext.IndiceCompositions.ToListAsync(cancellationToken)).ToDictionary(i => i.Symbol, i => i);

            compositionsBySymbols["l1amg2001"].Address = "0xa0ae553f3bcec040d7d1922c820f0cd216c13b3d";
            compositionsBySymbols["l1cex2001"].Address = "0x852f92fe812a4a357c88c7f02ac38597933a51d2";
            compositionsBySymbols["l1dex2001"].Address = "0xae81ae0179b38588e05f404e05882a3965d1b415";
            compositionsBySymbols["l1len2001"].Address = "0x4bbb210cd20441e9db0b70b32f4b4b9348edd36a";
            compositionsBySymbols["l1sca2001"].Address = "0x48a4e297b2ada9d48906fc41a6aaa3c73f17d0e7";

            compositionsBySymbols["l1amg2003"].Address = "0x4fa9c835f719257db50c3cd6bcbc037cd9224f29";
            compositionsBySymbols["l1cex2003"].Address = "0x4fff61c7da70676560ea5694217ecced9b057cba";
            compositionsBySymbols["l1dex2003"].Address = "0x1314e907bd1dbfcee99191e9b0fbc2b98cbba1a8";
            compositionsBySymbols["l1len2003"].Address = "0xb3455eb131cf2de3a3422b3ae2f1a1d4da782cdd";
            compositionsBySymbols["l1sca2003"].Address = "0xe21aa68a5f9832a3eddee632b69aba9150e6bcee";

            compositionsBySymbols["l1cex2004"].Address = "0xbdbf5cdee96ebe6fcce221de9bae8dcade0bb4e6";
            compositionsBySymbols["l1dex2004"].Address = "0x012849785647d40a96cc09cb34ac049656c959b9";
            compositionsBySymbols["l1mc10erc2004"].Address = "0x2466f531412506899b2ceb2acdc76e4a1985c543";
            compositionsBySymbols["l1btceth2004"].Address = "0x6364bc548cbf79af5127a56566f5a38202329c1e";
            compositionsBySymbols["l1vol15btc2004"].Address = "0xbb7ef115564ff2a14f832dcb500bb43a1e7471b5";
            compositionsBySymbols["l1vol20be2004"].Address = "0x819e7dab64486123fce3b2b2f8f260baf318fcff";

            compositionsBySymbols["l1cex2005"].Address = "0xc5474b40d7607becdafcb05873bf31e256c3929a";
            compositionsBySymbols["l1dex2005"].Address = "0x58ce1cafaeb7133bacd622b2cea57ae3e771f064";
            compositionsBySymbols["l1len2005"].Address = "0xf4c4c77442059cc38b6a17ff0596d10322dd52d3";
            compositionsBySymbols["l1mc10erc2005"].Address = "0xa4264e6be8c7dc00bf62d68ea7c0ac78ba1e061a";
            compositionsBySymbols["l1btceth2005"].Address = "0x716b48a91cd9802ff1d2bcd3b123766364d49107";
            compositionsBySymbols["l1vol15btc2005"].Address = "0x9d810e75d4aa3c15df884fe4fc9b215f0bebfba5";
            compositionsBySymbols["l1vol20be2005"].Address = "0x9d4b4e3fae0b78949387cbc70de93a6481318932";

            indiceBySymbols["l1amg"].Address = "0x7b0ef33d7d91f4d0f7e49e72fbe50d27522cf857";
            indiceBySymbols["l1cex"].Address = "0x90150b7c698e5c490198fa6537a7ea3a3e24aa5c";
            indiceBySymbols["l1dex"].Address = "0x028618150584251dd3145aaf4aca3e288a87aeb7";
            indiceBySymbols["l1len"].Address = "0x1fce55ee9d3f076e4917d0ef677f1675b1fc2930";
            indiceBySymbols["l1sca"].Address = "0xaef0a7523b04f5643a496a66d89ab6c2901f03ce";
            indiceBySymbols["l1mc10erc"].Address = "0x8ae4fb17d69938c384192f7c5b4e059857aadcab";
            indiceBySymbols["l1btceth"].Address = "0xd158f5ec1016cdabf54a8b978acdb7edcae8bd18";
            indiceBySymbols["l1vol15btc"].Address = "0xa6ac49d59bb34c19b8ad914fb41cb34c9ccdada6";
            indiceBySymbols["l1vol20be"].Address = "0x686e201815ea0f23c8d4b5fc7a09f2d683686111";

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        internal static async Task CreateComponentDefinitions(IndiceRepositoryContext dbContext, CancellationToken cancellationToken)
        {
            var componentDefinitions = new List<ComponentDefinitionDao>()
            {
                #region Asset Management
                new ComponentDefinitionDao("0xc80c5e40220172b36adee2c951f26f2a577810c5", "Bankera", "bnk", "bankera", 8),
                new ComponentDefinitionDao("0x1014613e2b3cbc4d575054d4982e580d9b99d7b1", "BitCapitalVendor", "bcv", "bcv", 8),
                new ComponentDefinitionDao("0x01ff50f8b7f74e4f00580d9596cd3d0d6d6e326f", "BnkToTheFuture", "bft", "bnktothefuture", 18),
                new ComponentDefinitionDao("0x26e75307fc0c021472feb8f727839531f112f317", "Crypto20", "c20", "crypto20", 18),
                new ComponentDefinitionDao("0x177d39ac676ed1c67a2b268ad7f1e58826e5b0af", "Blox", "cdt", "blox", 18),
                new ComponentDefinitionDao("0xba9d4199fab4f26efe3551d490e3821486f135ba", "Swissborg", "chsb", "swissborg", 8),
                new ComponentDefinitionDao("0xd4c435f5b09f855c3317c8524cb1f586e42795fa", "Cindicator", "cnd", "cindicator", 18),
                new ComponentDefinitionDao("0x103c3a209da59d3e7c4a89307e66521e081cfdf0", "Genesis Vision", "gvt", "genesis-vision", 18),
                new ComponentDefinitionDao("0xec67005c4E498Ec7f55E092bd1d35cbC47C91892", "Melon", "mln", "melon", 18),
                new ComponentDefinitionDao("0x653430560be843c4a3d143d0110e896c2ab8ac0d", "Molecular Future", "mof", "molecular-future", 16),
                new ComponentDefinitionDao("0x1776e1F26f98b1A5dF9cD347953a26dd3Cb46671", "Numeraire", "nmr", "numeraire", 18),
                new ComponentDefinitionDao("0xd26114cd6EE289AccF82350c8d8487fedB8A0C07", "OmiseGo", "omg", "omisego", 18),
                new ComponentDefinitionDao("0x9992ec3cf6a55b00978cddf2b27bc6882d88d1ec", "Polymath Network", "poly", "polymath-network", 18),
                new ComponentDefinitionDao("0xFc2C4D8f95002C14eD0a7aA65102Cac9e5953b5E", "Rublix", "rblx", "rublix", 18),
                new ComponentDefinitionDao("0xa31b1767e09f842ecfd4bc471fe44f830e3891aa", "Robee", "roobee", "roobee", 18),
                new ComponentDefinitionDao("0xc011a72400e58ecd99ee497cf89e3775d4bd732f", "Synthetix Network Token", "snx", "havven", 18),
                new ComponentDefinitionDao("0x3a92bd396aef82af98ebc0aa9030d25a23b11c6b", "Tokenbox", "tbx", "tokenbox", 18),
                new ComponentDefinitionDao("0x4824a7b64e3966b0133f4f4ffb1b9d6beb75fff7", "TokenClub", "tct", "tokenclub", 18),
                new ComponentDefinitionDao("0xf7920b0768ecb20a123fac32311d07d193381d6f", "Time New Bank  ", "tnb", "time-new-bank", 18),
                #endregion

                #region Centralised Exchanges
                new ComponentDefinitionDao("0xb3104b4b9da82025e8b9f8fb28b3553ce2f67069", "Bibox Token", "bix", "bibox-token", 18),
                new ComponentDefinitionDao("0x4375e7ad8a01b8ec3ed041399f62d9cd120e0063", "Bit-Z Token", "bz", "bit-z-token", 18),
                new ComponentDefinitionDao("0xced4e93198734ddaff8492d525bd258d49eb388e", "Eidoo", "edo", "eidoo", 18),
                new ComponentDefinitionDao("0x50d1c9771902476076ecfc8b2a83ad6b9355a4c9", "FTX Token", "ftt", "ftx-token", 18),
                new ComponentDefinitionDao("0x6f259637dcd74c767781e37bc6133cd6a68aa161", "Huobi Token", "ht", "huobi-token", 18),
                new ComponentDefinitionDao("0x039b5649a59967e3e936d7471f9c3700100ee1ab", "Kucoin Shares", "kcs", "kucoin-shares", 6),
                new ComponentDefinitionDao("0x2af5d2ad76741191d15dfe7bf6ac92d4bd912ca3", "LEO Token", "leo", "leo-token", 18),
                new ComponentDefinitionDao("0x75231f58b43240c9718dd58b4967c5114342a86c", "OKB", "okb", "okb", 18),
                new ComponentDefinitionDao("0x618e75ac90b12c6049ba3b27f5d5f8651b0037f6", "Qash", "qash", "qash", 6),
                new ComponentDefinitionDao("0xbd0793332e9fb844a52a205a233ef27a5b34b927", "ZB Token", "zb", "zb-token", 18),
                #endregion

                #region Decentralised Exchanges
                new ComponentDefinitionDao("0x27054b13b1b798b345b591a4d22e6562d47ea75a", "AirSwap", "ast", "airswap", 4),
                new ComponentDefinitionDao("0x1f573d6fb3f13d689ff844b4ce37794d79a7ff1c", "Bancor Network Token", "bnt", "bancor", 18),
                new ComponentDefinitionDao("0x1c83501478f1320977047008496dacbd60bb15ef", "Digitex Futures Exchange", "dgtx", "digitex-futures-exchange", 18),
                new ComponentDefinitionDao("0x9af839687f6c94542ac5ece2e317daae355493a1", "Hydro Protocol", "hot", "hydro-protocol", 18),
                new ComponentDefinitionDao("0xB705268213D593B8FD88d3FDEFF93AFF5CbDcfAE", "Idex", "idex", "aurora-dao", 18),
                new ComponentDefinitionDao("0xdd974d5c2e2928dea5f71b9825b8b646686bd200", "Kyber Network", "knc", "kyber-network", 18),
                new ComponentDefinitionDao("0xbbbbca6a901c926f240b89eacb641d8aec7aeafd", "Loopring", "lrc", "loopring", 18),
                new ComponentDefinitionDao("0xcc80c051057b774cd75067dc48f8987c4eb97a5e", "Nectar Token", "nec", "nectar-token", 18),
                new ComponentDefinitionDao("0x701c244b988a513c945973defa05de933b23fe1d", "Token OpenANX", "oax", "openanx", 18),
                new ComponentDefinitionDao("0xa974c709cfb4566686553a20790685a47aceaa33", "Mixin", "xin", "mixin", 18),
                new ComponentDefinitionDao("0xe41d2489571d322189246dafa5ebde1f4699f498", "0x", "zrx", "0x", 18),
                #endregion

                #region Lending
                new ComponentDefinitionDao("0x0947b0e6d821378805c9598291385ce7c791a6b2", "Lendingblock", "lnd", "lendingblock", 18),
                new ComponentDefinitionDao("0x8ab7404063ec4dbcfd4598215992dc3f8ec853d7", "Akropolis", "akro", "akropolis", 18),
                new ComponentDefinitionDao("0x1c4481750daa5ff521a2a7490d9981ed46465dbd", "Blockmason Credit Protocol", "bcpt", "blockmason-credit-protocol", 18),
                new ComponentDefinitionDao("0xfe5f141bf94fe84bc28ded0ab966c16b17490657", "Cred", "lba", "libra-credit", 18),
                new ComponentDefinitionDao("0x80fB784B7eD66730e8b1DBd9820aFD29931aab03", "Aave", "lend", "ethlend", 18),
                new ComponentDefinitionDao("0x9f8f72aa9304c8b593d555f12ef6589cc3a579a2", "Maker", "mkr", "maker", 18),
                new ComponentDefinitionDao("0xb62132e35a6c13ee1ee0f84dc5d40bad8d815206", "nexo", "nexo", "nexo", 18),
                new ComponentDefinitionDao("0xd4fa1460f537bb9085d22c7bccb5dd450ef28e3a", "Populous", "ppt", "populous", 8),
                new ComponentDefinitionDao("0xf970b8e36e23f7fc3fd752eea86f8be8d83375a6", "Ripio Credit Network", "rcn", "ripio-credit-network", 18),
                new ComponentDefinitionDao("0x4156D3342D5c385a87D264F90653733592000581", "SALT", "salt", "salt", 8),
                #endregion

                #region Scaling
                new ComponentDefinitionDao("0x4f9254c83eb525f9fcf346490bbb3ed28a81c667", "Celer Network", "celr", "celer-network", 18),
                new ComponentDefinitionDao("0xa4e8c3ec456107ea67d3075bf9e3df3a75823db0", "Loom Network", "loom", "loom-network", 18),
                new ComponentDefinitionDao("0x7d1afa7b718fb893db30a3abc0cfc608aacfebb0", "Matic Network", "matic", "matic-network", 18),
                new ComponentDefinitionDao("0x255aa6df07540cb5d3d297f0d0d4d84cb52bc8e6", "Raiden Network Token", "rdn", "raiden-network", 18),
                #endregion

                #region Wrapped Tokens
                new ComponentDefinitionDao("0x2260fac5e5542a773aa44fbcfedf7c193bc2c599", "Wrapped BTC", "wbtc", "wrapped-bitcoin", 8),
                new ComponentDefinitionDao("0xc02aaa39b223fe8d0a0e5c4f27ead9083c756cc2", "Wrapped ETH", "weth", "weth", 18),
                #endregion

                #region Top 10 Erc
                new ComponentDefinitionDao("0x514910771af9ca656af840dff83e8264ecf986ca", "ChainLink Token", "link", "chainlink", 18),
                new ComponentDefinitionDao("0xb1eef147028e9f480dbc5ccaa3277d417d1b85f0", "Seele Token", "seele", "seele", 18),
                new ComponentDefinitionDao("0x0d8775f648430679a709e98d2b0cb6250d2887ef", "Basic Attention Token", "bat", "basic-attention-token", 18),
                #endregion

                new ComponentDefinitionDao("0xa0b86991c6218b36c1d19d4a2e9eb0ce3606eb48", "USD Coin", "usdc", "usd-coin", 6),
                
            };
            await dbContext.ComponentDefinitions.AddRangeAsync(componentDefinitions, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        internal static async Task CreateIndiceDefinitions(IndiceRepositoryContext dbContext, CancellationToken cancellationToken)
        {
            var firstJan = new DateTime(2020, 1, 1);
            var firstApr = new DateTime(2020, 4, 1);
            var indiceDefinitions = new List<IndiceDefinitionDao>
            {
                new IndiceDefinitionDao("l1amg", "Asset Management",
                    "Index composed of tokens from the Messari Asset Management sector",
                    10,
                    "", firstJan),
                new IndiceDefinitionDao("l1cex", "Centralised Exchanges",
                    "Index composed of tokens from the Messari Centralised Exchange sector, capped at 30%",
                    13,
                    "", firstJan),
                new IndiceDefinitionDao("l1dex", "Decentralised Exchanges",
                    "Index composed of tokens from the Messari Decentralised Exchange sector, capped at 50%",
                    14,
                    "", firstJan),
                new IndiceDefinitionDao("l1len", "Lending",
                    "Index composed of tokens from the Messari Lending sector",
                    10,
                    "", firstJan),
                new IndiceDefinitionDao("l1sca", "Scalability",
                    "Index composed of tokens from the Messari Scalability sector",
                    11,
                    "", firstJan),
                new IndiceDefinitionDao("l1mc10erc", "Top 10 Ethereum",
                    "Index composed of the top 10 ERC20 by market cap, capped at 15%",
                    14,
                    "", firstApr),
                new IndiceDefinitionDao("l1btceth", "Bitcoin Ether 50/50",
                    "Index composed of half Bitcoin and half Ether",
                    14,
                    "", firstApr),
                new IndiceDefinitionDao("l1vol15btc", "Bitcoin Control15",
                    "Index composed of Bitcoin with a risk control mechanism",
                    14,
                    "", firstApr),
                new IndiceDefinitionDao("l1vol20be", "Bitcoin Ether Control20",
                    "Index composed of half Bitcoin and half Ether with a risk control mechanism",
                    14,
                    "", firstApr),
            };
            await dbContext.AddRangeAsync(indiceDefinitions, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public struct CompositionData
        {
            public CompositionData(IndiceDefinitionDao indiceDefinition, DateTime asOf, ComponentDefinitionDao componentDefinition,
                decimal price, decimal targetWeight)
            {
                AsOf = asOf;
                ComponentDefinition = componentDefinition;
                IndiceDefinition = indiceDefinition;
                PriceAndWeight = new PriceAndTargetWeight(price, targetWeight);
            }
            public DateTime AsOf { get; }
            public PriceAndTargetWeight PriceAndWeight { get; }
            public ComponentDefinitionDao ComponentDefinition { get; }
            public IndiceDefinitionDao IndiceDefinition { get; }
        }

        private static async Task CreateIndiceCompositions(IndiceRepositoryContext dbContext, IMapper mapper, CancellationToken cancellationToken)
        {
            var indiceBySymbols = (await dbContext.IndiceDefinitions.ToListAsync(cancellationToken)).ToDictionary(i => i.Symbol, i => i);
            var componentsBySymbols = (await dbContext.ComponentDefinitions.ToListAsync(cancellationToken)).ToDictionary(i => i.Symbol, i => i); 

            var allCompositionData = new List<CompositionData>{
                
                #region Jan 2020
                new CompositionData(indiceBySymbols["l1amg"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["bnk"], 0.00127682717125466m, 0.0499844599306641m),
                new CompositionData(indiceBySymbols["l1amg"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["bcv"], 0.00955292820960845m, 0.0294006779631108m),
                new CompositionData(indiceBySymbols["l1amg"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["bft"], 0.0119812064436263m, 0.014428525868912m),
                new CompositionData(indiceBySymbols["l1amg"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["c20"], 0.349519615487476m, 0.043368972650723m),
                new CompositionData(indiceBySymbols["l1amg"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["cdt"], 0.00674435577567889m, 0.0165630076824209m),
                new CompositionData(indiceBySymbols["l1amg"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["chsb"], 0.00952294708958006m, 0.0306902989259322m),
                new CompositionData(indiceBySymbols["l1amg"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["cnd"], 0.00595943153957113m, 0.0406154545973136m),
                new CompositionData(indiceBySymbols["l1amg"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["gvt"], 0.950674194531802m, 0.0153912243115378m),
                new CompositionData(indiceBySymbols["l1amg"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["mln"], 2.8833896725712m, 0.00933916345643658m),
                new CompositionData(indiceBySymbols["l1amg"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["mof"], 1.69544454043322m, 0.270154278336068m),
                new CompositionData(indiceBySymbols["l1amg"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["nmr"], 6.35947348950127m, 0.0977608378777385m),
                new CompositionData(indiceBySymbols["l1amg"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["omg"], 0.604808110884946m, 0.3m),
                new CompositionData(indiceBySymbols["l1amg"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["poly"], 0.0160377918288668m, 0.0326230182928632m),
                new CompositionData(indiceBySymbols["l1amg"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["rblx"], 0.121976877211174m, 0.0095305716466697m),
                new CompositionData(indiceBySymbols["l1amg"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["tbx"], 0.00538368570118677m, 0.000486782814904515m),
                new CompositionData(indiceBySymbols["l1amg"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["tct"], 0.0104193472179932m, 0.0218059851793901m),
                new CompositionData(indiceBySymbols["l1amg"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["tnb"], 0.00158556400689227m, 0.0178567404653135m),
                new CompositionData(indiceBySymbols["l1cex"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["bix"], 0.089493515237409m, 0.00929248780205452m),
                new CompositionData(indiceBySymbols["l1cex"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["bz"], 0.173237660031493m, 0.014216253755328m),
                new CompositionData(indiceBySymbols["l1cex"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["edo"], 0.18852616414119m, 0.00785614659446946m),
                new CompositionData(indiceBySymbols["l1cex"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["ftt"], 2.13790281590313m, 0.0305091553304733m),
                new CompositionData(indiceBySymbols["l1cex"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["ht"], 2.7370290011753m, 0.277485014341216m),
                new CompositionData(indiceBySymbols["l1cex"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["kcs"], 0.905953652540513m, 0.0358527808450112m),
                new CompositionData(indiceBySymbols["l1cex"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["leo"], 0.814762934840878m, 0.3m),
                new CompositionData(indiceBySymbols["l1cex"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["okb"], 2.60909080011118m, 0.3m),
                new CompositionData(indiceBySymbols["l1cex"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["qash"], 0.0449816356152302m, 0.0116330485567578m),
                new CompositionData(indiceBySymbols["l1cex"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["zb"], 0.18506306899m, 0.0131551127746894m),
                new CompositionData(indiceBySymbols["l1dex"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["ast"], 0.0177339431816429m, 0.0151997643954643m),
                new CompositionData(indiceBySymbols["l1dex"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["bnt"], 0.256411484845533m, 0.0637506397924764m),
                new CompositionData(indiceBySymbols["l1dex"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["dgtx"], 0.0372852878856344m, 0.108510102162468m),
                new CompositionData(indiceBySymbols["l1dex"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["hot"], 0.00217223568546613m, 0.00799719221776441m),
                new CompositionData(indiceBySymbols["l1dex"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["idex"], 0.00907486808710847m, 0.0189086366603815m),
                new CompositionData(indiceBySymbols["l1dex"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["knc"], 0.183783456399917m, 0.106497089306184m),
                new CompositionData(indiceBySymbols["l1dex"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["lrc"], 0.0215384350199587m, 0.0726976405180716m),
                new CompositionData(indiceBySymbols["l1dex"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["nec"], 0.0793305206167633m, 0.0275884456947414m),
                new CompositionData(indiceBySymbols["l1dex"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["oax"], 0.0494125594465681m, 0.0138485110144m),
                new CompositionData(indiceBySymbols["l1dex"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["xin"], 175.122315555066m, 0.265001978238046m),
                new CompositionData(indiceBySymbols["l1dex"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["zrx"], 0.180131026909712m, 0.3m),
                new CompositionData(indiceBySymbols["l1len"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["lnd"], 0.00152077889740119m, 0.0487275092314901m),
                new CompositionData(indiceBySymbols["l1len"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["akro"], 0.00106594967399099m, 0.048267987428224m),
                new CompositionData(indiceBySymbols["l1len"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["bcpt"], 0.0199456639115704m, 0.0514905531675755m),
                new CompositionData(indiceBySymbols["l1len"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["lba"], 0.0198576158149738m, 0.075050561708752m),
                new CompositionData(indiceBySymbols["l1len"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["lend"], 0.0162103877518573m, 0.0865759511672131m),
                new CompositionData(indiceBySymbols["l1len"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["mkr"], 433.309542948559m, 0.3m),
                new CompositionData(indiceBySymbols["l1len"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["nexo"], 0.09443929731183m, 0.164041680816541m),
                new CompositionData(indiceBySymbols["l1len"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["ppt"], 0.339213629017299m, 0.0735535096292126m),
                new CompositionData(indiceBySymbols["l1len"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["rcn"], 0.0448472125928383m, 0.0969502811373821m),
                new CompositionData(indiceBySymbols["l1len"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["salt"], 0.0503968166330671m, 0.0553419657136083m),
                new CompositionData(indiceBySymbols["l1sca"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["celr"], 0.00354795542862913m, 0.268136705575649m),
                new CompositionData(indiceBySymbols["l1sca"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["loom"], 0.0153843850233695m, 0.268496511387167m),
                new CompositionData(indiceBySymbols["l1sca"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["matic"], 0.013721005734545m, 0.3m),
                new CompositionData(indiceBySymbols["l1sca"], DateTime.Parse("01-Jan-2020"), componentsBySymbols["rdn"], 0.114910875756317m, 0.163366783037182m),
                #endregion

                #region Mar 2020
                new CompositionData(indiceBySymbols["l1amg"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["bnk"], 0.00179927067940598m, 0.0206910168488897m),
                new CompositionData(indiceBySymbols["l1amg"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["bcv"], 0.00652393360098039m, 0.0224773899135165m),
                new CompositionData(indiceBySymbols["l1amg"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["bft"], 0.0131152454045855m, 0.0185651815233328m),
                new CompositionData(indiceBySymbols["l1amg"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["c20"], 0.540455014732873m, 0.0623488255922278m),
                new CompositionData(indiceBySymbols["l1amg"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["cdt"], 0.0057474571339946m, 0.0170788650764569m),
                new CompositionData(indiceBySymbols["l1amg"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["chsb"], 0.0220050845036541m, 0.0561135181181634m),
                new CompositionData(indiceBySymbols["l1amg"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["cnd"], 0.00584859187225522m, 0.0406552514229226m),
                new CompositionData(indiceBySymbols["l1amg"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["gvt"], 1.01170709001647m, 0.0190193349760296m),
                new CompositionData(indiceBySymbols["l1amg"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["mln"], 4.1985094120947m, 0.0171306684413808m),
                new CompositionData(indiceBySymbols["l1amg"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["mof"], 0.730657583001441m, 0.166762958390392m),
                new CompositionData(indiceBySymbols["l1amg"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["nmr"], 8.65344953265644m, 0.132911321110343m),
                new CompositionData(indiceBySymbols["l1amg"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["omg"], 0.835872986683654m, 0.3m),
                new CompositionData(indiceBySymbols["l1amg"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["poly"], 0.024806747751792m, 0.0534041506672715m),
                new CompositionData(indiceBySymbols["l1amg"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["rblx"], 0.148482670829074m, 0.0148501651319708m),
                new CompositionData(indiceBySymbols["l1amg"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["roobee"], 0.00335377444363906m, 0.0126866596985958m),
                new CompositionData(indiceBySymbols["l1amg"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["tbx"], 0.00940074719338648m, 0.00501693777569143m),
                new CompositionData(indiceBySymbols["l1amg"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["tct"], 0.00822078969971243m, 0.0199390244086283m),
                new CompositionData(indiceBySymbols["l1amg"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["tnb"], 0.00157653631604079m, 0.0203487309041854m),
                new CompositionData(indiceBySymbols["l1cex"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["bix"], 0.145596531807524m, 0.0171355792907843m),
                new CompositionData(indiceBySymbols["l1cex"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["bz"], 0.166302026278831m, 0.0182761299655992m),
                new CompositionData(indiceBySymbols["l1cex"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["edo"], 0.17774961790996m, 0.0145541186171703m),
                new CompositionData(indiceBySymbols["l1cex"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["ftt"], 2.58473124840741m, 0.0316051364819033m),
                new CompositionData(indiceBySymbols["l1cex"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["ht"], 4.83808171621169m, 0.296093156309471m),
                new CompositionData(indiceBySymbols["l1cex"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["kcs"], 1.18220253951047m, 0.0370531129773155m),
                new CompositionData(indiceBySymbols["l1cex"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["leo"], 0.958145980936401m, 0.24661621522009m),
                new CompositionData(indiceBySymbols["l1cex"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["okb"], 5.83293331012433m, 0.3m),
                new CompositionData(indiceBySymbols["l1cex"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["qash"], 0.0477015136106287m, 0.0172279155657657m),
                new CompositionData(indiceBySymbols["l1cex"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["zb"], 0.320086927593225m, 0.0214386355718992m),
                new CompositionData(indiceBySymbols["l1dex"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["ast"], 0.0194412304573582m, 0.00676533002410039m),
                new CompositionData(indiceBySymbols["l1dex"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["bnt"], 0.267657854254685m, 0.0379004104610325m),
                new CompositionData(indiceBySymbols["l1dex"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["dgtx"], 0.0374799919300932m, 0.0653199601275881m),
                new CompositionData(indiceBySymbols["l1dex"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["hot"], 0.00225402160447076m, 0.00168001323598462m),
                new CompositionData(indiceBySymbols["l1dex"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["idex"], 0.0139369686750128m, 0.0141626027644421m),
                new CompositionData(indiceBySymbols["l1dex"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["knc"], 0.644178083807428m, 0.231808637119559m),
                new CompositionData(indiceBySymbols["l1dex"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["lrc"], 0.0405873640470377m, 0.0934633115470443m),
                new CompositionData(indiceBySymbols["l1dex"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["nec"], 0.0850702023391386m, 0.0139569409665785m),
                new CompositionData(indiceBySymbols["l1dex"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["oax"], 0.0469286539476105m, 0.00487614342809933m),
                new CompositionData(indiceBySymbols["l1dex"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["xin"], 246.83385038913m, 0.241312397478017m),
                new CompositionData(indiceBySymbols["l1dex"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["zrx"], 0.228677970080333m, 0.288754252847552m),
                new CompositionData(indiceBySymbols["l1len"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["lnd"], 0.00155359312006652m, 0.0484664099619815m),
                new CompositionData(indiceBySymbols["l1len"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["akro"], 0.00252665346875605m, 0.0511204180654679m),
                new CompositionData(indiceBySymbols["l1len"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["bcpt"], 0.0219315246073939m, 0.0504731748260241m),
                new CompositionData(indiceBySymbols["l1len"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["lba"], 0.0187571617369506m, 0.0648896877646399m),
                new CompositionData(indiceBySymbols["l1len"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["lend"], 0.0264762931461642m, 0.091218912537611m),
                new CompositionData(indiceBySymbols["l1len"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["mkr"], 553.603684322521m, 0.3m),
                new CompositionData(indiceBySymbols["l1len"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["nexo"], 0.154411606115267m, 0.176357800705126m),
                new CompositionData(indiceBySymbols["l1len"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["ppt"], 0.427505548392617m, 0.0698816909889621m),
                new CompositionData(indiceBySymbols["l1len"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["rcn"], 0.0588212835802107m, 0.091477052036923m),
                new CompositionData(indiceBySymbols["l1len"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["salt"], 0.0786002649008706m, 0.0561148531132636m),
                new CompositionData(indiceBySymbols["l1sca"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["celr"], 0.00331769744726571m, 0.245006056678801m),
                new CompositionData(indiceBySymbols["l1sca"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["loom"], 0.0189526689318092m, 0.283327216553418m),
                new CompositionData(indiceBySymbols["l1sca"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["matic"], 0.0202222588096855m, 0.3m),
                new CompositionData(indiceBySymbols["l1sca"], DateTime.Parse("01-Mar-2020"), componentsBySymbols["rdn"], 0.120808864365804m, 0.17166672676778m),
                #endregion

                #region Apr 2020

                new CompositionData(indiceBySymbols["l1cex"], DateTime.Parse("01-Apr-2020"), componentsBySymbols["ftt"], 2.414m, 0.3m),
                new CompositionData(indiceBySymbols["l1cex"], DateTime.Parse("01-Apr-2020"), componentsBySymbols["ht"], 3.299m, 0.2918m),
                new CompositionData(indiceBySymbols["l1cex"], DateTime.Parse("01-Apr-2020"), componentsBySymbols["kcs"], 0.9446m, 0.0349m),
                new CompositionData(indiceBySymbols["l1cex"], DateTime.Parse("01-Apr-2020"), componentsBySymbols["leo"], 1.044m, 0.2697m),
                new CompositionData(indiceBySymbols["l1cex"], DateTime.Parse("01-Apr-2020"), componentsBySymbols["okb"], 4.271m, 0.1036m),
                
                new CompositionData(indiceBySymbols["l1dex"], DateTime.Parse("01-Apr-2020"), componentsBySymbols["knc"], 0.4395m, 0.3848m),
                new CompositionData(indiceBySymbols["l1dex"], DateTime.Parse("01-Apr-2020"), componentsBySymbols["lrc"], 0.02635m, 0.132m),
                new CompositionData(indiceBySymbols["l1dex"], DateTime.Parse("01-Apr-2020"), componentsBySymbols["zrx"], 0.1521m, 0.4832m),

                new CompositionData(indiceBySymbols["l1mc10erc"], DateTime.Parse("01-Apr-2020"), componentsBySymbols["ftt"], 2.414m, 0.15m),
                new CompositionData(indiceBySymbols["l1mc10erc"], DateTime.Parse("01-Apr-2020"), componentsBySymbols["ht"], 3.299m, 0.15m),
                new CompositionData(indiceBySymbols["l1mc10erc"], DateTime.Parse("01-Apr-2020"), componentsBySymbols["mkr"], 295.02m, 0.0982m),
                new CompositionData(indiceBySymbols["l1mc10erc"], DateTime.Parse("01-Apr-2020"), componentsBySymbols["leo"], 1.044m, 0.15m),
                new CompositionData(indiceBySymbols["l1mc10erc"], DateTime.Parse("01-Apr-2020"), componentsBySymbols["okb"], 4.271m, 0.0883m),
                new CompositionData(indiceBySymbols["l1mc10erc"], DateTime.Parse("01-Apr-2020"), componentsBySymbols["link"], 2.209m, 0.15m),
                new CompositionData(indiceBySymbols["l1mc10erc"], DateTime.Parse("01-Apr-2020"), componentsBySymbols["knc"], 0.4395m, 0.0446m),
                new CompositionData(indiceBySymbols["l1mc10erc"], DateTime.Parse("01-Apr-2020"), componentsBySymbols["zrx"], 0.1521m, 0.0495m),
                new CompositionData(indiceBySymbols["l1mc10erc"], DateTime.Parse("01-Apr-2020"), componentsBySymbols["kcs"], 0.9446m, 0.0442m),
                new CompositionData(indiceBySymbols["l1mc10erc"], DateTime.Parse("01-Apr-2020"), componentsBySymbols["bat"], 0.1411m, 0.0753m),
                
                new CompositionData(indiceBySymbols["l1btceth"], DateTime.Parse("01-Apr-2020"), componentsBySymbols["wbtc"], 6434.42m, 0.5m),
                new CompositionData(indiceBySymbols["l1btceth"], DateTime.Parse("01-Apr-2020"), componentsBySymbols["weth"], 132.88m, 0.5m),

                new CompositionData(indiceBySymbols["l1vol15btc"], DateTime.Parse("01-Apr-2020"), componentsBySymbols["wbtc"], 6434.42m, 0.11m),
                new CompositionData(indiceBySymbols["l1vol15btc"], DateTime.Parse("01-Apr-2020"), componentsBySymbols["usdc"], 1m, 0.89m),

                new CompositionData(indiceBySymbols["l1vol20be"], DateTime.Parse("01-Apr-2020"), componentsBySymbols["wbtc"], 6434.42m, 0.065m),
                new CompositionData(indiceBySymbols["l1vol20be"], DateTime.Parse("01-Apr-2020"), componentsBySymbols["weth"], 132.88m, 0.065m),
                new CompositionData(indiceBySymbols["l1vol20be"], DateTime.Parse("01-Apr-2020"), componentsBySymbols["usdc"], 1m, 0.87m),

                #endregion

                #region May 2020
                new CompositionData(indiceBySymbols["l1cex"], DateTime.Parse("01-May-2020"), componentsBySymbols["ht"], 4.334m, 0.3m),
                new CompositionData(indiceBySymbols["l1cex"], DateTime.Parse("01-May-2020"), componentsBySymbols["leo"], 1.077m, 0.3m),
                new CompositionData(indiceBySymbols["l1cex"], DateTime.Parse("01-May-2020"), componentsBySymbols["okb"], 5.589m, 0.173837143707778m),
                new CompositionData(indiceBySymbols["l1cex"], DateTime.Parse("01-May-2020"), componentsBySymbols["ftt"], 3.148m, 0.159019388021262m),
                new CompositionData(indiceBySymbols["l1cex"], DateTime.Parse("01-May-2020"), componentsBySymbols["kcs"], 0.9871m, 0.0671434682709591m),

                new CompositionData(indiceBySymbols["l1dex"], DateTime.Parse("01-May-2020"), componentsBySymbols["zrx"], 0.2125m, 0.461325486990298m),
                new CompositionData(indiceBySymbols["l1dex"], DateTime.Parse("01-May-2020"), componentsBySymbols["knc"], 0.7005m, 0.419673117709983m),
                new CompositionData(indiceBySymbols["l1dex"], DateTime.Parse("01-May-2020"), componentsBySymbols["lrc"], 0.03402m, 0.119001395299719m),

                new CompositionData(indiceBySymbols["l1len"], DateTime.Parse("01-May-2020"), componentsBySymbols["mkr"], 344.22m, 0.5m),
                new CompositionData(indiceBySymbols["l1len"], DateTime.Parse("01-May-2020"), componentsBySymbols["nexo"], 0.1123m, 0.257004796960025m),
                new CompositionData(indiceBySymbols["l1len"], DateTime.Parse("01-May-2020"), componentsBySymbols["lend"], 0.04336m, 0.242995203039975m),

                new CompositionData(indiceBySymbols["l1mc10erc"], DateTime.Parse("01-May-2020"), componentsBySymbols["ht"], 4.334m, 0.15m),
                new CompositionData(indiceBySymbols["l1mc10erc"], DateTime.Parse("01-May-2020"), componentsBySymbols["leo"], 1.077m, 0.15m),
                new CompositionData(indiceBySymbols["l1mc10erc"], DateTime.Parse("01-May-2020"), componentsBySymbols["link"], 3.807m, 0.15m),
                new CompositionData(indiceBySymbols["l1mc10erc"], DateTime.Parse("01-May-2020"), componentsBySymbols["mkr"], 344.22m, 0.104006332168963m),
                new CompositionData(indiceBySymbols["l1mc10erc"], DateTime.Parse("01-May-2020"), componentsBySymbols["okb"], 5.589m, 0.101663897855392m),
                new CompositionData(indiceBySymbols["l1mc10erc"], DateTime.Parse("01-May-2020"), componentsBySymbols["ftt"], 3.148m, 0.0939783080477074m),
                new CompositionData(indiceBySymbols["l1mc10erc"], DateTime.Parse("01-May-2020"), componentsBySymbols["bat"], 0.1892m, 0.0887419219421933m),
                new CompositionData(indiceBySymbols["l1mc10erc"], DateTime.Parse("01-May-2020"), componentsBySymbols["zrx"], 0.2125m, 0.058999997823911m),
                new CompositionData(indiceBySymbols["l1mc10erc"], DateTime.Parse("01-May-2020"), componentsBySymbols["knc"], 0.7005m, 0.0562849171241085m),
                new CompositionData(indiceBySymbols["l1mc10erc"], DateTime.Parse("01-May-2020"), componentsBySymbols["kcs"], 0.9871m, 0.0463246250377244m),

                new CompositionData(indiceBySymbols["l1btceth"], DateTime.Parse("01-May-2020"), componentsBySymbols["wbtc"], 8940.69m, 0.5m),
                new CompositionData(indiceBySymbols["l1btceth"], DateTime.Parse("01-May-2020"), componentsBySymbols["weth"], 214.79m, 0.5m),

                new CompositionData(indiceBySymbols["l1vol15btc"], DateTime.Parse("01-May-2020"), componentsBySymbols["wbtc"], 8940.69m, 0.125m),
                new CompositionData(indiceBySymbols["l1vol15btc"], DateTime.Parse("01-May-2020"), componentsBySymbols["usdc"], 1m, 0.875m),

                new CompositionData(indiceBySymbols["l1vol20be"], DateTime.Parse("01-May-2020"), componentsBySymbols["wbtc"], 8940.69m, 0.075m),
                new CompositionData(indiceBySymbols["l1vol20be"], DateTime.Parse("01-May-2020"), componentsBySymbols["weth"], 214.79m, 0.075m),
                new CompositionData(indiceBySymbols["l1vol20be"], DateTime.Parse("01-May-2020"), componentsBySymbols["usdc"], 1m, 0.85m),
                #endregion

                #region Jun 2020
                new CompositionData(indiceBySymbols["l1cex"], DateTime.Parse("29-May-2020"), componentsBySymbols["ht"], 4.023m, 0.3m),
                new CompositionData(indiceBySymbols["l1cex"], DateTime.Parse("29-May-2020"), componentsBySymbols["leo"], 1.171m, 0.3m),
                new CompositionData(indiceBySymbols["l1cex"], DateTime.Parse("29-May-2020"), componentsBySymbols["okb"], 5.23m, 0.171944435337883m),
                new CompositionData(indiceBySymbols["l1cex"], DateTime.Parse("29-May-2020"), componentsBySymbols["ftt"], 2.943m, 0.15652502472127m),
                new CompositionData(indiceBySymbols["l1cex"], DateTime.Parse("29-May-2020"), componentsBySymbols["kcs"], 0.9823m, 0.0715305399408469m),

                new CompositionData(indiceBySymbols["l1dex"], DateTime.Parse("29-May-2020"), componentsBySymbols["zrx"], 0.3367m, 0.5m),
                new CompositionData(indiceBySymbols["l1dex"], DateTime.Parse("29-May-2020"), componentsBySymbols["knc"], 0.6863m, 0.347254704239048m),
                new CompositionData(indiceBySymbols["l1dex"], DateTime.Parse("29-May-2020"), componentsBySymbols["lrc"], 0.04495m, 0.152745295760952m),

                new CompositionData(indiceBySymbols["l1len"], DateTime.Parse("29-May-2020"), componentsBySymbols["mkr"], 427.96m, 0.5m),
                new CompositionData(indiceBySymbols["l1len"], DateTime.Parse("29-May-2020"), componentsBySymbols["nexo"], 0.1313m, 0.253994231722931m),
                new CompositionData(indiceBySymbols["l1len"], DateTime.Parse("29-May-2020"), componentsBySymbols["lend"], 0.05304m, 0.246005768277069m),

                new CompositionData(indiceBySymbols["l1mc10erc"], DateTime.Parse("29-May-2020"), componentsBySymbols["ht"], 4.023m, 0.15m),
                new CompositionData(indiceBySymbols["l1mc10erc"], DateTime.Parse("29-May-2020"), componentsBySymbols["leo"], 1.171m, 0.15m),
                new CompositionData(indiceBySymbols["l1mc10erc"], DateTime.Parse("29-May-2020"), componentsBySymbols["link"], 4.04m, 0.15m),
                new CompositionData(indiceBySymbols["l1mc10erc"], DateTime.Parse("29-May-2020"), componentsBySymbols["mkr"], 427.96m, 0.115787438260279m),
                new CompositionData(indiceBySymbols["l1mc10erc"], DateTime.Parse("29-May-2020"), componentsBySymbols["okb"], 5.23m, 0.0916454316166629m),
                new CompositionData(indiceBySymbols["l1mc10erc"], DateTime.Parse("29-May-2020"), componentsBySymbols["ftt"], 2.943m, 0.0842025532094753m),
                new CompositionData(indiceBySymbols["l1mc10erc"], DateTime.Parse("29-May-2020"), componentsBySymbols["bat"], 0.2113m, 0.0906347528376021m),
                new CompositionData(indiceBySymbols["l1mc10erc"], DateTime.Parse("29-May-2020"), componentsBySymbols["zrx"], 0.3367m, 0.0723356933063399m),
                new CompositionData(indiceBySymbols["l1mc10erc"], DateTime.Parse("29-May-2020"), componentsBySymbols["knc"], 0.6863m, 0.0522180234621038m),
                new CompositionData(indiceBySymbols["l1mc10erc"], DateTime.Parse("29-May-2020"), componentsBySymbols["kcs"], 0.9823m, 0.0431761073075374m),

                new CompositionData(indiceBySymbols["l1vol20be"], DateTime.Parse("29-May-2020"), componentsBySymbols["usdc"], 1m, 0.7409m),
                new CompositionData(indiceBySymbols["l1vol20be"], DateTime.Parse("29-May-2020"), componentsBySymbols["wbtc"], 9472.24m, 0.12955m),
                new CompositionData(indiceBySymbols["l1vol20be"], DateTime.Parse("29-May-2020"), componentsBySymbols["weth"], 221.25m, 0.12955m),

                new CompositionData(indiceBySymbols["l1vol15btc"], DateTime.Parse("29-May-2020"), componentsBySymbols["usdc"], 1m, 0.7892m),
                new CompositionData(indiceBySymbols["l1vol15btc"], DateTime.Parse("29-May-2020"), componentsBySymbols["wbtc"], 9472.24m, 0.2108m),

                new CompositionData(indiceBySymbols["l1btceth"], DateTime.Parse("29-May-2020"), componentsBySymbols["weth"], 221.25m, 0.5m),
                new CompositionData(indiceBySymbols["l1btceth"], DateTime.Parse("29-May-2020"), componentsBySymbols["wbtc"], 9472.24m, 0.5m),
                #endregion
            };



            foreach (var indiceDefinition in allCompositionData.Select(c => c.IndiceDefinition).Distinct())
            {
                uint version = 0;
                foreach (var date in allCompositionData.Where(c => c.IndiceDefinition == indiceDefinition)
                    .Select(c => c.AsOf).Distinct().OrderBy(d => d))
                {
                    version++;
                    var compositionData = allCompositionData
                        .Where(c => c.AsOf == date && c.IndiceDefinition == indiceDefinition).ToList();
                    await AddCompositionAndInitialValuations(dbContext, mapper, date, version, indiceDefinition,
                        compositionData, cancellationToken).ConfigureAwait(false);
                }
            }


            await dbContext.SaveChangesAsync(cancellationToken);
        }

        internal static async Task AddCompositionAndInitialValuations(IndiceRepositoryContext dbContext, IMapper mapper,
            DateTime historicalAsOfDate, uint version, IndiceDefinitionDao indiceDefinitionDao, List<CompositionData> compositionData,
            CancellationToken cancellationToken)
        {
            var priceAndTargetWeights = compositionData.ToDictionary(c => (IComponentDefinition)c.ComponentDefinition, c => c.PriceAndWeight);
            var compositionSymbol = indiceDefinitionDao.GetCompositionSymbol(historicalAsOfDate);
            var targetIndicePrice = GetTargetIssuePrice(compositionSymbol);
            var compositions = IndiceCompositionCalculator.CalculateIndiceComposition(indiceDefinitionDao,
                priceAndTargetWeights, targetIndicePrice, version, historicalAsOfDate);

            var compositionDao = mapper.Map<IndiceCompositionDao>(compositions);
            compositionDao.ComponentQuantityDaos.ForEach(q => q.LinkToIndiceComposition(compositionDao));
            await dbContext.IndiceCompositions.AddAsync(compositionDao, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            var componentValuations = compositionData.Select(p => new ComponentValuationDao(
                compositionDao.ComponentQuantityDaos.Single(q => q.ComponentDefinition.Symbol == p.ComponentDefinition.Symbol),
                historicalAsOfDate, Usdc, p.PriceAndWeight.Price, "cryptoCompare")).ToList();

            await dbContext.ComponentValuations.AddRangeAsync(componentValuations, cancellationToken);

            var indiceValuation = new IndiceValuationDao(componentValuations);
            await dbContext.IndiceValuations.AddAsync(indiceValuation, cancellationToken);
        }

        internal static decimal GetTargetIssuePrice(string compositionSymbol)
        {
            return TargetIssuePriceByCompositionSymbol.TryGetValue(compositionSymbol, out var issuePrice) ? issuePrice : 100m;
        }

        private static readonly Dictionary<string, decimal> TargetIssuePriceByCompositionSymbol =
            new Dictionary<string, decimal>
            {
                {"l1amg2003", 109.01747953582852998702237578m},
                {"l1cex2003", 166.89063577270502672090053720m},
                {"l1dex2003", 153.70073834097101260102037937m},
                {"l1len2003", 139.07719885058049760491362928m},
                {"l1sca2003", 119.54041799142740614271470609m},

                {"l1cex2006", 98.78196024409610152057474844m},
                {"l1dex2006", 130.60197236946566472017116372m},
                {"l1btceth2006", 103.85607935995173194247m},
                {"l1len2006", 119.17740439259966310741733793m},
                {"l1mc10erc2006", 104.75395058863399644703986350m},
                {"l1vol15btc", 100.64102994287913139848m},
                {"l1vol20be", 100.57841190399275939853m},
            };
    }
}