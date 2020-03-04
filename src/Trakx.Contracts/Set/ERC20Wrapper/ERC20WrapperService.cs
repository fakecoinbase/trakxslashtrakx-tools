using System.Threading.Tasks;
using System.Numerics;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.ContractHandlers;
using System.Threading;
using Nethereum.Web3;
using Trakx.Contracts.Set.ERC20Wrapper.ContractDefinition;

namespace Trakx.Contracts.Set.ERC20Wrapper
{
    public partial class ERC20WrapperService
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(IWeb3 web3, ERC20WrapperDeployment eRC20WrapperDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<ERC20WrapperDeployment>().SendRequestAndWaitForReceiptAsync(eRC20WrapperDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(IWeb3 web3, ERC20WrapperDeployment eRC20WrapperDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<ERC20WrapperDeployment>().SendRequestAsync(eRC20WrapperDeployment);
        }

        public static async Task<ERC20WrapperService> DeployContractAndGetServiceAsync(IWeb3 web3, ERC20WrapperDeployment eRC20WrapperDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, eRC20WrapperDeployment, cancellationTokenSource);
            return new ERC20WrapperService(web3, receipt.ContractAddress);
        }

        protected IWeb3 Web3{ get; }

        public ContractHandler ContractHandler { get; }

        public ERC20WrapperService(IWeb3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }

        public Task<BigInteger> BalanceOfQueryAsync(BalanceOfFunction balanceOfFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<BalanceOfFunction, BigInteger>(balanceOfFunction, blockParameter);
        }

        
        public Task<BigInteger> BalanceOfQueryAsync(string token, string owner, BlockParameter blockParameter = null)
        {
            var balanceOfFunction = new BalanceOfFunction();
                balanceOfFunction.Token = token;
                balanceOfFunction.Owner = owner;
            
            return ContractHandler.QueryAsync<BalanceOfFunction, BigInteger>(balanceOfFunction, blockParameter);
        }
    }
}
