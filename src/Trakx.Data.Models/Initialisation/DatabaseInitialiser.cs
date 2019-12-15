using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Trakx.Data.Models.Index;

namespace Trakx.Data.Models.Initialisation
{
    public class DatabaseInitialiser : IDatabaseInitialiser
    {
        private const string Usd = "USD";
        private readonly IndexRepositoryContext _dbContext;
        private readonly ILogger<DatabaseInitialiser> _logger;

        public DatabaseInitialiser(IndexRepositoryContext dbContext,
            ILogger<DatabaseInitialiser> logger)
        {
            _dbContext = dbContext;
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

            var firstOctober = new DateTime(2019, 10, 1);

            #region Computation Services
            var computationServices = new IndexDefinition(
                    "L1CPU003",
                    "Top 3 Computation Services",
                    "This index is composed of the top 3 erc20 tokens for Computation Services. Created on the 1st of October as a POC.",
                    new List<ComponentDefinition>
                    {
                        new ComponentDefinition(
                            "0xbf2179859fc6D5BEE9Bf9158632Dc51678a4100e",
                            "ELF Token",
                            "ELF",
                            18,
                            42,
                            0.08069m,
                            Usd,
                            firstOctober),
                        new ComponentDefinition(
                            "0xB98d4C97425d9908E66E53A6fDf673ACcA0BE986",
                            "ArcBlock",
                            "ABT",
                            18,
                            17,
                            0.202m,
                            Usd,
                            firstOctober),
                        new ComponentDefinition(
                            "0x8290333ceF9e6D528dD5618Fb97a76f268f3EDD4",
                            "Ankr Network",
                            "ANKR",
                            18,
                            1010,
                            0.080104855165688m,
                            Usd,
                            firstOctober)
                    },
                    "0x7210cc724480c85b893a9febbecc24a8dc4ff1de",
                    firstOctober);

            //_logger.LogDebug(JsonSerializer.Serialize(computationServices));
            #endregion

            #region Financial Services
            var financialServices = new IndexDefinition(
                "L1FIN003",
                "Top 3 Financial Services",
                "This index is composed of the top 3 erc20 tokens for Financial Services. Created on the 1st of October as a POC.",
                new List<ComponentDefinition>
                {
                    new ComponentDefinition(
                        "0xB63B606Ac810a52cCa15e44bB630fd42D8d1d83d",
                        "Monaco",
                        "MCO",
                        8,
                        11,
                        3.299m,
                        Usd,
                        firstOctober),
                    new ComponentDefinition(
                        "0xd26114cd6EE289AccF82350c8d8487fedB8A0C07",
                        "OMGToken",
                        "OMG",
                        18,
                        318066157761,
                        1.048m,
                        Usd,
                        firstOctober),
                    new ComponentDefinition(
                        "0xA15C7Ebe1f07CaF6bFF097D8a589fb8AC49Ae5B3",
                        "Pundi X Token",
                        "NPXS",
                        18,
                        941087897609628,
                        0.0003542m,
                        Usd,
                        firstOctober)
                },
                "0xa308dde45d2520108d16078457dbd489c3947e8a",
                firstOctober);

            //_logger.LogDebug(JsonSerializer.Serialize(financialServices));
            #endregion

            #region Infrastructure Services
            var infrastructureServices = new IndexDefinition(
                "L1INF003",
                "Top 3 Infrastructure Services",
                "This index is composed of the top 3 erc20 tokens for Infrastructure Services. Created on the 1st of October as a POC.",
                new List<ComponentDefinition>
                {
                    new ComponentDefinition(
                        "0x6c6EE5e31d828De241282B9606C8e98Ea48526E2",
                        "HoloToken",
                        "HOT",
                        8,
                        4128989636237,
                        0.0008073m,
                        Usd,
                        firstOctober),
                    new ComponentDefinition(
                        "0x744d70FDBE2Ba4CF95131626614a1763DF805B9E",
                        "Status Network Token",
                        "SNT",
                        18,
                        229568411387,
                        0.01452m,
                        Usd,
                        firstOctober),
                    new ComponentDefinition(
                        "0x08f5a9235B08173b7569F83645d2c7fB55e8cCD8",
                        "Tierion Network Token",
                        "TNT",
                        8,
                        12,
                        0.02852m,
                        Usd,
                        firstOctober)
                },
                "0x5a3996551e34ee9f3c0496af727dd07e8be127f2",
                firstOctober);

            //_logger.LogDebug(JsonSerializer.Serialize(infrastructureServices));
            #endregion

            #region Scalability Services
            var scalabilityServices = new IndexDefinition(
                "L1SCA004",
                "Top 4 Scalability Services",
                "This index is composed of the top 4 erc20 tokens for Scalability Services. Created on the 1st of October as a POC.",
                new List<ComponentDefinition>
                {
                    new ComponentDefinition(
                        "0xd26114cd6EE289AccF82350c8d8487fedB8A0C07",
                        "OMGToken",
                        "OMG",
                        18,
                        3,
                        1.049m,
                        Usd,
                        firstOctober),
                    new ComponentDefinition(
                        "0x7D1AfA7B718fb893dB30A3aBc0Cfc608AaCfeBB0",
                        "Matic Token",
                        "MATIC",
                        18,
                        193,
                        0.01297m,
                        Usd,
                        firstOctober),
                    new ComponentDefinition(
                        "0x4F9254C83EB525f9FCf346490bbb3ed28a81C667",
                        "CelerToken",
                        "CELR",
                        18,
                        467,
                        0.005354m,
                        Usd,
                        firstOctober),
                    new ComponentDefinition(
                        "0x4E15361FD6b4BB609Fa63C81A2be19d873717870",
                        "Fantom Token",
                        "FTM",
                        18,
                        159,
                        0.01575m,
                        Usd,
                        firstOctober)
                },
                "0xb2fc2d89e09e0d903c33f28608aecbe9b402ba59",
                firstOctober);

            //_logger.LogDebug(JsonSerializer.Serialize(scalabilityServices));
            #endregion

            #region Storage Services
            var storageServices = new IndexDefinition(
                "L1STR004",
                "Top 4 Storage Services",
                "This index is composed of the top 4 erc20 tokens for Storage Services. Created on the 1st of October as a POC.",
                new List<ComponentDefinition>
                {
                    new ComponentDefinition(
                        "0xB64ef51C888972c908CFacf59B47C1AfBC0Ab8aC",
                        "StorjToken",
                        "STORJ",
                        8,
                        2,
                        0.1493m,
                        Usd,
                        firstOctober),
                    new ComponentDefinition(
                        "0x8290333ceF9e6D528dD5618Fb97a76f268f3EDD4",
                        "Ankr Network",
                        "ANKR",
                        18,
                        836960160697,
                        0.002987m,
                        Usd,
                        firstOctober),
                    new ComponentDefinition(
                        "0x5732046A883704404F284Ce41FfADd5b007FD668",
                        "Bluzelle Token",
                        "BLZ",
                        18,
                        75688767787,
                        0.03303m,
                        Usd,
                        firstOctober),
                    new ComponentDefinition(
                        "0x607F4C5BB672230e8672085532f7e901544a7375",
                        "iEx.ec Network Token",
                        "RLC",
                        9,
                        12,
                        0.2181m,
                        Usd,
                        firstOctober)
                },
                "0xe05168c3fa30e93d3f1667b35e9456aac9b5519a",
                firstOctober);

            //_logger.LogDebug(JsonSerializer.Serialize(scalabilityServices));
            #endregion

            await _dbContext.IndexDefinitions.AddRangeAsync(new[]
            {
                computationServices,
                financialServices,
                infrastructureServices,
                scalabilityServices,
                storageServices
            }).ConfigureAwait(false);
        }
    }
}