using System.Threading.Tasks;
using System.Numerics;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.ContractHandlers;
using System.Threading;
using Trakx.Contracts.Set.RebalancingSetExchangeIssuanceModule.ContractDefinition;

namespace Trakx.Contracts.Set.RebalancingSetExchangeIssuanceModule
{
    public partial class RebalancingSetExchangeIssuanceModuleService
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.Web3 web3, RebalancingSetExchangeIssuanceModuleDeployment rebalancingSetExchangeIssuanceModuleDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<RebalancingSetExchangeIssuanceModuleDeployment>().SendRequestAndWaitForReceiptAsync(rebalancingSetExchangeIssuanceModuleDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.Web3 web3, RebalancingSetExchangeIssuanceModuleDeployment rebalancingSetExchangeIssuanceModuleDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<RebalancingSetExchangeIssuanceModuleDeployment>().SendRequestAsync(rebalancingSetExchangeIssuanceModuleDeployment);
        }

        public static async Task<RebalancingSetExchangeIssuanceModuleService> DeployContractAndGetServiceAsync(Nethereum.Web3.Web3 web3, RebalancingSetExchangeIssuanceModuleDeployment rebalancingSetExchangeIssuanceModuleDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, rebalancingSetExchangeIssuanceModuleDeployment, cancellationTokenSource);
            return new RebalancingSetExchangeIssuanceModuleService(web3, receipt.ContractAddress);
        }

        protected Nethereum.Web3.Web3 Web3{ get; }

        public ContractHandler ContractHandler { get; }

        public RebalancingSetExchangeIssuanceModuleService(Nethereum.Web3.Web3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }

        public Task<string> IssueRebalancingSetWithEtherRequestAsync(IssueRebalancingSetWithEtherFunction issueRebalancingSetWithEtherFunction)
        {
             return ContractHandler.SendRequestAsync(issueRebalancingSetWithEtherFunction);
        }

        public Task<TransactionReceipt> IssueRebalancingSetWithEtherRequestAndWaitForReceiptAsync(IssueRebalancingSetWithEtherFunction issueRebalancingSetWithEtherFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(issueRebalancingSetWithEtherFunction, cancellationToken);
        }

        public Task<string> IssueRebalancingSetWithEtherRequestAsync(string rebalancingSetAddress, BigInteger rebalancingSetQuantity, ExchangeIssuanceParams exchangeIssuanceParams, byte[] orderData, bool keepChangeInVault)
        {
            var issueRebalancingSetWithEtherFunction = new IssueRebalancingSetWithEtherFunction();
                issueRebalancingSetWithEtherFunction.RebalancingSetAddress = rebalancingSetAddress;
                issueRebalancingSetWithEtherFunction.RebalancingSetQuantity = rebalancingSetQuantity;
                issueRebalancingSetWithEtherFunction.ExchangeIssuanceParams = exchangeIssuanceParams;
                issueRebalancingSetWithEtherFunction.OrderData = orderData;
                issueRebalancingSetWithEtherFunction.KeepChangeInVault = keepChangeInVault;
            
             return ContractHandler.SendRequestAsync(issueRebalancingSetWithEtherFunction);
        }

        public Task<TransactionReceipt> IssueRebalancingSetWithEtherRequestAndWaitForReceiptAsync(string rebalancingSetAddress, BigInteger rebalancingSetQuantity, ExchangeIssuanceParams exchangeIssuanceParams, byte[] orderData, bool keepChangeInVault, CancellationTokenSource cancellationToken = null)
        {
            var issueRebalancingSetWithEtherFunction = new IssueRebalancingSetWithEtherFunction();
                issueRebalancingSetWithEtherFunction.RebalancingSetAddress = rebalancingSetAddress;
                issueRebalancingSetWithEtherFunction.RebalancingSetQuantity = rebalancingSetQuantity;
                issueRebalancingSetWithEtherFunction.ExchangeIssuanceParams = exchangeIssuanceParams;
                issueRebalancingSetWithEtherFunction.OrderData = orderData;
                issueRebalancingSetWithEtherFunction.KeepChangeInVault = keepChangeInVault;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(issueRebalancingSetWithEtherFunction, cancellationToken);
        }

        public Task<string> TransferProxyInstanceQueryAsync(TransferProxyInstanceFunction transferProxyInstanceFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<TransferProxyInstanceFunction, string>(transferProxyInstanceFunction, blockParameter);
        }

        
        public Task<string> TransferProxyInstanceQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<TransferProxyInstanceFunction, string>(null, blockParameter);
        }

        public Task<string> ExchangeIssuanceModuleInstanceQueryAsync(ExchangeIssuanceModuleInstanceFunction exchangeIssuanceModuleInstanceFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<ExchangeIssuanceModuleInstanceFunction, string>(exchangeIssuanceModuleInstanceFunction, blockParameter);
        }

        
        public Task<string> ExchangeIssuanceModuleInstanceQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<ExchangeIssuanceModuleInstanceFunction, string>(null, blockParameter);
        }

        public Task<string> RedeemRebalancingSetIntoEtherRequestAsync(RedeemRebalancingSetIntoEtherFunction redeemRebalancingSetIntoEtherFunction)
        {
             return ContractHandler.SendRequestAsync(redeemRebalancingSetIntoEtherFunction);
        }

        public Task<TransactionReceipt> RedeemRebalancingSetIntoEtherRequestAndWaitForReceiptAsync(RedeemRebalancingSetIntoEtherFunction redeemRebalancingSetIntoEtherFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(redeemRebalancingSetIntoEtherFunction, cancellationToken);
        }

        public Task<string> RedeemRebalancingSetIntoEtherRequestAsync(string rebalancingSetAddress, BigInteger rebalancingSetQuantity, ExchangeIssuanceParams exchangeIssuanceParams, byte[] orderData, bool keepChangeInVault)
        {
            var redeemRebalancingSetIntoEtherFunction = new RedeemRebalancingSetIntoEtherFunction();
                redeemRebalancingSetIntoEtherFunction.RebalancingSetAddress = rebalancingSetAddress;
                redeemRebalancingSetIntoEtherFunction.RebalancingSetQuantity = rebalancingSetQuantity;
                redeemRebalancingSetIntoEtherFunction.ExchangeIssuanceParams = exchangeIssuanceParams;
                redeemRebalancingSetIntoEtherFunction.OrderData = orderData;
                redeemRebalancingSetIntoEtherFunction.KeepChangeInVault = keepChangeInVault;
            
             return ContractHandler.SendRequestAsync(redeemRebalancingSetIntoEtherFunction);
        }

        public Task<TransactionReceipt> RedeemRebalancingSetIntoEtherRequestAndWaitForReceiptAsync(string rebalancingSetAddress, BigInteger rebalancingSetQuantity, ExchangeIssuanceParams exchangeIssuanceParams, byte[] orderData, bool keepChangeInVault, CancellationTokenSource cancellationToken = null)
        {
            var redeemRebalancingSetIntoEtherFunction = new RedeemRebalancingSetIntoEtherFunction();
                redeemRebalancingSetIntoEtherFunction.RebalancingSetAddress = rebalancingSetAddress;
                redeemRebalancingSetIntoEtherFunction.RebalancingSetQuantity = rebalancingSetQuantity;
                redeemRebalancingSetIntoEtherFunction.ExchangeIssuanceParams = exchangeIssuanceParams;
                redeemRebalancingSetIntoEtherFunction.OrderData = orderData;
                redeemRebalancingSetIntoEtherFunction.KeepChangeInVault = keepChangeInVault;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(redeemRebalancingSetIntoEtherFunction, cancellationToken);
        }

        public Task<string> VaultInstanceQueryAsync(VaultInstanceFunction vaultInstanceFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<VaultInstanceFunction, string>(vaultInstanceFunction, blockParameter);
        }

        
        public Task<string> VaultInstanceQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<VaultInstanceFunction, string>(null, blockParameter);
        }

        public Task<string> WethInstanceQueryAsync(WethInstanceFunction wethInstanceFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<WethInstanceFunction, string>(wethInstanceFunction, blockParameter);
        }

        
        public Task<string> WethInstanceQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<WethInstanceFunction, string>(null, blockParameter);
        }

        public Task<string> CoreInstanceQueryAsync(CoreInstanceFunction coreInstanceFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<CoreInstanceFunction, string>(coreInstanceFunction, blockParameter);
        }

        
        public Task<string> CoreInstanceQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<CoreInstanceFunction, string>(null, blockParameter);
        }

        public Task<string> RedeemRebalancingSetIntoERC20RequestAsync(RedeemRebalancingSetIntoERC20Function redeemRebalancingSetIntoERC20Function)
        {
             return ContractHandler.SendRequestAsync(redeemRebalancingSetIntoERC20Function);
        }

        public Task<TransactionReceipt> RedeemRebalancingSetIntoERC20RequestAndWaitForReceiptAsync(RedeemRebalancingSetIntoERC20Function redeemRebalancingSetIntoERC20Function, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(redeemRebalancingSetIntoERC20Function, cancellationToken);
        }

        public Task<string> RedeemRebalancingSetIntoERC20RequestAsync(string rebalancingSetAddress, BigInteger rebalancingSetQuantity, string outputTokenAddress, ExchangeIssuanceParams exchangeIssuanceParams, byte[] orderData, bool keepChangeInVault)
        {
            var redeemRebalancingSetIntoERC20Function = new RedeemRebalancingSetIntoERC20Function();
                redeemRebalancingSetIntoERC20Function.RebalancingSetAddress = rebalancingSetAddress;
                redeemRebalancingSetIntoERC20Function.RebalancingSetQuantity = rebalancingSetQuantity;
                redeemRebalancingSetIntoERC20Function.OutputTokenAddress = outputTokenAddress;
                redeemRebalancingSetIntoERC20Function.ExchangeIssuanceParams = exchangeIssuanceParams;
                redeemRebalancingSetIntoERC20Function.OrderData = orderData;
                redeemRebalancingSetIntoERC20Function.KeepChangeInVault = keepChangeInVault;
            
             return ContractHandler.SendRequestAsync(redeemRebalancingSetIntoERC20Function);
        }

        public Task<TransactionReceipt> RedeemRebalancingSetIntoERC20RequestAndWaitForReceiptAsync(string rebalancingSetAddress, BigInteger rebalancingSetQuantity, string outputTokenAddress, ExchangeIssuanceParams exchangeIssuanceParams, byte[] orderData, bool keepChangeInVault, CancellationTokenSource cancellationToken = null)
        {
            var redeemRebalancingSetIntoERC20Function = new RedeemRebalancingSetIntoERC20Function();
                redeemRebalancingSetIntoERC20Function.RebalancingSetAddress = rebalancingSetAddress;
                redeemRebalancingSetIntoERC20Function.RebalancingSetQuantity = rebalancingSetQuantity;
                redeemRebalancingSetIntoERC20Function.OutputTokenAddress = outputTokenAddress;
                redeemRebalancingSetIntoERC20Function.ExchangeIssuanceParams = exchangeIssuanceParams;
                redeemRebalancingSetIntoERC20Function.OrderData = orderData;
                redeemRebalancingSetIntoERC20Function.KeepChangeInVault = keepChangeInVault;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(redeemRebalancingSetIntoERC20Function, cancellationToken);
        }

        public Task<string> IssueRebalancingSetWithERC20RequestAsync(IssueRebalancingSetWithERC20Function issueRebalancingSetWithERC20Function)
        {
             return ContractHandler.SendRequestAsync(issueRebalancingSetWithERC20Function);
        }

        public Task<TransactionReceipt> IssueRebalancingSetWithERC20RequestAndWaitForReceiptAsync(IssueRebalancingSetWithERC20Function issueRebalancingSetWithERC20Function, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(issueRebalancingSetWithERC20Function, cancellationToken);
        }

        public Task<string> IssueRebalancingSetWithERC20RequestAsync(string rebalancingSetAddress, BigInteger rebalancingSetQuantity, string paymentTokenAddress, BigInteger paymentTokenQuantity, ExchangeIssuanceParams exchangeIssuanceParams, byte[] orderData, bool keepChangeInVault)
        {
            var issueRebalancingSetWithERC20Function = new IssueRebalancingSetWithERC20Function();
                issueRebalancingSetWithERC20Function.RebalancingSetAddress = rebalancingSetAddress;
                issueRebalancingSetWithERC20Function.RebalancingSetQuantity = rebalancingSetQuantity;
                issueRebalancingSetWithERC20Function.PaymentTokenAddress = paymentTokenAddress;
                issueRebalancingSetWithERC20Function.PaymentTokenQuantity = paymentTokenQuantity;
                issueRebalancingSetWithERC20Function.ExchangeIssuanceParams = exchangeIssuanceParams;
                issueRebalancingSetWithERC20Function.OrderData = orderData;
                issueRebalancingSetWithERC20Function.KeepChangeInVault = keepChangeInVault;
            
             return ContractHandler.SendRequestAsync(issueRebalancingSetWithERC20Function);
        }

        public Task<TransactionReceipt> IssueRebalancingSetWithERC20RequestAndWaitForReceiptAsync(string rebalancingSetAddress, BigInteger rebalancingSetQuantity, string paymentTokenAddress, BigInteger paymentTokenQuantity, ExchangeIssuanceParams exchangeIssuanceParams, byte[] orderData, bool keepChangeInVault, CancellationTokenSource cancellationToken = null)
        {
            var issueRebalancingSetWithERC20Function = new IssueRebalancingSetWithERC20Function();
                issueRebalancingSetWithERC20Function.RebalancingSetAddress = rebalancingSetAddress;
                issueRebalancingSetWithERC20Function.RebalancingSetQuantity = rebalancingSetQuantity;
                issueRebalancingSetWithERC20Function.PaymentTokenAddress = paymentTokenAddress;
                issueRebalancingSetWithERC20Function.PaymentTokenQuantity = paymentTokenQuantity;
                issueRebalancingSetWithERC20Function.ExchangeIssuanceParams = exchangeIssuanceParams;
                issueRebalancingSetWithERC20Function.OrderData = orderData;
                issueRebalancingSetWithERC20Function.KeepChangeInVault = keepChangeInVault;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(issueRebalancingSetWithERC20Function, cancellationToken);
        }
    }
}
