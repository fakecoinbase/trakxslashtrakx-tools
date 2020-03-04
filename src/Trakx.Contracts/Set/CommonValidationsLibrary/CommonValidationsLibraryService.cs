using System.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.ContractHandlers;
using System.Threading;
using Nethereum.Web3;
using Trakx.Contracts.Set.CommonValidationsLibrary.ContractDefinition;

namespace Trakx.Contracts.Set.CommonValidationsLibrary
{
    public partial class CommonValidationsLibraryService
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(IWeb3 web3, CommonValidationsLibraryDeployment commonValidationsLibraryDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<CommonValidationsLibraryDeployment>().SendRequestAndWaitForReceiptAsync(commonValidationsLibraryDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(IWeb3 web3, CommonValidationsLibraryDeployment commonValidationsLibraryDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<CommonValidationsLibraryDeployment>().SendRequestAsync(commonValidationsLibraryDeployment);
        }

        public static async Task<CommonValidationsLibraryService> DeployContractAndGetServiceAsync(IWeb3 web3, CommonValidationsLibraryDeployment commonValidationsLibraryDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, commonValidationsLibraryDeployment, cancellationTokenSource);
            return new CommonValidationsLibraryService(web3, receipt.ContractAddress);
        }

        protected IWeb3 Web3{ get; }

        public ContractHandler ContractHandler { get; }

        public CommonValidationsLibraryService(IWeb3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }




    }
}
