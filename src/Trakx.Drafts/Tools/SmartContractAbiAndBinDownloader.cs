using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Drafts.Tools
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


        /// <summary>
        /// 1) Point this test to the address of the contracts you want to get an Abi and Bin file for.
        /// 2) Run the test and observe that you get an bunch of new folders in /Tools with the downloaded files.
        /// 3) Take the new folders and move them to Trakx.Contracts.
        /// 4) Uncomment the reference to Nethereum.Autogen.ContractApi and rebuild the Trakx.Contracts project.
        /// 5) You should now have the new CSharp classes to interact with the contracts.
        /// 6) Cleanup the Abi and Bin files, comment the reference to Nethereum.Autogen.ContractApi.
        /// 7) It is a good idea to edit the generated file to make use IWeb3 instead of Web3.
        /// </summary>
        /// <returns></returns>
        [Fact(Skip = "not a test")]
        //[Fact]
        public async Task GenerateSetBinAndAbiFiles()
        {
            var projectDirectory = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.Parent.Parent.Parent;
            var targetPath = Path.Combine(projectDirectory.FullName, "Tools", "AbiBin");
            var targetDirectory = new DirectoryInfo(targetPath);
            if (targetDirectory.Exists) targetDirectory.Delete();
            targetDirectory.Create();

            var contracts = new Dictionary<string, string>
            {
                { "RebalancingSetTokenV3", "0x26c477C0e431C2AB3d82F59d1D350323AF15b795" },
                { "RebalancingSetTokenV3Factory", "0xA367A2513Cbd5be1c75a745914521a93E011549c"},
            };
            foreach (var addressByName in contracts)
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
