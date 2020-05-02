using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Trakx.Contracts.Set.RebalancingSetToken.ContractDefinition;

namespace Trakx.Contracts.Set.RebalancingSetToken
{
    public partial class RebalancingSetTokenService
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(IWeb3 web3, RebalancingSetTokenDeployment rebalancingSetTokenDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<RebalancingSetTokenDeployment>().SendRequestAndWaitForReceiptAsync(rebalancingSetTokenDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(IWeb3 web3, RebalancingSetTokenDeployment rebalancingSetTokenDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<RebalancingSetTokenDeployment>().SendRequestAsync(rebalancingSetTokenDeployment);
        }

        public static async Task<RebalancingSetTokenService> DeployContractAndGetServiceAsync(IWeb3 web3, RebalancingSetTokenDeployment rebalancingSetTokenDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, rebalancingSetTokenDeployment, cancellationTokenSource);
            return new RebalancingSetTokenService(web3, receipt.ContractAddress);
        }

        protected IWeb3 Web3{ get; }

        public ContractHandler ContractHandler { get; }

        public RebalancingSetTokenService(IWeb3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }

        public Task<BigInteger> UnitSharesQueryAsync(UnitSharesFunction unitSharesFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<UnitSharesFunction, BigInteger>(unitSharesFunction, blockParameter);
        }

        
        public Task<BigInteger> UnitSharesQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<UnitSharesFunction, BigInteger>(null, blockParameter);
        }

