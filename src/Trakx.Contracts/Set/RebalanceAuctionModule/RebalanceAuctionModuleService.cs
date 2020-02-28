using System.Threading.Tasks;
using System.Numerics;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.ContractHandlers;
using System.Threading;
using Trakx.Contracts.Set.RebalanceAuctionModule.ContractDefinition;

namespace Trakx.Contracts.Set.RebalanceAuctionModule
{
    public partial class RebalanceAuctionModuleService
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.Web3 web3, RebalanceAuctionModuleDeployment rebalanceAuctionModuleDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<RebalanceAuctionModuleDeployment>().SendRequestAndWaitForReceiptAsync(rebalanceAuctionModuleDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.Web3 web3, RebalanceAuctionModuleDeployment rebalanceAuctionModuleDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<RebalanceAuctionModuleDeployment>().SendRequestAsync(rebalanceAuctionModuleDeployment);
        }

        public static async Task<RebalanceAuctionModuleService> DeployContractAndGetServiceAsync(Nethereum.Web3.Web3 web3, RebalanceAuctionModuleDeployment rebalanceAuctionModuleDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, rebalanceAuctionModuleDeployment, cancellationTokenSource);
            return new RebalanceAuctionModuleService(web3, receipt.ContractAddress);
        }

        protected Nethereum.Web3.Web3 Web3{ get; }

        public ContractHandler ContractHandler { get; }

        public RebalanceAuctionModuleService(Nethereum.Web3.Web3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }

        public Task<string> VaultInstanceQueryAsync(VaultInstanceFunction vaultInstanceFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<VaultInstanceFunction, string>(vaultInstanceFunction, blockParameter);
        }

        
        public Task<string> VaultInstanceQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<VaultInstanceFunction, string>(null, blockParameter);
        }

        public Task<string> BidAndWithdrawRequestAsync(BidAndWithdrawFunction bidAndWithdrawFunction)
        {
             return ContractHandler.SendRequestAsync(bidAndWithdrawFunction);
        }

        public Task<TransactionReceipt> BidAndWithdrawRequestAndWaitForReceiptAsync(BidAndWithdrawFunction bidAndWithdrawFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(bidAndWithdrawFunction, cancellationToken);
        }

        public Task<string> BidAndWithdrawRequestAsync(string rebalancingSetToken, BigInteger quantity, bool allowPartialFill)
        {
            var bidAndWithdrawFunction = new BidAndWithdrawFunction();
                bidAndWithdrawFunction.RebalancingSetToken = rebalancingSetToken;
                bidAndWithdrawFunction.Quantity = quantity;
                bidAndWithdrawFunction.AllowPartialFill = allowPartialFill;
            
             return ContractHandler.SendRequestAsync(bidAndWithdrawFunction);
        }

        public Task<TransactionReceipt> BidAndWithdrawRequestAndWaitForReceiptAsync(string rebalancingSetToken, BigInteger quantity, bool allowPartialFill, CancellationTokenSource cancellationToken = null)
        {
            var bidAndWithdrawFunction = new BidAndWithdrawFunction();
                bidAndWithdrawFunction.RebalancingSetToken = rebalancingSetToken;
                bidAndWithdrawFunction.Quantity = quantity;
                bidAndWithdrawFunction.AllowPartialFill = allowPartialFill;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(bidAndWithdrawFunction, cancellationToken);
        }

        public Task<string> BidRequestAsync(BidFunction bidFunction)
        {
             return ContractHandler.SendRequestAsync(bidFunction);
        }

        public Task<TransactionReceipt> BidRequestAndWaitForReceiptAsync(BidFunction bidFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(bidFunction, cancellationToken);
        }

        public Task<string> BidRequestAsync(string rebalancingSetToken, BigInteger quantity, bool allowPartialFill)
        {
            var bidFunction = new BidFunction();
                bidFunction.RebalancingSetToken = rebalancingSetToken;
                bidFunction.Quantity = quantity;
                bidFunction.AllowPartialFill = allowPartialFill;
            
             return ContractHandler.SendRequestAsync(bidFunction);
        }

        public Task<TransactionReceipt> BidRequestAndWaitForReceiptAsync(string rebalancingSetToken, BigInteger quantity, bool allowPartialFill, CancellationTokenSource cancellationToken = null)
        {
            var bidFunction = new BidFunction();
                bidFunction.RebalancingSetToken = rebalancingSetToken;
                bidFunction.Quantity = quantity;
                bidFunction.AllowPartialFill = allowPartialFill;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(bidFunction, cancellationToken);
        }

        public Task<string> CoreInstanceQueryAsync(CoreInstanceFunction coreInstanceFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<CoreInstanceFunction, string>(coreInstanceFunction, blockParameter);
        }

        
        public Task<string> CoreInstanceQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<CoreInstanceFunction, string>(null, blockParameter);
        }

        public Task<string> RedeemFromFailedRebalanceRequestAsync(RedeemFromFailedRebalanceFunction redeemFromFailedRebalanceFunction)
        {
             return ContractHandler.SendRequestAsync(redeemFromFailedRebalanceFunction);
        }

        public Task<TransactionReceipt> RedeemFromFailedRebalanceRequestAndWaitForReceiptAsync(RedeemFromFailedRebalanceFunction redeemFromFailedRebalanceFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(redeemFromFailedRebalanceFunction, cancellationToken);
        }

        public Task<string> RedeemFromFailedRebalanceRequestAsync(string rebalancingSetToken)
        {
            var redeemFromFailedRebalanceFunction = new RedeemFromFailedRebalanceFunction();
                redeemFromFailedRebalanceFunction.RebalancingSetToken = rebalancingSetToken;
            
             return ContractHandler.SendRequestAsync(redeemFromFailedRebalanceFunction);
        }

        public Task<TransactionReceipt> RedeemFromFailedRebalanceRequestAndWaitForReceiptAsync(string rebalancingSetToken, CancellationTokenSource cancellationToken = null)
        {
            var redeemFromFailedRebalanceFunction = new RedeemFromFailedRebalanceFunction();
                redeemFromFailedRebalanceFunction.RebalancingSetToken = rebalancingSetToken;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(redeemFromFailedRebalanceFunction, cancellationToken);
        }

        public Task<string> CoreQueryAsync(CoreFunction coreFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<CoreFunction, string>(coreFunction, blockParameter);
        }

        
        public Task<string> CoreQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<CoreFunction, string>(null, blockParameter);
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
