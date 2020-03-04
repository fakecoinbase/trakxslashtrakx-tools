using System.Threading.Tasks;
using System.Numerics;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.ContractHandlers;
using System.Threading;
using Nethereum.Web3;
using Trakx.Contracts.Set.RebalancingSetIssuanceModule.ContractDefinition;

namespace Trakx.Contracts.Set.RebalancingSetIssuanceModule
{
    public partial class RebalancingSetIssuanceModuleService
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(IWeb3 web3, RebalancingSetIssuanceModuleDeployment rebalancingSetIssuanceModuleDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<RebalancingSetIssuanceModuleDeployment>().SendRequestAndWaitForReceiptAsync(rebalancingSetIssuanceModuleDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(IWeb3 web3, RebalancingSetIssuanceModuleDeployment rebalancingSetIssuanceModuleDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<RebalancingSetIssuanceModuleDeployment>().SendRequestAsync(rebalancingSetIssuanceModuleDeployment);
        }

        public static async Task<RebalancingSetIssuanceModuleService> DeployContractAndGetServiceAsync(IWeb3 web3, RebalancingSetIssuanceModuleDeployment rebalancingSetIssuanceModuleDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, rebalancingSetIssuanceModuleDeployment, cancellationTokenSource);
            return new RebalancingSetIssuanceModuleService(web3, receipt.ContractAddress);
        }

        protected IWeb3 Web3{ get; }

        public ContractHandler ContractHandler { get; }

        public RebalancingSetIssuanceModuleService(IWeb3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }

        public Task<string> IssueRebalancingSetRequestAsync(IssueRebalancingSetFunction issueRebalancingSetFunction)
        {
             return ContractHandler.SendRequestAsync(issueRebalancingSetFunction);
        }

        public Task<TransactionReceipt> IssueRebalancingSetRequestAndWaitForReceiptAsync(IssueRebalancingSetFunction issueRebalancingSetFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(issueRebalancingSetFunction, cancellationToken);
        }

        public Task<string> IssueRebalancingSetRequestAsync(string rebalancingSetAddress, BigInteger rebalancingSetQuantity, bool keepChangeInVault)
        {
            var issueRebalancingSetFunction = new IssueRebalancingSetFunction();
                issueRebalancingSetFunction.RebalancingSetAddress = rebalancingSetAddress;
                issueRebalancingSetFunction.RebalancingSetQuantity = rebalancingSetQuantity;
                issueRebalancingSetFunction.KeepChangeInVault = keepChangeInVault;
            
             return ContractHandler.SendRequestAsync(issueRebalancingSetFunction);
        }

        public Task<TransactionReceipt> IssueRebalancingSetRequestAndWaitForReceiptAsync(string rebalancingSetAddress, BigInteger rebalancingSetQuantity, bool keepChangeInVault, CancellationTokenSource cancellationToken = null)
        {
            var issueRebalancingSetFunction = new IssueRebalancingSetFunction();
                issueRebalancingSetFunction.RebalancingSetAddress = rebalancingSetAddress;
                issueRebalancingSetFunction.RebalancingSetQuantity = rebalancingSetQuantity;
                issueRebalancingSetFunction.KeepChangeInVault = keepChangeInVault;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(issueRebalancingSetFunction, cancellationToken);
        }

        public Task<string> IssueRebalancingSetWrappingEtherRequestAsync(IssueRebalancingSetWrappingEtherFunction issueRebalancingSetWrappingEtherFunction)
        {
             return ContractHandler.SendRequestAsync(issueRebalancingSetWrappingEtherFunction);
        }

        public Task<TransactionReceipt> IssueRebalancingSetWrappingEtherRequestAndWaitForReceiptAsync(IssueRebalancingSetWrappingEtherFunction issueRebalancingSetWrappingEtherFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(issueRebalancingSetWrappingEtherFunction, cancellationToken);
        }

        public Task<string> IssueRebalancingSetWrappingEtherRequestAsync(string rebalancingSetAddress, BigInteger rebalancingSetQuantity, bool keepChangeInVault)
        {
            var issueRebalancingSetWrappingEtherFunction = new IssueRebalancingSetWrappingEtherFunction();
                issueRebalancingSetWrappingEtherFunction.RebalancingSetAddress = rebalancingSetAddress;
                issueRebalancingSetWrappingEtherFunction.RebalancingSetQuantity = rebalancingSetQuantity;
                issueRebalancingSetWrappingEtherFunction.KeepChangeInVault = keepChangeInVault;
            
             return ContractHandler.SendRequestAsync(issueRebalancingSetWrappingEtherFunction);
        }

        public Task<TransactionReceipt> IssueRebalancingSetWrappingEtherRequestAndWaitForReceiptAsync(string rebalancingSetAddress, BigInteger rebalancingSetQuantity, bool keepChangeInVault, CancellationTokenSource cancellationToken = null)
        {
            var issueRebalancingSetWrappingEtherFunction = new IssueRebalancingSetWrappingEtherFunction();
                issueRebalancingSetWrappingEtherFunction.RebalancingSetAddress = rebalancingSetAddress;
                issueRebalancingSetWrappingEtherFunction.RebalancingSetQuantity = rebalancingSetQuantity;
                issueRebalancingSetWrappingEtherFunction.KeepChangeInVault = keepChangeInVault;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(issueRebalancingSetWrappingEtherFunction, cancellationToken);
        }

        public Task<string> WethQueryAsync(WethFunction wethFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<WethFunction, string>(wethFunction, blockParameter);
        }

        
        public Task<string> WethQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<WethFunction, string>(null, blockParameter);
        }