        public Task<List<BigInteger>> GetUnitsQueryAsync(GetUnitsFunction getUnitsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetUnitsFunction, List<BigInteger>>(getUnitsFunction, blockParameter);
        }

        
        public Task<List<BigInteger>> GetUnitsQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetUnitsFunction, List<BigInteger>>(null, blockParameter);
        }

        public Task<string> NameQueryAsync(NameFunction nameFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<NameFunction, string>(nameFunction, blockParameter);
        }

        
        public Task<string> NameQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<NameFunction, string>(null, blockParameter);
        }

        public Task<string> ApproveRequestAsync(ApproveFunction approveFunction)
        {
             return ContractHandler.SendRequestAsync(approveFunction);
        }

        public Task<TransactionReceipt> ApproveRequestAndWaitForReceiptAsync(ApproveFunction approveFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(approveFunction, cancellationToken);
        }

        public Task<string> ApproveRequestAsync(string spender, BigInteger value)
        {
            var approveFunction = new ApproveFunction();
                approveFunction.Spender = spender;
                approveFunction.Value = value;
            
             return ContractHandler.SendRequestAsync(approveFunction);
        }

        public Task<TransactionReceipt> ApproveRequestAndWaitForReceiptAsync(string spender, BigInteger value, CancellationTokenSource cancellationToken = null)
        {
            var approveFunction = new ApproveFunction();
                approveFunction.Spender = spender;
                approveFunction.Value = value;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(approveFunction, cancellationToken);
        }

        public Task<string> FailedAuctionWithdrawComponentsQueryAsync(FailedAuctionWithdrawComponentsFunction failedAuctionWithdrawComponentsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<FailedAuctionWithdrawComponentsFunction, string>(failedAuctionWithdrawComponentsFunction, blockParameter);
        }

        
        public Task<string> FailedAuctionWithdrawComponentsQueryAsync(BigInteger returnValue1, BlockParameter blockParameter = null)
        {
            var failedAuctionWithdrawComponentsFunction = new FailedAuctionWithdrawComponentsFunction();
                failedAuctionWithdrawComponentsFunction.ReturnValue1 = returnValue1;
            
            return ContractHandler.QueryAsync<FailedAuctionWithdrawComponentsFunction, string>(failedAuctionWithdrawComponentsFunction, blockParameter);
        }

        public Task<BigInteger> RebalanceIntervalQueryAsync(RebalanceIntervalFunction rebalanceIntervalFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<RebalanceIntervalFunction, BigInteger>(rebalanceIntervalFunction, blockParameter);
        }

        
        public Task<BigInteger> RebalanceIntervalQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<RebalanceIntervalFunction, BigInteger>(null, blockParameter);
        }

        public Task<BigInteger> TotalSupplyQueryAsync(TotalSupplyFunction totalSupplyFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<TotalSupplyFunction, BigInteger>(totalSupplyFunction, blockParameter);
        }

        
        public Task<BigInteger> TotalSupplyQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<TotalSupplyFunction, BigInteger>(null, blockParameter);
        }

        public Task<bool> TokenIsComponentQueryAsync(TokenIsComponentFunction tokenIsComponentFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<TokenIsComponentFunction, bool>(tokenIsComponentFunction, blockParameter);
        }

        
        public Task<bool> TokenIsComponentQueryAsync(string tokenAddress, BlockParameter blockParameter = null)
        {
            var tokenIsComponentFunction = new TokenIsComponentFunction();
                tokenIsComponentFunction.TokenAddress = tokenAddress;
            
            return ContractHandler.QueryAsync<TokenIsComponentFunction, bool>(tokenIsComponentFunction, blockParameter);
        }

        public Task<string> TransferFromRequestAsync(TransferFromFunction transferFromFunction)
        {
             return ContractHandler.SendRequestAsync(transferFromFunction);
        }

        public Task<TransactionReceipt> TransferFromRequestAndWaitForReceiptAsync(TransferFromFunction transferFromFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferFromFunction, cancellationToken);
        }

        public Task<string> TransferFromRequestAsync(string from, string to, BigInteger value)
        {
            var transferFromFunction = new TransferFromFunction();
                transferFromFunction.From = from;
                transferFromFunction.To = to;
                transferFromFunction.Value = value;
            
             return ContractHandler.SendRequestAsync(transferFromFunction);
        }

        public Task<TransactionReceipt> TransferFromRequestAndWaitForReceiptAsync(string from, string to, BigInteger value, CancellationTokenSource cancellationToken = null)
        {
            var transferFromFunction = new TransferFromFunction();
                transferFromFunction.From = from;
                transferFromFunction.To = to;
                transferFromFunction.Value = value;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferFromFunction, cancellationToken);
        }

        public Task<BigInteger> ProposalPeriodQueryAsync(ProposalPeriodFunction proposalPeriodFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<ProposalPeriodFunction, BigInteger>(proposalPeriodFunction, blockParameter);
        }

        
        public Task<BigInteger> ProposalPeriodQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<ProposalPeriodFunction, BigInteger>(null, blockParameter);
        }

        public Task<string> CurrentSetQueryAsync(CurrentSetFunction currentSetFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<CurrentSetFunction, string>(currentSetFunction, blockParameter);
        }

        
        public Task<string> CurrentSetQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<CurrentSetFunction, string>(null, blockParameter);
        }

        public Task<byte> DecimalsQueryAsync(DecimalsFunction decimalsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<DecimalsFunction, byte>(decimalsFunction, blockParameter);
        }

        
        public Task<byte> DecimalsQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<DecimalsFunction, byte>(null, blockParameter);
        }

        public Task<string> IncreaseAllowanceRequestAsync(IncreaseAllowanceFunction increaseAllowanceFunction)
        {
             return ContractHandler.SendRequestAsync(increaseAllowanceFunction);
        }

        public Task<TransactionReceipt> IncreaseAllowanceRequestAndWaitForReceiptAsync(IncreaseAllowanceFunction increaseAllowanceFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(increaseAllowanceFunction, cancellationToken);
        }

        public Task<string> IncreaseAllowanceRequestAsync(string spender, BigInteger addedValue)
        {
            var increaseAllowanceFunction = new IncreaseAllowanceFunction();
                increaseAllowanceFunction.Spender = spender;
                increaseAllowanceFunction.AddedValue = addedValue;
            
             return ContractHandler.SendRequestAsync(increaseAllowanceFunction);
        }

        public Task<TransactionReceipt> IncreaseAllowanceRequestAndWaitForReceiptAsync(string spender, BigInteger addedValue, CancellationTokenSource cancellationToken = null)
        {
            var increaseAllowanceFunction = new IncreaseAllowanceFunction();
                increaseAllowanceFunction.Spender = spender;
                increaseAllowanceFunction.AddedValue = addedValue;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(increaseAllowanceFunction, cancellationToken);
        }

        public Task<string> MintRequestAsync(MintFunction mintFunction)
        {
             return ContractHandler.SendRequestAsync(mintFunction);
        }

        public Task<TransactionReceipt> MintRequestAndWaitForReceiptAsync(MintFunction mintFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(mintFunction, cancellationToken);
        }

        public Task<string> MintRequestAsync(string issuer, BigInteger quantity)
        {
            var mintFunction = new MintFunction();
                mintFunction.Issuer = issuer;
                mintFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAsync(mintFunction);
        }

        public Task<TransactionReceipt> MintRequestAndWaitForReceiptAsync(string issuer, BigInteger quantity, CancellationTokenSource cancellationToken = null)
        {
            var mintFunction = new MintFunction();
                mintFunction.Issuer = issuer;
                mintFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(mintFunction, cancellationToken);
        }

        public Task<BigInteger> NaturalUnitQueryAsync(NaturalUnitFunction naturalUnitFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<NaturalUnitFunction, BigInteger>(naturalUnitFunction, blockParameter);
        }

        
        public Task<BigInteger> NaturalUnitQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<NaturalUnitFunction, BigInteger>(null, blockParameter);
        }

        public Task<BigInteger> StartingCurrentSetAmountQueryAsync(StartingCurrentSetAmountFunction startingCurrentSetAmountFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<StartingCurrentSetAmountFunction, BigInteger>(startingCurrentSetAmountFunction, blockParameter);
        }

        
        public Task<BigInteger> StartingCurrentSetAmountQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<StartingCurrentSetAmountFunction, BigInteger>(null, blockParameter);
        }

        public Task<string> ManagerQueryAsync(ManagerFunction managerFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<ManagerFunction, string>(managerFunction, blockParameter);
        }

        
        public Task<string> ManagerQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<ManagerFunction, string>(null, blockParameter);
        }

        public Task<string> StartRebalanceRequestAsync(StartRebalanceFunction startRebalanceFunction)
        {
             return ContractHandler.SendRequestAsync(startRebalanceFunction);
        }

        public Task<string> StartRebalanceRequestAsync()
        {
             return ContractHandler.SendRequestAsync<StartRebalanceFunction>();
        }

        public Task<TransactionReceipt> StartRebalanceRequestAndWaitForReceiptAsync(StartRebalanceFunction startRebalanceFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(startRebalanceFunction, cancellationToken);
        }

        public Task<TransactionReceipt> StartRebalanceRequestAndWaitForReceiptAsync(CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync<StartRebalanceFunction>(null, cancellationToken);
        }

        public Task<BigInteger> GetCombinedTokenArrayLengthQueryAsync(GetCombinedTokenArrayLengthFunction getCombinedTokenArrayLengthFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetCombinedTokenArrayLengthFunction, BigInteger>(getCombinedTokenArrayLengthFunction, blockParameter);
        }

        
        public Task<BigInteger> GetCombinedTokenArrayLengthQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetCombinedTokenArrayLengthFunction, BigInteger>(null, blockParameter);
        }

        public Task<BiddingParametersOutputDTO> BiddingParametersQueryAsync(BiddingParametersFunction biddingParametersFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<BiddingParametersFunction, BiddingParametersOutputDTO>(biddingParametersFunction, blockParameter);
        }

        public Task<BiddingParametersOutputDTO> BiddingParametersQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<BiddingParametersFunction, BiddingParametersOutputDTO>(null, blockParameter);
        }

        public Task<BigInteger> BalanceOfQueryAsync(BalanceOfFunction balanceOfFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<BalanceOfFunction, BigInteger>(balanceOfFunction, blockParameter);
        }

        
        public Task<BigInteger> BalanceOfQueryAsync(string owner, BlockParameter blockParameter = null)
        {
            var balanceOfFunction = new BalanceOfFunction();
                balanceOfFunction.Owner = owner;
            
            return ContractHandler.QueryAsync<BalanceOfFunction, BigInteger>(balanceOfFunction, blockParameter);
        }

        public Task<string> EndFailedAuctionRequestAsync(EndFailedAuctionFunction endFailedAuctionFunction)
        {
             return ContractHandler.SendRequestAsync(endFailedAuctionFunction);
        }

        public Task<string> EndFailedAuctionRequestAsync()
        {
             return ContractHandler.SendRequestAsync<EndFailedAuctionFunction>();
        }

        public Task<TransactionReceipt> EndFailedAuctionRequestAndWaitForReceiptAsync(EndFailedAuctionFunction endFailedAuctionFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(endFailedAuctionFunction, cancellationToken);
        }

        public Task<TransactionReceipt> EndFailedAuctionRequestAndWaitForReceiptAsync(CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync<EndFailedAuctionFunction>(null, cancellationToken);
        }

        public Task<string> ProposeRequestAsync(ProposeFunction proposeFunction)
        {
             return ContractHandler.SendRequestAsync(proposeFunction);
        }

        public Task<TransactionReceipt> ProposeRequestAndWaitForReceiptAsync(ProposeFunction proposeFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(proposeFunction, cancellationToken);
        }

        public Task<string> ProposeRequestAsync(string nextSet, string auctionLibrary, BigInteger auctionTimeToPivot, BigInteger auctionStartPrice, BigInteger auctionPivotPrice)
        {
            var proposeFunction = new ProposeFunction();
                proposeFunction.NextSet = nextSet;
                proposeFunction.AuctionLibrary = auctionLibrary;
                proposeFunction.AuctionTimeToPivot = auctionTimeToPivot;
                proposeFunction.AuctionStartPrice = auctionStartPrice;
                proposeFunction.AuctionPivotPrice = auctionPivotPrice;
            
             return ContractHandler.SendRequestAsync(proposeFunction);
        }

        public Task<TransactionReceipt> ProposeRequestAndWaitForReceiptAsync(string nextSet, string auctionLibrary, BigInteger auctionTimeToPivot, BigInteger auctionStartPrice, BigInteger auctionPivotPrice, CancellationTokenSource cancellationToken = null)
        {
            var proposeFunction = new ProposeFunction();
                proposeFunction.NextSet = nextSet;
                proposeFunction.AuctionLibrary = auctionLibrary;
                proposeFunction.AuctionTimeToPivot = auctionTimeToPivot;
                proposeFunction.AuctionStartPrice = auctionStartPrice;
                proposeFunction.AuctionPivotPrice = auctionPivotPrice;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(proposeFunction, cancellationToken);
        }

        public Task<List<string>> GetCombinedTokenArrayQueryAsync(GetCombinedTokenArrayFunction getCombinedTokenArrayFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetCombinedTokenArrayFunction, List<string>>(getCombinedTokenArrayFunction, blockParameter);
        }

        
        public Task<List<string>> GetCombinedTokenArrayQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetCombinedTokenArrayFunction, List<string>>(null, blockParameter);
        }

        public Task<string> SymbolQueryAsync(SymbolFunction symbolFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<SymbolFunction, string>(symbolFunction, blockParameter);
        }

        
        public Task<string> SymbolQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<SymbolFunction, string>(null, blockParameter);
        }

        public Task<string> AuctionLibraryQueryAsync(AuctionLibraryFunction auctionLibraryFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<AuctionLibraryFunction, string>(auctionLibraryFunction, blockParameter);
        }

        
        public Task<string> AuctionLibraryQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<AuctionLibraryFunction, string>(null, blockParameter);
        }

        public Task<string> PlaceBidRequestAsync(PlaceBidFunction placeBidFunction)
        {
             return ContractHandler.SendRequestAsync(placeBidFunction);
        }

        public Task<TransactionReceipt> PlaceBidRequestAndWaitForReceiptAsync(PlaceBidFunction placeBidFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(placeBidFunction, cancellationToken);
        }

        public Task<string> PlaceBidRequestAsync(BigInteger quantity)
        {
            var placeBidFunction = new PlaceBidFunction();
                placeBidFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAsync(placeBidFunction);
        }

        public Task<TransactionReceipt> PlaceBidRequestAndWaitForReceiptAsync(BigInteger quantity, CancellationTokenSource cancellationToken = null)
        {
            var placeBidFunction = new PlaceBidFunction();
                placeBidFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(placeBidFunction, cancellationToken);
        }

        public Task<List<string>> GetComponentsQueryAsync(GetComponentsFunction getComponentsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetComponentsFunction, List<string>>(getComponentsFunction, blockParameter);
        }

        
        public Task<List<string>> GetComponentsQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetComponentsFunction, List<string>>(null, blockParameter);
        }

        public Task<GetBidPriceOutputDTO> GetBidPriceQueryAsync(GetBidPriceFunction getBidPriceFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetBidPriceFunction, GetBidPriceOutputDTO>(getBidPriceFunction, blockParameter);
        }

        public Task<GetBidPriceOutputDTO> GetBidPriceQueryAsync(BigInteger quantity, BlockParameter blockParameter = null)
        {
            var getBidPriceFunction = new GetBidPriceFunction();
                getBidPriceFunction.Quantity = quantity;
            
            return ContractHandler.QueryDeserializingToObjectAsync<GetBidPriceFunction, GetBidPriceOutputDTO>(getBidPriceFunction, blockParameter);
        }

        public Task<string> BurnRequestAsync(BurnFunction burnFunction)
        {
             return ContractHandler.SendRequestAsync(burnFunction);
        }

        public Task<TransactionReceipt> BurnRequestAndWaitForReceiptAsync(BurnFunction burnFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(burnFunction, cancellationToken);
        }

        public Task<string> BurnRequestAsync(string from, BigInteger quantity)
        {
            var burnFunction = new BurnFunction();
                burnFunction.From = from;
                burnFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAsync(burnFunction);
        }

        public Task<TransactionReceipt> BurnRequestAndWaitForReceiptAsync(string from, BigInteger quantity, CancellationTokenSource cancellationToken = null)
        {
            var burnFunction = new BurnFunction();
                burnFunction.From = from;
                burnFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(burnFunction, cancellationToken);
        }

        public Task<List<BigInteger>> GetAuctionPriceParametersQueryAsync(GetAuctionPriceParametersFunction getAuctionPriceParametersFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetAuctionPriceParametersFunction, List<BigInteger>>(getAuctionPriceParametersFunction, blockParameter);
        }

        
        public Task<List<BigInteger>> GetAuctionPriceParametersQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetAuctionPriceParametersFunction, List<BigInteger>>(null, blockParameter);
        }

        public Task<List<BigInteger>> GetBiddingParametersQueryAsync(GetBiddingParametersFunction getBiddingParametersFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetBiddingParametersFunction, List<BigInteger>>(getBiddingParametersFunction, blockParameter);
        }

        
        public Task<List<BigInteger>> GetBiddingParametersQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetBiddingParametersFunction, List<BigInteger>>(null, blockParameter);
        }

        public Task<string> DecreaseAllowanceRequestAsync(DecreaseAllowanceFunction decreaseAllowanceFunction)
        {
             return ContractHandler.SendRequestAsync(decreaseAllowanceFunction);
        }

        public Task<TransactionReceipt> DecreaseAllowanceRequestAndWaitForReceiptAsync(DecreaseAllowanceFunction decreaseAllowanceFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(decreaseAllowanceFunction, cancellationToken);
        }

        public Task<string> DecreaseAllowanceRequestAsync(string spender, BigInteger subtractedValue)
        {
            var decreaseAllowanceFunction = new DecreaseAllowanceFunction();
                decreaseAllowanceFunction.Spender = spender;
                decreaseAllowanceFunction.SubtractedValue = subtractedValue;
            
             return ContractHandler.SendRequestAsync(decreaseAllowanceFunction);
        }

        public Task<TransactionReceipt> DecreaseAllowanceRequestAndWaitForReceiptAsync(string spender, BigInteger subtractedValue, CancellationTokenSource cancellationToken = null)
        {
            var decreaseAllowanceFunction = new DecreaseAllowanceFunction();
                decreaseAllowanceFunction.Spender = spender;
                decreaseAllowanceFunction.SubtractedValue = subtractedValue;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(decreaseAllowanceFunction, cancellationToken);
        }

        public Task<string> TransferRequestAsync(TransferFunction transferFunction)
        {
             return ContractHandler.SendRequestAsync(transferFunction);
        }

        public Task<TransactionReceipt> TransferRequestAndWaitForReceiptAsync(TransferFunction transferFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferFunction, cancellationToken);
        }

        public Task<string> TransferRequestAsync(string to, BigInteger value)
        {
            var transferFunction = new TransferFunction();
                transferFunction.To = to;
                transferFunction.Value = value;
            
             return ContractHandler.SendRequestAsync(transferFunction);
        }

        public Task<TransactionReceipt> TransferRequestAndWaitForReceiptAsync(string to, BigInteger value, CancellationTokenSource cancellationToken = null)
        {
            var transferFunction = new TransferFunction();
                transferFunction.To = to;
                transferFunction.Value = value;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferFunction, cancellationToken);
        }

        public Task<BigInteger> ProposalStartTimeQueryAsync(ProposalStartTimeFunction proposalStartTimeFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<ProposalStartTimeFunction, BigInteger>(proposalStartTimeFunction, blockParameter);
        }

        
        public Task<BigInteger> ProposalStartTimeQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<ProposalStartTimeFunction, BigInteger>(null, blockParameter);
        }

        public Task<BigInteger> LastRebalanceTimestampQueryAsync(LastRebalanceTimestampFunction lastRebalanceTimestampFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<LastRebalanceTimestampFunction, BigInteger>(lastRebalanceTimestampFunction, blockParameter);
        }

        
        public Task<BigInteger> LastRebalanceTimestampQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<LastRebalanceTimestampFunction, BigInteger>(null, blockParameter);
        }

        public Task<string> FactoryQueryAsync(FactoryFunction factoryFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<FactoryFunction, string>(factoryFunction, blockParameter);
        }

        
        public Task<string> FactoryQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<FactoryFunction, string>(null, blockParameter);
        }

        public Task<string> NextSetQueryAsync(NextSetFunction nextSetFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<NextSetFunction, string>(nextSetFunction, blockParameter);
        }

        
        public Task<string> NextSetQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<NextSetFunction, string>(null, blockParameter);
        }

        public Task<List<BigInteger>> GetCombinedNextSetUnitsQueryAsync(GetCombinedNextSetUnitsFunction getCombinedNextSetUnitsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetCombinedNextSetUnitsFunction, List<BigInteger>>(getCombinedNextSetUnitsFunction, blockParameter);
        }

        
        public Task<List<BigInteger>> GetCombinedNextSetUnitsQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetCombinedNextSetUnitsFunction, List<BigInteger>>(null, blockParameter);
        }

        public Task<string> SetManagerRequestAsync(SetManagerFunction setManagerFunction)
        {
             return ContractHandler.SendRequestAsync(setManagerFunction);
        }

        public Task<TransactionReceipt> SetManagerRequestAndWaitForReceiptAsync(SetManagerFunction setManagerFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setManagerFunction, cancellationToken);
        }

        public Task<string> SetManagerRequestAsync(string newManager)
        {
            var setManagerFunction = new SetManagerFunction();
                setManagerFunction.NewManager = newManager;
            
             return ContractHandler.SendRequestAsync(setManagerFunction);
        }

        public Task<TransactionReceipt> SetManagerRequestAndWaitForReceiptAsync(string newManager, CancellationTokenSource cancellationToken = null)
        {
            var setManagerFunction = new SetManagerFunction();
                setManagerFunction.NewManager = newManager;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setManagerFunction, cancellationToken);
        }

        public Task<AuctionPriceParametersOutputDTO> AuctionPriceParametersQueryAsync(AuctionPriceParametersFunction auctionPriceParametersFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<AuctionPriceParametersFunction, AuctionPriceParametersOutputDTO>(auctionPriceParametersFunction, blockParameter);
        }

        public Task<AuctionPriceParametersOutputDTO> AuctionPriceParametersQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<AuctionPriceParametersFunction, AuctionPriceParametersOutputDTO>(null, blockParameter);
        }

        public Task<BigInteger> AllowanceQueryAsync(AllowanceFunction allowanceFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<AllowanceFunction, BigInteger>(allowanceFunction, blockParameter);
        }

        
        public Task<BigInteger> AllowanceQueryAsync(string owner, string spender, BlockParameter blockParameter = null)
        {
            var allowanceFunction = new AllowanceFunction();
                allowanceFunction.Owner = owner;
                allowanceFunction.Spender = spender;
            
            return ContractHandler.QueryAsync<AllowanceFunction, BigInteger>(allowanceFunction, blockParameter);
        }

        public Task<List<string>> GetFailedAuctionWithdrawComponentsQueryAsync(GetFailedAuctionWithdrawComponentsFunction getFailedAuctionWithdrawComponentsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetFailedAuctionWithdrawComponentsFunction, List<string>>(getFailedAuctionWithdrawComponentsFunction, blockParameter);
        }

        
        public Task<List<string>> GetFailedAuctionWithdrawComponentsQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetFailedAuctionWithdrawComponentsFunction, List<string>>(null, blockParameter);
        }

        public Task<List<BigInteger>> GetCombinedCurrentUnitsQueryAsync(GetCombinedCurrentUnitsFunction getCombinedCurrentUnitsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetCombinedCurrentUnitsFunction, List<BigInteger>>(getCombinedCurrentUnitsFunction, blockParameter);
        }

        
        public Task<List<BigInteger>> GetCombinedCurrentUnitsQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetCombinedCurrentUnitsFunction, List<BigInteger>>(null, blockParameter);
        }

        public Task<string> CoreQueryAsync(CoreFunction coreFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<CoreFunction, string>(coreFunction, blockParameter);
        }

        
        public Task<string> CoreQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<CoreFunction, string>(null, blockParameter);
        }

        public Task<byte> RebalanceStateQueryAsync(RebalanceStateFunction rebalanceStateFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<RebalanceStateFunction, byte>(rebalanceStateFunction, blockParameter);
        }

        
        public Task<byte> RebalanceStateQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<RebalanceStateFunction, byte>(null, blockParameter);
        }

        public Task<string> SettleRebalanceRequestAsync(SettleRebalanceFunction settleRebalanceFunction)
        {
             return ContractHandler.SendRequestAsync(settleRebalanceFunction);
        }

        public Task<string> SettleRebalanceRequestAsync()
        {
             return ContractHandler.SendRequestAsync<SettleRebalanceFunction>();
        }

        public Task<TransactionReceipt> SettleRebalanceRequestAndWaitForReceiptAsync(SettleRebalanceFunction settleRebalanceFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(settleRebalanceFunction, cancellationToken);
        }

        public Task<TransactionReceipt> SettleRebalanceRequestAndWaitForReceiptAsync(CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync<SettleRebalanceFunction>(null, cancellationToken);
        }

        public Task<string> VaultQueryAsync(VaultFunction vaultFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<VaultFunction, string>(vaultFunction, blockParameter);
        }

        
        public Task<string> VaultQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<VaultFunction, string>(null, blockParameter);
        }

        public Task<string> ComponentWhiteListAddressQueryAsync(ComponentWhiteListAddressFunction componentWhiteListAddressFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<ComponentWhiteListAddressFunction, string>(componentWhiteListAddressFunction, blockParameter);
        }

        
        public Task<string> ComponentWhiteListAddressQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<ComponentWhiteListAddressFunction, string>(null, blockParameter);
        }
    }
}
