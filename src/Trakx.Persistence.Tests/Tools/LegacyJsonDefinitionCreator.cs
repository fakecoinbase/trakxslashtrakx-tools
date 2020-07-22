using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Trakx.Common.Interfaces;
using Trakx.Common.Interfaces.Indice;
using Trakx.Persistence.Tests.Model;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Persistence.Tests.Tools
{
    [Collection(nameof(SeededDbContextCollection))]
    public class LegacyJsonDefinitionCreator : IClassFixture<SeededDbContextFixture>
    {
        private readonly IndiceDataProvider _indiceDetailProvider;

        public LegacyJsonDefinitionCreator(SeededDbContextFixture fixture, ITestOutputHelper output)
        {
            var context = fixture.Context;
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddMemoryCache();
            var logger = output.ToLogger<IndiceDataProvider>();
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            _indiceDetailProvider = new IndiceDataProvider(context, serviceProvider.GetService<IMemoryCache>(), new DateTimeProvider(), logger);
        }

        [Theory(Skip = "not a test")]
        [InlineData("l1cex2005")]
        [InlineData("l1dex2005")]
        [InlineData("l1len2005")]
        [InlineData("l1mc10erc2005")]
        [InlineData("l1btceth2005")]
        [InlineData("l1vol15btc2005")]
        [InlineData("l1vol20be2005")]
        public async Task GenerateLegacyJsonFile(string compositionSymbol)
        {
            var composition = await _indiceDetailProvider.GetCompositionFromSymbol(compositionSymbol)
                .ConfigureAwait(false);
            var initialValuation = await _indiceDetailProvider.GetInitialValuation(composition)
                .ConfigureAwait(false);

            var jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };
            var targetPath = CreateResultDirectory();

            var definitionFilePath = Path.Combine(targetPath, $"{compositionSymbol}.definition.json");
            var definitionModel = new LegacyDefinitionModel(composition, initialValuation);
            File.WriteAllText(definitionFilePath, JsonSerializer.Serialize(definitionModel, jsonSerializerOptions));

            var detailsFilePath = Path.Combine(targetPath, $"{compositionSymbol}.details.json");
            var detailsModel = new LegacyDetailsModel(composition, initialValuation);
            File.WriteAllText(detailsFilePath, JsonSerializer.Serialize(detailsModel, jsonSerializerOptions));
        }

        private string CreateResultDirectory()
        {
            var projectDirectory = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory?.Parent?.Parent?.Parent;
            var targetPath = Path.Combine(projectDirectory?.FullName, "Tools", "LegacyJson");
            var targetDirectory = new DirectoryInfo(targetPath);
            //if (targetDirectory.Exists) targetDirectory.Delete();
            if (!targetDirectory.Exists) targetDirectory.Create();
            return targetPath;
        }
    }

    public class LegacyDetailsModel
    {
        public LegacyDetailsModel(IIndiceComposition composition, IIndiceValuation initialValuation)
        {
            Name = composition.IndiceDefinition.Name;
            Symbol = composition.Symbol;
            TargetUsdPrice = Math.Round(initialValuation.NetAssetValue, 5);
            UsdBidAsk = new BidAskModel(TargetUsdPrice);
            Components = initialValuation.ComponentValuations.Select(c => new ComponentDetailModel(c)).ToList();
            ComponentAddresses = composition.ComponentQuantities.Select(c => c.ComponentDefinition.Address).ToList();
        }
        
        [JsonPropertyName("name")] public string Name { get; set; }
        [JsonPropertyName("symbol")] public string Symbol { get; set; }
        [JsonPropertyName("targetUsdPrice")] public decimal TargetUsdPrice { get; set; }
        [JsonPropertyName("usdBidAsk")] public BidAskModel UsdBidAsk { get; set; }
        [JsonPropertyName("components")] public List<ComponentDetailModel> Components { get; set; }
        [JsonPropertyName("componentAddresses")] public List<string> ComponentAddresses { get; set; }
    }

    public class LegacyDefinitionModel
    {
        public LegacyDefinitionModel(IIndiceComposition composition, IIndiceValuation initialValuation)
        {
            Name = composition.IndiceDefinition.Name;
            Symbol = composition.Symbol;
            TargetUsdPrice = Math.Round(initialValuation.NetAssetValue, 5);
            ComponentDefinitions = initialValuation.ComponentValuations.Select(q => new ComponentDefinitionModel(q)).ToList();
        }

        [JsonPropertyName("name")] public string Name { get; set; }
        [JsonPropertyName("symbol")] public string Symbol { get; set; }
        [JsonPropertyName("targetUsdPrice")] public decimal TargetUsdPrice { get; set; }
        [JsonPropertyName("componentDefinitions")] public List<ComponentDefinitionModel> ComponentDefinitions { get; set; }
    }

    public class ComponentDefinitionModel
    {
        public ComponentDefinitionModel(IComponentValuation componentValuation)
        {
            Address = componentValuation.ComponentQuantity.ComponentDefinition.Address;
            UsdValueAtCreation = Math.Round(componentValuation.Value, 5);
            UsdWeightAtCreation = (decimal)(componentValuation.Weight ?? 0);
            Decimals = componentValuation.ComponentQuantity.ComponentDefinition.Decimals;
        }

        [JsonPropertyName("address")] public string Address { get; set; }
        [JsonPropertyName("usdWeightAtCreation")] public decimal UsdWeightAtCreation { get; set; }
        [JsonPropertyName("usdValueAtCreation")] public decimal UsdValueAtCreation { get; set; }
        [JsonPropertyName("decimals")] public int Decimals { get; set; }
    }

    public class ComponentDetailModel
    {
        public ComponentDetailModel(IComponentValuation componentValuation)
        {
            Address = componentValuation.ComponentQuantity.ComponentDefinition.Address;
            Name = componentValuation.ComponentQuantity.ComponentDefinition.Name;
            Symbol = componentValuation.ComponentQuantity.ComponentDefinition.Symbol;
            Decimals = componentValuation.ComponentQuantity.ComponentDefinition.Decimals;
            UsdBidAsk = new BidAskModel(componentValuation.Price);
            Proportion = (decimal) (componentValuation.Weight ?? 0);
        }

        [JsonPropertyName("address")] public string Address { get; set; }
        [JsonPropertyName("name")] public string Name { get; set; }
        [JsonPropertyName("symbol")] public string Symbol { get; set; }
        [JsonPropertyName("decimals")] public int Decimals { get; set; }
        [JsonPropertyName("usdBidAsk")] public BidAskModel UsdBidAsk { get; set; }
        [JsonPropertyName("proportion")] public decimal Proportion { get; set; }
    }

    public class BidAskModel
    {
        public BidAskModel(decimal price)
        {
            Ask = Bid = Math.Round(price, 5);
        }
        [JsonPropertyName("ask")] public decimal Ask { get; set; }
        [JsonPropertyName("bid")] public decimal Bid { get; set; }
    }
}