        public Task<string> VaultInstanceQueryAsync(VaultInstanceFunction vaultInstanceFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<VaultInstanceFunction, string>(vaultInstanceFunction, blockParameter);
        }

        
        public Task<string> VaultInstanceQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<VaultInstanceFunction, string>(null, blockParameter);
        }

        public Task<string> TransferProxyQueryAsync(TransferProxyFunction transferProxyFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<TransferProxyFunction, string>(transferProxyFunction, blockParameter);
        }

        
        public Task<string> TransferProxyQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<TransferProxyFunction, string>(null, blockParameter);
        }

        public Task<string> CoreInstanceQueryAsync(CoreInstanceFunction coreInstanceFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<CoreInstanceFunction, string>(coreInstanceFunction, blockParameter);
        }

        
        public Task<string> CoreInstanceQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<CoreInstanceFunction, string>(null, blockParameter);
        }

        public Task<string> RedeemRebalancingSetRequestAsync(RedeemRebalancingSetFunction redeemRebalancingSetFunction)
        {
             return ContractHandler.SendRequestAsync(redeemRebalancingSetFunction);
        }

        public Task<TransactionReceipt> RedeemRebalancingSetRequestAndWaitForReceiptAsync(RedeemRebalancingSetFunction redeemRebalancingSetFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(redeemRebalancingSetFunction, cancellationToken);
        }

        public Task<string> RedeemRebalancingSetRequestAsync(string rebalancingSetAddress, BigInteger rebalancingSetQuantity, bool keepChangeInVault)
        {
            var redeemRebalancingSetFunction = new RedeemRebalancingSetFunction();
                redeemRebalancingSetFunction.RebalancingSetAddress = rebalancingSetAddress;
                redeemRebalancingSetFunction.RebalancingSetQuantity = rebalancingSetQuantity;
                redeemRebalancingSetFunction.KeepChangeInVault = keepChangeInVault;
            
             return ContractHandler.SendRequestAsync(redeemRebalancingSetFunction);
        }

        public Task<TransactionReceipt> RedeemRebalancingSetRequestAndWaitForReceiptAsync(string rebalancingSetAddress, BigInteger rebalancingSetQuantity, bool keepChangeInVault, CancellationTokenSource cancellationToken = null)
        {
            var redeemRebalancingSetFunction = new RedeemRebalancingSetFunction();
                redeemRebalancingSetFunction.RebalancingSetAddress = rebalancingSetAddress;
                redeemRebalancingSetFunction.RebalancingSetQuantity = rebalancingSetQuantity;
                redeemRebalancingSetFunction.KeepChangeInVault = keepChangeInVault;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(redeemRebalancingSetFunction, cancellationToken);
        }

        public Task<string> CoreQueryAsync(CoreFunction coreFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<CoreFunction, string>(coreFunction, blockParameter);
        }

        
        public Task<string> CoreQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<CoreFunction, string>(null, blockParameter);
        }

        public Task<string> RedeemRebalancingSetUnwrappingEtherRequestAsync(RedeemRebalancingSetUnwrappingEtherFunction redeemRebalancingSetUnwrappingEtherFunction)
        {
             return ContractHandler.SendRequestAsync(redeemRebalancingSetUnwrappingEtherFunction);
        }

        public Task<TransactionReceipt> RedeemRebalancingSetUnwrappingEtherRequestAndWaitForReceiptAsync(RedeemRebalancingSetUnwrappingEtherFunction redeemRebalancingSetUnwrappingEtherFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(redeemRebalancingSetUnwrappingEtherFunction, cancellationToken);
        }

        public Task<string> RedeemRebalancingSetUnwrappingEtherRequestAsync(string rebalancingSetAddress, BigInteger rebalancingSetQuantity, bool keepChangeInVault)
        {
            var redeemRebalancingSetUnwrappingEtherFunction = new RedeemRebalancingSetUnwrappingEtherFunction();
                redeemRebalancingSetUnwrappingEtherFunction.RebalancingSetAddress = rebalancingSetAddress;
                redeemRebalancingSetUnwrappingEtherFunction.RebalancingSetQuantity = rebalancingSetQuantity;
                redeemRebalancingSetUnwrappingEtherFunction.KeepChangeInVault = keepChangeInVault;
            
             return ContractHandler.SendRequestAsync(redeemRebalancingSetUnwrappingEtherFunction);
        }

        public Task<TransactionReceipt> RedeemRebalancingSetUnwrappingEtherRequestAndWaitForReceiptAsync(string rebalancingSetAddress, BigInteger rebalancingSetQuantity, bool keepChangeInVault, CancellationTokenSource cancellationToken = null)
        {
            var redeemRebalancingSetUnwrappingEtherFunction = new RedeemRebalancingSetUnwrappingEtherFunction();
                redeemRebalancingSetUnwrappingEtherFunction.RebalancingSetAddress = rebalancingSetAddress;
                redeemRebalancingSetUnwrappingEtherFunction.RebalancingSetQuantity = rebalancingSetQuantity;
                redeemRebalancingSetUnwrappingEtherFunction.KeepChangeInVault = keepChangeInVault;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(redeemRebalancingSetUnwrappingEtherFunction, cancellationToken);
        }

        public Task<string> VaultQueryAsync(VaultFunction vaultFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<VaultFunction, string>(vaultFunction, blockParameter);
        }

        
        public Task<string> VaultQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<VaultFunction, string>(null, blockParameter);
        }
    }
}
