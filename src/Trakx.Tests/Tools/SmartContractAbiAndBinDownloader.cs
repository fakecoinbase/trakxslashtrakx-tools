using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using Trakx.Contracts.Set;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Tests.Tools
{
    public class SmartContractAbiAndBinDownloader
    {
        private ITestOutputHelper _output;
        private IServiceProvider _serviceProvider;
        private HttpClient _httpClient;

        public SmartContractAbiAndBinDownloader(ITestOutputHelper output)
        {
            _output = output;
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddHttpClient();

            _serviceProvider = serviceCollection.BuildServiceProvider();

            _httpClient = _serviceProvider.GetService<IHttpClientFactory>().CreateClient();
            _httpClient.BaseAddress = new Uri(@"https://etherscan.io/address/");
        }

        [Fact(Skip = "not a test")]
        public async Task GenerateSetBinAndAbiFiles()
        {
            var projectDirectory = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.Parent.Parent.Parent;
            var targetPath = Path.Combine(projectDirectory.FullName, "Tools", "AbiBin");
            var targetDirectory = new DirectoryInfo(targetPath);
            if (targetDirectory.Exists) targetDirectory.Delete();
            targetDirectory.Create();

            foreach (var addressByName in DeployedContractAddresses.AddressByName)
            {
                var address = addressByName.Value;
                var contractName = addressByName.Key;

                await CreateAbiAndBinFilesFromEtherscanResponse(address, targetPath, contractName);
            }
        }

        /// <summary>
        /// Dirty web scrapper used to retrieve the contract abi and bins, then create the corresponding files.
        /// It will put them in a temp folder that you can copy in a Trakx.Contracts.* project and simply rebuild
        /// to regenerate the C# classes for the smart contracts.
        /// </summary>
        private async Task CreateAbiAndBinFilesFromEtherscanResponse(string address, string targetPath, string contractName)
        {
            var response = await _httpClient.GetAsync(address).ConfigureAwait(false);
            var contentStream = await response.Content.ReadAsStreamAsync();

            var doc = new HtmlDocument();
            doc.Load(contentStream);

            var abi = doc.DocumentNode.SelectSingleNode("//pre[@id='js-copytextarea2']")?.InnerText;
            var bin = doc.DocumentNode.SelectSingleNode("//div[@id='verifiedbytecode2']")?.InnerText;
            if (abi == null || bin == null) return;

            await File.WriteAllTextAsync(Path.Combine(targetPath, $"{contractName}.abi"), abi ?? "");
            await File.WriteAllTextAsync(Path.Combine(targetPath, $"{contractName}.bin"), bin ?? "");
        }
    }
}
