using System.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.ContractHandlers;
using System.Threading;
using Trakx.Contracts.Set.CommonValidationsLibrary.ContractDefinition;

namespace Trakx.Contracts.Set.CommonValidationsLibrary
{
    public partial class CommonValidationsLibraryService
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.Web3 web3, CommonValidationsLibraryDeployment commonValidationsLibraryDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<CommonValidationsLibraryDeployment>().SendRequestAndWaitForReceiptAsync(commonValidationsLibraryDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.Web3 web3, CommonValidationsLibraryDeployment commonValidationsLibraryDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<CommonValidationsLibraryDeployment>().SendRequestAsync(commonValidationsLibraryDeployment);
        }

        public static async Task<CommonValidationsLibraryService> DeployContractAndGetServiceAsync(Nethereum.Web3.Web3 web3, CommonValidationsLibraryDeployment commonValidationsLibraryDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, commonValidationsLibraryDeployment, cancellationTokenSource);
            return new CommonValidationsLibraryService(web3, receipt.ContractAddress);
        }

        protected Nethereum.Web3.Web3 Web3{ get; }

        public ContractHandler ContractHandler { get; }

        public CommonValidationsLibraryService(Nethereum.Web3.Web3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }




    }
}
