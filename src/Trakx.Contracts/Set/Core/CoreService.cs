using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.ContractHandlers;
using System.Threading;
using Nethereum.Web3;
using Trakx.Contracts.Set.Core.ContractDefinition;

namespace Trakx.Contracts.Set.Core
{
    public partial class CoreService : ICoreService
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(IWeb3 web3, CoreDeployment coreDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<CoreDeployment>().SendRequestAndWaitForReceiptAsync(coreDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(IWeb3 web3, CoreDeployment coreDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<CoreDeployment>().SendRequestAsync(coreDeployment);
        }

        public static async Task<CoreService> DeployContractAndGetServiceAsync(IWeb3 web3, CoreDeployment coreDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, coreDeployment, cancellationTokenSource);
            return new CoreService(web3, receipt.ContractAddress);
        }

        protected IWeb3 Web3{ get; }

        public ContractHandler ContractHandler { get; }

        public CoreService(IWeb3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }

        public Task<string> WithdrawModuleRequestAsync(WithdrawModuleFunction withdrawModuleFunction)
        {
             return ContractHandler.SendRequestAsync(withdrawModuleFunction);
        }

        public Task<TransactionReceipt> WithdrawModuleRequestAndWaitForReceiptAsync(WithdrawModuleFunction withdrawModuleFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(withdrawModuleFunction, cancellationToken);
        }

        public Task<string> WithdrawModuleRequestAsync(string from, string to, string token, BigInteger quantity)
        {
            var withdrawModuleFunction = new WithdrawModuleFunction();
                withdrawModuleFunction.From = from;
                withdrawModuleFunction.To = to;
                withdrawModuleFunction.Token = token;
                withdrawModuleFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAsync(withdrawModuleFunction);
        }

        public Task<TransactionReceipt> WithdrawModuleRequestAndWaitForReceiptAsync(string from, string to, string token, BigInteger quantity, CancellationTokenSource cancellationToken = null)
        {
            var withdrawModuleFunction = new WithdrawModuleFunction();
                withdrawModuleFunction.From = from;
                withdrawModuleFunction.To = to;
                withdrawModuleFunction.Token = token;
                withdrawModuleFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(withdrawModuleFunction, cancellationToken);
        }

        public Task<bool> ValidFactoriesQueryAsync(ValidFactoriesFunction validFactoriesFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<ValidFactoriesFunction, bool>(validFactoriesFunction, blockParameter);
        }
        
        public Task<bool> ValidFactoriesQueryAsync(string factory, BlockParameter blockParameter = null)
        {
            var validFactoriesFunction = new ValidFactoriesFunction();
                validFactoriesFunction.Factory = factory;
            
            return ContractHandler.QueryAsync<ValidFactoriesFunction, bool>(validFactoriesFunction, blockParameter);
        }

        public Task<string> RemoveExchangeRequestAsync(RemoveExchangeFunction removeExchangeFunction)
        {
             return ContractHandler.SendRequestAsync(removeExchangeFunction);
        }

        public Task<TransactionReceipt> RemoveExchangeRequestAndWaitForReceiptAsync(RemoveExchangeFunction removeExchangeFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(removeExchangeFunction, cancellationToken);
        }

        public Task<string> RemoveExchangeRequestAsync(byte exchangeId, string exchange)
        {
            var removeExchangeFunction = new RemoveExchangeFunction();
                removeExchangeFunction.ExchangeId = exchangeId;
                removeExchangeFunction.Exchange = exchange;
            
             return ContractHandler.SendRequestAsync(removeExchangeFunction);
        }

        public Task<TransactionReceipt> RemoveExchangeRequestAndWaitForReceiptAsync(byte exchangeId, string exchange, CancellationTokenSource cancellationToken = null)
        {
            var removeExchangeFunction = new RemoveExchangeFunction();
                removeExchangeFunction.ExchangeId = exchangeId;
                removeExchangeFunction.Exchange = exchange;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(removeExchangeFunction, cancellationToken);
        }

        public Task<BigInteger> TimeLockedUpgradesQueryAsync(TimeLockedUpgradesFunction timeLockedUpgradesFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<TimeLockedUpgradesFunction, BigInteger>(timeLockedUpgradesFunction, blockParameter);
        }

        
        public Task<BigInteger> TimeLockedUpgradesQueryAsync(byte[] returnValue1, BlockParameter blockParameter = null)
        {
            var timeLockedUpgradesFunction = new TimeLockedUpgradesFunction();
                timeLockedUpgradesFunction.ReturnValue1 = returnValue1;
            
            return ContractHandler.QueryAsync<TimeLockedUpgradesFunction, BigInteger>(timeLockedUpgradesFunction, blockParameter);
        }

        public Task<string> ReenableSetRequestAsync(ReenableSetFunction reenableSetFunction)
        {
             return ContractHandler.SendRequestAsync(reenableSetFunction);
        }

        public Task<TransactionReceipt> ReenableSetRequestAndWaitForReceiptAsync(ReenableSetFunction reenableSetFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(reenableSetFunction, cancellationToken);
        }

        public Task<string> ReenableSetRequestAsync(string set)
        {
            var reenableSetFunction = new ReenableSetFunction();
                reenableSetFunction.Set = set;
            
             return ContractHandler.SendRequestAsync(reenableSetFunction);
        }

        public Task<TransactionReceipt> ReenableSetRequestAndWaitForReceiptAsync(string set, CancellationTokenSource cancellationToken = null)
        {
            var reenableSetFunction = new ReenableSetFunction();
                reenableSetFunction.Set = set;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(reenableSetFunction, cancellationToken);
        }

        public Task<string> RedeemAndWithdrawToRequestAsync(RedeemAndWithdrawToFunction redeemAndWithdrawToFunction)
        {
             return ContractHandler.SendRequestAsync(redeemAndWithdrawToFunction);
        }

        public Task<TransactionReceipt> RedeemAndWithdrawToRequestAndWaitForReceiptAsync(RedeemAndWithdrawToFunction redeemAndWithdrawToFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(redeemAndWithdrawToFunction, cancellationToken);
        }

        public Task<string> RedeemAndWithdrawToRequestAsync(string set, string to, BigInteger quantity, BigInteger toExclude)
        {
            var redeemAndWithdrawToFunction = new RedeemAndWithdrawToFunction();
                redeemAndWithdrawToFunction.Set = set;
                redeemAndWithdrawToFunction.To = to;
                redeemAndWithdrawToFunction.Quantity = quantity;
                redeemAndWithdrawToFunction.ToExclude = toExclude;
            
             return ContractHandler.SendRequestAsync(redeemAndWithdrawToFunction);
        }

        public Task<TransactionReceipt> RedeemAndWithdrawToRequestAndWaitForReceiptAsync(string set, string to, BigInteger quantity, BigInteger toExclude, CancellationTokenSource cancellationToken = null)
        {
            var redeemAndWithdrawToFunction = new RedeemAndWithdrawToFunction();
                redeemAndWithdrawToFunction.Set = set;
                redeemAndWithdrawToFunction.To = to;
                redeemAndWithdrawToFunction.Quantity = quantity;
                redeemAndWithdrawToFunction.ToExclude = toExclude;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(redeemAndWithdrawToFunction, cancellationToken);
        }

        public Task<string> RedeemRequestAsync(RedeemFunction redeemFunction)
        {
             return ContractHandler.SendRequestAsync(redeemFunction);
        }

        public Task<TransactionReceipt> RedeemRequestAndWaitForReceiptAsync(RedeemFunction redeemFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(redeemFunction, cancellationToken);
        }

        public Task<string> RedeemRequestAsync(string set, BigInteger quantity)
        {
            var redeemFunction = new RedeemFunction();
                redeemFunction.Set = set;
                redeemFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAsync(redeemFunction);
        }

        public Task<TransactionReceipt> RedeemRequestAndWaitForReceiptAsync(string set, BigInteger quantity, CancellationTokenSource cancellationToken = null)
        {
            var redeemFunction = new RedeemFunction();
                redeemFunction.Set = set;
                redeemFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(redeemFunction, cancellationToken);
        }

        public Task<string> AddModuleRequestAsync(AddModuleFunction addModuleFunction)
        {
             return ContractHandler.SendRequestAsync(addModuleFunction);
        }

        public Task<TransactionReceipt> AddModuleRequestAndWaitForReceiptAsync(AddModuleFunction addModuleFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(addModuleFunction, cancellationToken);
        }

        public Task<string> AddModuleRequestAsync(string module)
        {
            var addModuleFunction = new AddModuleFunction();
                addModuleFunction.Module = module;
            
             return ContractHandler.SendRequestAsync(addModuleFunction);
        }

        public Task<TransactionReceipt> AddModuleRequestAndWaitForReceiptAsync(string module, CancellationTokenSource cancellationToken = null)
        {
            var addModuleFunction = new AddModuleFunction();
                addModuleFunction.Module = module;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(addModuleFunction, cancellationToken);
        }

        public Task<List<string>> ExchangesQueryAsync(ExchangesFunction exchangesFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<ExchangesFunction, List<string>>(exchangesFunction, blockParameter);
        }

        
        public Task<List<string>> ExchangesQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<ExchangesFunction, List<string>>(null, blockParameter);
        }

        public Task<string> AddFactoryRequestAsync(AddFactoryFunction addFactoryFunction)
        {
             return ContractHandler.SendRequestAsync(addFactoryFunction);
        }

        public Task<TransactionReceipt> AddFactoryRequestAndWaitForReceiptAsync(AddFactoryFunction addFactoryFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(addFactoryFunction, cancellationToken);
        }

        public Task<string> AddFactoryRequestAsync(string factory)
        {
            var addFactoryFunction = new AddFactoryFunction();
                addFactoryFunction.Factory = factory;
            
             return ContractHandler.SendRequestAsync(addFactoryFunction);
        }

        public Task<TransactionReceipt> AddFactoryRequestAndWaitForReceiptAsync(string factory, CancellationTokenSource cancellationToken = null)
        {
            var addFactoryFunction = new AddFactoryFunction();
                addFactoryFunction.Factory = factory;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(addFactoryFunction, cancellationToken);
        }

        public Task<string> BatchDepositRequestAsync(BatchDepositFunction batchDepositFunction)
        {
             return ContractHandler.SendRequestAsync(batchDepositFunction);
        }

        public Task<TransactionReceipt> BatchDepositRequestAndWaitForReceiptAsync(BatchDepositFunction batchDepositFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(batchDepositFunction, cancellationToken);
        }

        public Task<string> BatchDepositRequestAsync(List<string> tokens, List<BigInteger> quantities)
        {
            var batchDepositFunction = new BatchDepositFunction();
                batchDepositFunction.Tokens = tokens;
                batchDepositFunction.Quantities = quantities;
            
             return ContractHandler.SendRequestAsync(batchDepositFunction);
        }

        public Task<TransactionReceipt> BatchDepositRequestAndWaitForReceiptAsync(List<string> tokens, List<BigInteger> quantities, CancellationTokenSource cancellationToken = null)
        {
            var batchDepositFunction = new BatchDepositFunction();
                batchDepositFunction.Tokens = tokens;
                batchDepositFunction.Quantities = quantities;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(batchDepositFunction, cancellationToken);
        }

        public Task<string> BatchIncrementTokenOwnerModuleRequestAsync(BatchIncrementTokenOwnerModuleFunction batchIncrementTokenOwnerModuleFunction)
        {
             return ContractHandler.SendRequestAsync(batchIncrementTokenOwnerModuleFunction);
        }

        public Task<TransactionReceipt> BatchIncrementTokenOwnerModuleRequestAndWaitForReceiptAsync(BatchIncrementTokenOwnerModuleFunction batchIncrementTokenOwnerModuleFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(batchIncrementTokenOwnerModuleFunction, cancellationToken);
        }

        public Task<string> BatchIncrementTokenOwnerModuleRequestAsync(List<string> tokens, string owner, List<BigInteger> quantities)
        {
            var batchIncrementTokenOwnerModuleFunction = new BatchIncrementTokenOwnerModuleFunction();
                batchIncrementTokenOwnerModuleFunction.Tokens = tokens;
                batchIncrementTokenOwnerModuleFunction.Owner = owner;
                batchIncrementTokenOwnerModuleFunction.Quantities = quantities;
            
             return ContractHandler.SendRequestAsync(batchIncrementTokenOwnerModuleFunction);
        }

        public Task<TransactionReceipt> BatchIncrementTokenOwnerModuleRequestAndWaitForReceiptAsync(List<string> tokens, string owner, List<BigInteger> quantities, CancellationTokenSource cancellationToken = null)
        {
            var batchIncrementTokenOwnerModuleFunction = new BatchIncrementTokenOwnerModuleFunction();
                batchIncrementTokenOwnerModuleFunction.Tokens = tokens;
                batchIncrementTokenOwnerModuleFunction.Owner = owner;
                batchIncrementTokenOwnerModuleFunction.Quantities = quantities;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(batchIncrementTokenOwnerModuleFunction, cancellationToken);
        }

        public Task<string> RemovePriceLibraryRequestAsync(RemovePriceLibraryFunction removePriceLibraryFunction)
        {
             return ContractHandler.SendRequestAsync(removePriceLibraryFunction);
        }

        public Task<TransactionReceipt> RemovePriceLibraryRequestAndWaitForReceiptAsync(RemovePriceLibraryFunction removePriceLibraryFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(removePriceLibraryFunction, cancellationToken);
        }

        public Task<string> RemovePriceLibraryRequestAsync(string priceLibrary)
        {
            var removePriceLibraryFunction = new RemovePriceLibraryFunction();
                removePriceLibraryFunction.PriceLibrary = priceLibrary;
            
             return ContractHandler.SendRequestAsync(removePriceLibraryFunction);
        }

        public Task<TransactionReceipt> RemovePriceLibraryRequestAndWaitForReceiptAsync(string priceLibrary, CancellationTokenSource cancellationToken = null)
        {
            var removePriceLibraryFunction = new RemovePriceLibraryFunction();
                removePriceLibraryFunction.PriceLibrary = priceLibrary;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(removePriceLibraryFunction, cancellationToken);
        }

        public Task<string> DepositRequestAsync(DepositFunction depositFunction)
        {
             return ContractHandler.SendRequestAsync(depositFunction);
        }

        public Task<TransactionReceipt> DepositRequestAndWaitForReceiptAsync(DepositFunction depositFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(depositFunction, cancellationToken);
        }

        public Task<string> DepositRequestAsync(string token, BigInteger quantity)
        {
            var depositFunction = new DepositFunction();
                depositFunction.Token = token;
                depositFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAsync(depositFunction);
        }

        public Task<TransactionReceipt> DepositRequestAndWaitForReceiptAsync(string token, BigInteger quantity, CancellationTokenSource cancellationToken = null)
        {
            var depositFunction = new DepositFunction();
                depositFunction.Token = token;
                depositFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(depositFunction, cancellationToken);
        }

        public Task<string> RemoveFactoryRequestAsync(RemoveFactoryFunction removeFactoryFunction)
        {
             return ContractHandler.SendRequestAsync(removeFactoryFunction);
        }

        public Task<TransactionReceipt> RemoveFactoryRequestAndWaitForReceiptAsync(RemoveFactoryFunction removeFactoryFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(removeFactoryFunction, cancellationToken);
        }

        public Task<string> RemoveFactoryRequestAsync(string factory)
        {
            var removeFactoryFunction = new RemoveFactoryFunction();
                removeFactoryFunction.Factory = factory;
            
             return ContractHandler.SendRequestAsync(removeFactoryFunction);
        }

        public Task<TransactionReceipt> RemoveFactoryRequestAndWaitForReceiptAsync(string factory, CancellationTokenSource cancellationToken = null)
        {
            var removeFactoryFunction = new RemoveFactoryFunction();
                removeFactoryFunction.Factory = factory;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(removeFactoryFunction, cancellationToken);
        }

        public Task<string> AddExchangeRequestAsync(AddExchangeFunction addExchangeFunction)
        {
             return ContractHandler.SendRequestAsync(addExchangeFunction);
        }

        public Task<TransactionReceipt> AddExchangeRequestAndWaitForReceiptAsync(AddExchangeFunction addExchangeFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(addExchangeFunction, cancellationToken);
        }

        public Task<string> AddExchangeRequestAsync(byte exchangeId, string exchange)
        {
            var addExchangeFunction = new AddExchangeFunction();
                addExchangeFunction.ExchangeId = exchangeId;
                addExchangeFunction.Exchange = exchange;
            
             return ContractHandler.SendRequestAsync(addExchangeFunction);
        }

        public Task<TransactionReceipt> AddExchangeRequestAndWaitForReceiptAsync(byte exchangeId, string exchange, CancellationTokenSource cancellationToken = null)
        {
            var addExchangeFunction = new AddExchangeFunction();
                addExchangeFunction.ExchangeId = exchangeId;
                addExchangeFunction.Exchange = exchange;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(addExchangeFunction, cancellationToken);
        }

        public Task<string> BatchTransferModuleRequestAsync(BatchTransferModuleFunction batchTransferModuleFunction)
        {
             return ContractHandler.SendRequestAsync(batchTransferModuleFunction);
        }

        public Task<TransactionReceipt> BatchTransferModuleRequestAndWaitForReceiptAsync(BatchTransferModuleFunction batchTransferModuleFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(batchTransferModuleFunction, cancellationToken);
        }

        public Task<string> BatchTransferModuleRequestAsync(List<string> tokens, List<BigInteger> quantities, string from, string to)
        {
            var batchTransferModuleFunction = new BatchTransferModuleFunction();
                batchTransferModuleFunction.Tokens = tokens;
                batchTransferModuleFunction.Quantities = quantities;
                batchTransferModuleFunction.From = from;
                batchTransferModuleFunction.To = to;
            
             return ContractHandler.SendRequestAsync(batchTransferModuleFunction);
        }

        public Task<TransactionReceipt> BatchTransferModuleRequestAndWaitForReceiptAsync(List<string> tokens, List<BigInteger> quantities, string from, string to, CancellationTokenSource cancellationToken = null)
        {
            var batchTransferModuleFunction = new BatchTransferModuleFunction();
                batchTransferModuleFunction.Tokens = tokens;
                batchTransferModuleFunction.Quantities = quantities;
                batchTransferModuleFunction.From = from;
                batchTransferModuleFunction.To = to;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(batchTransferModuleFunction, cancellationToken);
        }

        public Task<string> SetOperationStateRequestAsync(SetOperationStateFunction setOperationStateFunction)
        {
             return ContractHandler.SendRequestAsync(setOperationStateFunction);
        }

        public Task<TransactionReceipt> SetOperationStateRequestAndWaitForReceiptAsync(SetOperationStateFunction setOperationStateFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setOperationStateFunction, cancellationToken);
        }

        public Task<string> SetOperationStateRequestAsync(byte operationState)
        {
            var setOperationStateFunction = new SetOperationStateFunction();
                setOperationStateFunction.OperationState = operationState;
            
             return ContractHandler.SendRequestAsync(setOperationStateFunction);
        }

        public Task<TransactionReceipt> SetOperationStateRequestAndWaitForReceiptAsync(byte operationState, CancellationTokenSource cancellationToken = null)
        {
            var setOperationStateFunction = new SetOperationStateFunction();
                setOperationStateFunction.OperationState = operationState;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setOperationStateFunction, cancellationToken);
        }

        public Task<List<string>> SetTokensQueryAsync(SetTokensFunction setTokensFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<SetTokensFunction, List<string>>(setTokensFunction, blockParameter);
        }

        
        public Task<List<string>> SetTokensQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<SetTokensFunction, List<string>>(null, blockParameter);
        }

        public Task<string> BatchTransferBalanceModuleRequestAsync(BatchTransferBalanceModuleFunction batchTransferBalanceModuleFunction)
        {
             return ContractHandler.SendRequestAsync(batchTransferBalanceModuleFunction);
        }

        public Task<TransactionReceipt> BatchTransferBalanceModuleRequestAndWaitForReceiptAsync(BatchTransferBalanceModuleFunction batchTransferBalanceModuleFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(batchTransferBalanceModuleFunction, cancellationToken);
        }

        public Task<string> BatchTransferBalanceModuleRequestAsync(List<string> tokens, string from, string to, List<BigInteger> quantities)
        {
            var batchTransferBalanceModuleFunction = new BatchTransferBalanceModuleFunction();
                batchTransferBalanceModuleFunction.Tokens = tokens;
                batchTransferBalanceModuleFunction.From = from;
                batchTransferBalanceModuleFunction.To = to;
                batchTransferBalanceModuleFunction.Quantities = quantities;
            
             return ContractHandler.SendRequestAsync(batchTransferBalanceModuleFunction);
        }

        public Task<TransactionReceipt> BatchTransferBalanceModuleRequestAndWaitForReceiptAsync(List<string> tokens, string from, string to, List<BigInteger> quantities, CancellationTokenSource cancellationToken = null)
        {
            var batchTransferBalanceModuleFunction = new BatchTransferBalanceModuleFunction();
                batchTransferBalanceModuleFunction.Tokens = tokens;
                batchTransferBalanceModuleFunction.From = from;
                batchTransferBalanceModuleFunction.To = to;
                batchTransferBalanceModuleFunction.Quantities = quantities;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(batchTransferBalanceModuleFunction, cancellationToken);
        }

        public Task<string> ExchangeIdsQueryAsync(ExchangeIdsFunction exchangeIdsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<ExchangeIdsFunction, string>(exchangeIdsFunction, blockParameter);
        }

        
        public Task<string> ExchangeIdsQueryAsync(byte exchangeId, BlockParameter blockParameter = null)
        {
            var exchangeIdsFunction = new ExchangeIdsFunction();
                exchangeIdsFunction.ExchangeId = exchangeId;
            
            return ContractHandler.QueryAsync<ExchangeIdsFunction, string>(exchangeIdsFunction, blockParameter);
        }

        public Task<string> RedeemModuleRequestAsync(RedeemModuleFunction redeemModuleFunction)
        {
             return ContractHandler.SendRequestAsync(redeemModuleFunction);
        }

        public Task<TransactionReceipt> RedeemModuleRequestAndWaitForReceiptAsync(RedeemModuleFunction redeemModuleFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(redeemModuleFunction, cancellationToken);
        }

        public Task<string> RedeemModuleRequestAsync(string burnAddress, string incrementAddress, string set, BigInteger quantity)
        {
            var redeemModuleFunction = new RedeemModuleFunction();
                redeemModuleFunction.BurnAddress = burnAddress;
                redeemModuleFunction.IncrementAddress = incrementAddress;
                redeemModuleFunction.Set = set;
                redeemModuleFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAsync(redeemModuleFunction);
        }

        public Task<TransactionReceipt> RedeemModuleRequestAndWaitForReceiptAsync(string burnAddress, string incrementAddress, string set, BigInteger quantity, CancellationTokenSource cancellationToken = null)
        {
            var redeemModuleFunction = new RedeemModuleFunction();
                redeemModuleFunction.BurnAddress = burnAddress;
                redeemModuleFunction.IncrementAddress = incrementAddress;
                redeemModuleFunction.Set = set;
                redeemModuleFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(redeemModuleFunction, cancellationToken);
        }

        public Task<string> InternalTransferRequestAsync(InternalTransferFunction internalTransferFunction)
        {
             return ContractHandler.SendRequestAsync(internalTransferFunction);
        }

        public Task<TransactionReceipt> InternalTransferRequestAndWaitForReceiptAsync(InternalTransferFunction internalTransferFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(internalTransferFunction, cancellationToken);
        }

        public Task<string> InternalTransferRequestAsync(string token, string to, BigInteger quantity)
        {
            var internalTransferFunction = new InternalTransferFunction();
                internalTransferFunction.Token = token;
                internalTransferFunction.To = to;
                internalTransferFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAsync(internalTransferFunction);
        }

        public Task<TransactionReceipt> InternalTransferRequestAndWaitForReceiptAsync(string token, string to, BigInteger quantity, CancellationTokenSource cancellationToken = null)
        {
            var internalTransferFunction = new InternalTransferFunction();
                internalTransferFunction.Token = token;
                internalTransferFunction.To = to;
                internalTransferFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(internalTransferFunction, cancellationToken);
        }

        public Task<string> CreateSetRequestAsync(CreateSetFunction createSetFunction)
        {
             return ContractHandler.SendRequestAsync(createSetFunction);
        }

        public Task<TransactionReceipt> CreateSetRequestAndWaitForReceiptAsync(CreateSetFunction createSetFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(createSetFunction, cancellationToken);
        }

        public Task<string> CreateSetRequestAsync(string factory, List<string> components, List<BigInteger> units, BigInteger naturalUnit, byte[] name, byte[] symbol, byte[] callData)
        {
            var createSetFunction = new CreateSetFunction();
                createSetFunction.Factory = factory;
                createSetFunction.Components = components;
                createSetFunction.Units = units;
                createSetFunction.NaturalUnit = naturalUnit;
                createSetFunction.Name = name;
                createSetFunction.Symbol = symbol;
                createSetFunction.CallData = callData;
            
             return ContractHandler.SendRequestAsync(createSetFunction);
        }

        public Task<TransactionReceipt> CreateSetRequestAndWaitForReceiptAsync(string factory, List<string> components, List<BigInteger> units, BigInteger naturalUnit, byte[] name, byte[] symbol, byte[] callData, CancellationTokenSource cancellationToken = null)
        {
            var createSetFunction = new CreateSetFunction();
                createSetFunction.Factory = factory;
                createSetFunction.Components = components;
                createSetFunction.Units = units;
                createSetFunction.NaturalUnit = naturalUnit;
                createSetFunction.Name = name;
                createSetFunction.Symbol = symbol;
                createSetFunction.CallData = callData;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(createSetFunction, cancellationToken);
        }

        public Task<bool> ValidModulesQueryAsync(ValidModulesFunction validModulesFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<ValidModulesFunction, bool>(validModulesFunction, blockParameter);
        }

        
        public Task<bool> ValidModulesQueryAsync(string module, BlockParameter blockParameter = null)
        {
            var validModulesFunction = new ValidModulesFunction();
                validModulesFunction.Module = module;
            
            return ContractHandler.QueryAsync<ValidModulesFunction, bool>(validModulesFunction, blockParameter);
        }

        public Task<string> TransferProxyQueryAsync(TransferProxyFunction transferProxyFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<TransferProxyFunction, string>(transferProxyFunction, blockParameter);
        }

        
        public Task<string> TransferProxyQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<TransferProxyFunction, string>(null, blockParameter);
        }

        public Task<string> RenounceOwnershipRequestAsync(RenounceOwnershipFunction renounceOwnershipFunction)
        {
             return ContractHandler.SendRequestAsync(renounceOwnershipFunction);
        }

        public Task<string> RenounceOwnershipRequestAsync()
        {
             return ContractHandler.SendRequestAsync<RenounceOwnershipFunction>();
        }

        public Task<TransactionReceipt> RenounceOwnershipRequestAndWaitForReceiptAsync(RenounceOwnershipFunction renounceOwnershipFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(renounceOwnershipFunction, cancellationToken);
        }

        public Task<TransactionReceipt> RenounceOwnershipRequestAndWaitForReceiptAsync(CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync<RenounceOwnershipFunction>(null, cancellationToken);
        }

        public Task<byte> OperationStateQueryAsync(OperationStateFunction operationStateFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OperationStateFunction, byte>(operationStateFunction, blockParameter);
        }

        
        public Task<byte> OperationStateQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OperationStateFunction, byte>(null, blockParameter);
        }

        public Task<string> IssueToRequestAsync(IssueToFunction issueToFunction)
        {
             return ContractHandler.SendRequestAsync(issueToFunction);
        }

        public Task<TransactionReceipt> IssueToRequestAndWaitForReceiptAsync(IssueToFunction issueToFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(issueToFunction, cancellationToken);
        }

        public Task<string> IssueToRequestAsync(string recipient, string set, BigInteger quantity)
        {
            var issueToFunction = new IssueToFunction();
                issueToFunction.Recipient = recipient;
                issueToFunction.Set = set;
                issueToFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAsync(issueToFunction);
        }

        public Task<TransactionReceipt> IssueToRequestAndWaitForReceiptAsync(string recipient, string set, BigInteger quantity, CancellationTokenSource cancellationToken = null)
        {
            var issueToFunction = new IssueToFunction();
                issueToFunction.Recipient = recipient;
                issueToFunction.Set = set;
                issueToFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(issueToFunction, cancellationToken);
        }

        public Task<string> DisableSetRequestAsync(DisableSetFunction disableSetFunction)
        {
             return ContractHandler.SendRequestAsync(disableSetFunction);
        }

        public Task<TransactionReceipt> DisableSetRequestAndWaitForReceiptAsync(DisableSetFunction disableSetFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(disableSetFunction, cancellationToken);
        }

        public Task<string> DisableSetRequestAsync(string set)
        {
            var disableSetFunction = new DisableSetFunction();
                disableSetFunction.Set = set;
            
             return ContractHandler.SendRequestAsync(disableSetFunction);
        }

        public Task<TransactionReceipt> DisableSetRequestAndWaitForReceiptAsync(string set, CancellationTokenSource cancellationToken = null)
        {
            var disableSetFunction = new DisableSetFunction();
                disableSetFunction.Set = set;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(disableSetFunction, cancellationToken);
        }

        public Task<BigInteger> TimeLockPeriodQueryAsync(TimeLockPeriodFunction timeLockPeriodFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<TimeLockPeriodFunction, BigInteger>(timeLockPeriodFunction, blockParameter);
        }

        
        public Task<BigInteger> TimeLockPeriodQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<TimeLockPeriodFunction, BigInteger>(null, blockParameter);
        }

        public Task<string> IssueInVaultModuleRequestAsync(IssueInVaultModuleFunction issueInVaultModuleFunction)
        {
             return ContractHandler.SendRequestAsync(issueInVaultModuleFunction);
        }

        public Task<TransactionReceipt> IssueInVaultModuleRequestAndWaitForReceiptAsync(IssueInVaultModuleFunction issueInVaultModuleFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(issueInVaultModuleFunction, cancellationToken);
        }

        public Task<string> IssueInVaultModuleRequestAsync(string recipient, string set, BigInteger quantity)
        {
            var issueInVaultModuleFunction = new IssueInVaultModuleFunction();
                issueInVaultModuleFunction.Recipient = recipient;
                issueInVaultModuleFunction.Set = set;
                issueInVaultModuleFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAsync(issueInVaultModuleFunction);
        }

        public Task<TransactionReceipt> IssueInVaultModuleRequestAndWaitForReceiptAsync(string recipient, string set, BigInteger quantity, CancellationTokenSource cancellationToken = null)
        {
            var issueInVaultModuleFunction = new IssueInVaultModuleFunction();
                issueInVaultModuleFunction.Recipient = recipient;
                issueInVaultModuleFunction.Set = set;
                issueInVaultModuleFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(issueInVaultModuleFunction, cancellationToken);
        }

        public Task<string> IssueModuleRequestAsync(IssueModuleFunction issueModuleFunction)
        {
             return ContractHandler.SendRequestAsync(issueModuleFunction);
        }

        public Task<TransactionReceipt> IssueModuleRequestAndWaitForReceiptAsync(IssueModuleFunction issueModuleFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(issueModuleFunction, cancellationToken);
        }

        public Task<string> IssueModuleRequestAsync(string componentOwner, string setRecipient, string set, BigInteger quantity)
        {
            var issueModuleFunction = new IssueModuleFunction();
                issueModuleFunction.ComponentOwner = componentOwner;
                issueModuleFunction.SetRecipient = setRecipient;
                issueModuleFunction.Set = set;
                issueModuleFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAsync(issueModuleFunction);
        }

        public Task<TransactionReceipt> IssueModuleRequestAndWaitForReceiptAsync(string componentOwner, string setRecipient, string set, BigInteger quantity, CancellationTokenSource cancellationToken = null)
        {
            var issueModuleFunction = new IssueModuleFunction();
                issueModuleFunction.ComponentOwner = componentOwner;
                issueModuleFunction.SetRecipient = setRecipient;
                issueModuleFunction.Set = set;
                issueModuleFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(issueModuleFunction, cancellationToken);
        }

        public Task<string> IssueRequestAsync(IssueFunction issueFunction)
        {
             return ContractHandler.SendRequestAsync(issueFunction);
        }

        public Task<TransactionReceipt> IssueRequestAndWaitForReceiptAsync(IssueFunction issueFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(issueFunction, cancellationToken);
        }

        public Task<string> IssueRequestAsync(string set, BigInteger quantity)
        {
            var issueFunction = new IssueFunction();
                issueFunction.Set = set;
                issueFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAsync(issueFunction);
        }

        public Task<TransactionReceipt> IssueRequestAndWaitForReceiptAsync(string set, BigInteger quantity, CancellationTokenSource cancellationToken = null)
        {
            var issueFunction = new IssueFunction();
                issueFunction.Set = set;
                issueFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(issueFunction, cancellationToken);
        }

        public Task<string> OwnerQueryAsync(OwnerFunction ownerFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OwnerFunction, string>(ownerFunction, blockParameter);
        }

        
        public Task<string> OwnerQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OwnerFunction, string>(null, blockParameter);
        }

        public Task<bool> IsOwnerQueryAsync(IsOwnerFunction isOwnerFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<IsOwnerFunction, bool>(isOwnerFunction, blockParameter);
        }

        
        public Task<bool> IsOwnerQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<IsOwnerFunction, bool>(null, blockParameter);
        }

        public Task<string> SetTimeLockPeriodRequestAsync(SetTimeLockPeriodFunction setTimeLockPeriodFunction)
        {
             return ContractHandler.SendRequestAsync(setTimeLockPeriodFunction);
        }

        public Task<TransactionReceipt> SetTimeLockPeriodRequestAndWaitForReceiptAsync(SetTimeLockPeriodFunction setTimeLockPeriodFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setTimeLockPeriodFunction, cancellationToken);
        }

        public Task<string> SetTimeLockPeriodRequestAsync(BigInteger timeLockPeriod)
        {
            var setTimeLockPeriodFunction = new SetTimeLockPeriodFunction();
                setTimeLockPeriodFunction.TimeLockPeriod = timeLockPeriod;
            
             return ContractHandler.SendRequestAsync(setTimeLockPeriodFunction);
        }

        public Task<TransactionReceipt> SetTimeLockPeriodRequestAndWaitForReceiptAsync(BigInteger timeLockPeriod, CancellationTokenSource cancellationToken = null)
        {
            var setTimeLockPeriodFunction = new SetTimeLockPeriodFunction();
                setTimeLockPeriodFunction.TimeLockPeriod = timeLockPeriod;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setTimeLockPeriodFunction, cancellationToken);
        }

        public Task<string> RemoveModuleRequestAsync(RemoveModuleFunction removeModuleFunction)
        {
             return ContractHandler.SendRequestAsync(removeModuleFunction);
        }

        public Task<TransactionReceipt> RemoveModuleRequestAndWaitForReceiptAsync(RemoveModuleFunction removeModuleFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(removeModuleFunction, cancellationToken);
        }

        public Task<string> RemoveModuleRequestAsync(string module)
        {
            var removeModuleFunction = new RemoveModuleFunction();
                removeModuleFunction.Module = module;
            
             return ContractHandler.SendRequestAsync(removeModuleFunction);
        }

        public Task<TransactionReceipt> RemoveModuleRequestAndWaitForReceiptAsync(string module, CancellationTokenSource cancellationToken = null)
        {
            var removeModuleFunction = new RemoveModuleFunction();
                removeModuleFunction.Module = module;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(removeModuleFunction, cancellationToken);
        }

        public Task<bool> ValidPriceLibrariesQueryAsync(ValidPriceLibrariesFunction validPriceLibrariesFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<ValidPriceLibrariesFunction, bool>(validPriceLibrariesFunction, blockParameter);
        }

        
        public Task<bool> ValidPriceLibrariesQueryAsync(string priceLibrary, BlockParameter blockParameter = null)
        {
            var validPriceLibrariesFunction = new ValidPriceLibrariesFunction();
                validPriceLibrariesFunction.PriceLibrary = priceLibrary;
            
            return ContractHandler.QueryAsync<ValidPriceLibrariesFunction, bool>(validPriceLibrariesFunction, blockParameter);
        }

        public Task<string> IssueInVaultRequestAsync(IssueInVaultFunction issueInVaultFunction)
        {
             return ContractHandler.SendRequestAsync(issueInVaultFunction);
        }

        public Task<TransactionReceipt> IssueInVaultRequestAndWaitForReceiptAsync(IssueInVaultFunction issueInVaultFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(issueInVaultFunction, cancellationToken);
        }

        public Task<string> IssueInVaultRequestAsync(string set, BigInteger quantity)
        {
            var issueInVaultFunction = new IssueInVaultFunction();
                issueInVaultFunction.Set = set;
                issueInVaultFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAsync(issueInVaultFunction);
        }

        public Task<TransactionReceipt> IssueInVaultRequestAndWaitForReceiptAsync(string set, BigInteger quantity, CancellationTokenSource cancellationToken = null)
        {
            var issueInVaultFunction = new IssueInVaultFunction();
                issueInVaultFunction.Set = set;
                issueInVaultFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(issueInVaultFunction, cancellationToken);
        }

        public Task<string> RedeemInVaultRequestAsync(RedeemInVaultFunction redeemInVaultFunction)
        {
             return ContractHandler.SendRequestAsync(redeemInVaultFunction);
        }

        public Task<TransactionReceipt> RedeemInVaultRequestAndWaitForReceiptAsync(RedeemInVaultFunction redeemInVaultFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(redeemInVaultFunction, cancellationToken);
        }

        public Task<string> RedeemInVaultRequestAsync(string set, BigInteger quantity)
        {
            var redeemInVaultFunction = new RedeemInVaultFunction();
                redeemInVaultFunction.Set = set;
                redeemInVaultFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAsync(redeemInVaultFunction);
        }

        public Task<TransactionReceipt> RedeemInVaultRequestAndWaitForReceiptAsync(string set, BigInteger quantity, CancellationTokenSource cancellationToken = null)
        {
            var redeemInVaultFunction = new RedeemInVaultFunction();
                redeemInVaultFunction.Set = set;
                redeemInVaultFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(redeemInVaultFunction, cancellationToken);
        }

        public Task<StateOutputDTO> StateQueryAsync(StateFunction stateFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<StateFunction, StateOutputDTO>(stateFunction, blockParameter);
        }

        public Task<StateOutputDTO> StateQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<StateFunction, StateOutputDTO>(null, blockParameter);
        }

        public Task<string> BatchDecrementTokenOwnerModuleRequestAsync(BatchDecrementTokenOwnerModuleFunction batchDecrementTokenOwnerModuleFunction)
        {
             return ContractHandler.SendRequestAsync(batchDecrementTokenOwnerModuleFunction);
        }

        public Task<TransactionReceipt> BatchDecrementTokenOwnerModuleRequestAndWaitForReceiptAsync(BatchDecrementTokenOwnerModuleFunction batchDecrementTokenOwnerModuleFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(batchDecrementTokenOwnerModuleFunction, cancellationToken);
        }

        public Task<string> BatchDecrementTokenOwnerModuleRequestAsync(List<string> tokens, string owner, List<BigInteger> quantities)
        {
            var batchDecrementTokenOwnerModuleFunction = new BatchDecrementTokenOwnerModuleFunction();
                batchDecrementTokenOwnerModuleFunction.Tokens = tokens;
                batchDecrementTokenOwnerModuleFunction.Owner = owner;
                batchDecrementTokenOwnerModuleFunction.Quantities = quantities;
            
             return ContractHandler.SendRequestAsync(batchDecrementTokenOwnerModuleFunction);
        }

        public Task<TransactionReceipt> BatchDecrementTokenOwnerModuleRequestAndWaitForReceiptAsync(List<string> tokens, string owner, List<BigInteger> quantities, CancellationTokenSource cancellationToken = null)
        {
            var batchDecrementTokenOwnerModuleFunction = new BatchDecrementTokenOwnerModuleFunction();
                batchDecrementTokenOwnerModuleFunction.Tokens = tokens;
                batchDecrementTokenOwnerModuleFunction.Owner = owner;
                batchDecrementTokenOwnerModuleFunction.Quantities = quantities;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(batchDecrementTokenOwnerModuleFunction, cancellationToken);
        }

        public Task<string> BatchDepositModuleRequestAsync(BatchDepositModuleFunction batchDepositModuleFunction)
        {
             return ContractHandler.SendRequestAsync(batchDepositModuleFunction);
        }

        public Task<TransactionReceipt> BatchDepositModuleRequestAndWaitForReceiptAsync(BatchDepositModuleFunction batchDepositModuleFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(batchDepositModuleFunction, cancellationToken);
        }

        public Task<string> BatchDepositModuleRequestAsync(string from, string to, List<string> tokens, List<BigInteger> quantities)
        {
            var batchDepositModuleFunction = new BatchDepositModuleFunction();
                batchDepositModuleFunction.From = from;
                batchDepositModuleFunction.To = to;
                batchDepositModuleFunction.Tokens = tokens;
                batchDepositModuleFunction.Quantities = quantities;
            
             return ContractHandler.SendRequestAsync(batchDepositModuleFunction);
        }

        public Task<TransactionReceipt> BatchDepositModuleRequestAndWaitForReceiptAsync(string from, string to, List<string> tokens, List<BigInteger> quantities, CancellationTokenSource cancellationToken = null)
        {
            var batchDepositModuleFunction = new BatchDepositModuleFunction();
                batchDepositModuleFunction.From = from;
                batchDepositModuleFunction.To = to;
                batchDepositModuleFunction.Tokens = tokens;
                batchDepositModuleFunction.Quantities = quantities;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(batchDepositModuleFunction, cancellationToken);
        }

        public Task<List<string>> PriceLibrariesQueryAsync(PriceLibrariesFunction priceLibrariesFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<PriceLibrariesFunction, List<string>>(priceLibrariesFunction, blockParameter);
        }

        
        public Task<List<string>> PriceLibrariesQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<PriceLibrariesFunction, List<string>>(null, blockParameter);
        }

        public Task<bool> DisabledSetsQueryAsync(DisabledSetsFunction disabledSetsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<DisabledSetsFunction, bool>(disabledSetsFunction, blockParameter);
        }

        
        public Task<bool> DisabledSetsQueryAsync(string set, BlockParameter blockParameter = null)
        {
            var disabledSetsFunction = new DisabledSetsFunction();
                disabledSetsFunction.Set = set;
            
            return ContractHandler.QueryAsync<DisabledSetsFunction, bool>(disabledSetsFunction, blockParameter);
        }

        public Task<string> DepositModuleRequestAsync(DepositModuleFunction depositModuleFunction)
        {
             return ContractHandler.SendRequestAsync(depositModuleFunction);
        }

        public Task<TransactionReceipt> DepositModuleRequestAndWaitForReceiptAsync(DepositModuleFunction depositModuleFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(depositModuleFunction, cancellationToken);
        }

        public Task<string> DepositModuleRequestAsync(string from, string to, string token, BigInteger quantity)
        {
            var depositModuleFunction = new DepositModuleFunction();
                depositModuleFunction.From = from;
                depositModuleFunction.To = to;
                depositModuleFunction.Token = token;
                depositModuleFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAsync(depositModuleFunction);
        }

        public Task<TransactionReceipt> DepositModuleRequestAndWaitForReceiptAsync(string from, string to, string token, BigInteger quantity, CancellationTokenSource cancellationToken = null)
        {
            var depositModuleFunction = new DepositModuleFunction();
                depositModuleFunction.From = from;
                depositModuleFunction.To = to;
                depositModuleFunction.Token = token;
                depositModuleFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(depositModuleFunction, cancellationToken);
        }

        public Task<string> BatchWithdrawRequestAsync(BatchWithdrawFunction batchWithdrawFunction)
        {
             return ContractHandler.SendRequestAsync(batchWithdrawFunction);
        }

        public Task<TransactionReceipt> BatchWithdrawRequestAndWaitForReceiptAsync(BatchWithdrawFunction batchWithdrawFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(batchWithdrawFunction, cancellationToken);
        }

        public Task<string> BatchWithdrawRequestAsync(List<string> tokens, List<BigInteger> quantities)
        {
            var batchWithdrawFunction = new BatchWithdrawFunction();
                batchWithdrawFunction.Tokens = tokens;
                batchWithdrawFunction.Quantities = quantities;
            
             return ContractHandler.SendRequestAsync(batchWithdrawFunction);
        }

        public Task<TransactionReceipt> BatchWithdrawRequestAndWaitForReceiptAsync(List<string> tokens, List<BigInteger> quantities, CancellationTokenSource cancellationToken = null)
        {
            var batchWithdrawFunction = new BatchWithdrawFunction();
                batchWithdrawFunction.Tokens = tokens;
                batchWithdrawFunction.Quantities = quantities;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(batchWithdrawFunction, cancellationToken);
        }

        public Task<string> AddPriceLibraryRequestAsync(AddPriceLibraryFunction addPriceLibraryFunction)
        {
             return ContractHandler.SendRequestAsync(addPriceLibraryFunction);
        }

        public Task<TransactionReceipt> AddPriceLibraryRequestAndWaitForReceiptAsync(AddPriceLibraryFunction addPriceLibraryFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(addPriceLibraryFunction, cancellationToken);
        }

        public Task<string> AddPriceLibraryRequestAsync(string priceLibrary)
        {
            var addPriceLibraryFunction = new AddPriceLibraryFunction();
                addPriceLibraryFunction.PriceLibrary = priceLibrary;
            
             return ContractHandler.SendRequestAsync(addPriceLibraryFunction);
        }

        public Task<TransactionReceipt> AddPriceLibraryRequestAndWaitForReceiptAsync(string priceLibrary, CancellationTokenSource cancellationToken = null)
        {
            var addPriceLibraryFunction = new AddPriceLibraryFunction();
                addPriceLibraryFunction.PriceLibrary = priceLibrary;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(addPriceLibraryFunction, cancellationToken);
        }

        public Task<string> RedeemToRequestAsync(RedeemToFunction redeemToFunction)
        {
             return ContractHandler.SendRequestAsync(redeemToFunction);
        }

        public Task<TransactionReceipt> RedeemToRequestAndWaitForReceiptAsync(RedeemToFunction redeemToFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(redeemToFunction, cancellationToken);
        }

        public Task<string> RedeemToRequestAsync(string recipient, string set, BigInteger quantity)
        {
            var redeemToFunction = new RedeemToFunction();
                redeemToFunction.Recipient = recipient;
                redeemToFunction.Set = set;
                redeemToFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAsync(redeemToFunction);
        }

        public Task<TransactionReceipt> RedeemToRequestAndWaitForReceiptAsync(string recipient, string set, BigInteger quantity, CancellationTokenSource cancellationToken = null)
        {
            var redeemToFunction = new RedeemToFunction();
                redeemToFunction.Recipient = recipient;
                redeemToFunction.Set = set;
                redeemToFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(redeemToFunction, cancellationToken);
        }

        public Task<string> TransferOwnershipRequestAsync(TransferOwnershipFunction transferOwnershipFunction)
        {
             return ContractHandler.SendRequestAsync(transferOwnershipFunction);
        }

        public Task<TransactionReceipt> TransferOwnershipRequestAndWaitForReceiptAsync(TransferOwnershipFunction transferOwnershipFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferOwnershipFunction, cancellationToken);
        }

        public Task<string> TransferOwnershipRequestAsync(string newOwner)
        {
            var transferOwnershipFunction = new TransferOwnershipFunction();
                transferOwnershipFunction.NewOwner = newOwner;
            
             return ContractHandler.SendRequestAsync(transferOwnershipFunction);
        }

        public Task<TransactionReceipt> TransferOwnershipRequestAndWaitForReceiptAsync(string newOwner, CancellationTokenSource cancellationToken = null)
        {
            var transferOwnershipFunction = new TransferOwnershipFunction();
                transferOwnershipFunction.NewOwner = newOwner;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferOwnershipFunction, cancellationToken);
        }

        public Task<string> WithdrawRequestAsync(WithdrawFunction withdrawFunction)
        {
             return ContractHandler.SendRequestAsync(withdrawFunction);
        }

        public Task<TransactionReceipt> WithdrawRequestAndWaitForReceiptAsync(WithdrawFunction withdrawFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(withdrawFunction, cancellationToken);
        }

        public Task<string> WithdrawRequestAsync(string token, BigInteger quantity)
        {
            var withdrawFunction = new WithdrawFunction();
                withdrawFunction.Token = token;
                withdrawFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAsync(withdrawFunction);
        }

        public Task<TransactionReceipt> WithdrawRequestAndWaitForReceiptAsync(string token, BigInteger quantity, CancellationTokenSource cancellationToken = null)
        {
            var withdrawFunction = new WithdrawFunction();
                withdrawFunction.Token = token;
                withdrawFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(withdrawFunction, cancellationToken);
        }

        public Task<List<string>> ModulesQueryAsync(ModulesFunction modulesFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<ModulesFunction, List<string>>(modulesFunction, blockParameter);
        }

        
        public Task<List<string>> ModulesQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<ModulesFunction, List<string>>(null, blockParameter);
        }

        public Task<string> VaultQueryAsync(VaultFunction vaultFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<VaultFunction, string>(vaultFunction, blockParameter);
        }

        
        public Task<string> VaultQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<VaultFunction, string>(null, blockParameter);
        }

        public Task<string> TransferModuleRequestAsync(TransferModuleFunction transferModuleFunction)
        {
             return ContractHandler.SendRequestAsync(transferModuleFunction);
        }

        public Task<TransactionReceipt> TransferModuleRequestAndWaitForReceiptAsync(TransferModuleFunction transferModuleFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferModuleFunction, cancellationToken);
        }

        public Task<string> TransferModuleRequestAsync(string token, BigInteger quantity, string from, string to)
        {
            var transferModuleFunction = new TransferModuleFunction();
                transferModuleFunction.Token = token;
                transferModuleFunction.Quantity = quantity;
                transferModuleFunction.From = from;
                transferModuleFunction.To = to;
            
             return ContractHandler.SendRequestAsync(transferModuleFunction);
        }

        public Task<TransactionReceipt> TransferModuleRequestAndWaitForReceiptAsync(string token, BigInteger quantity, string from, string to, CancellationTokenSource cancellationToken = null)
        {
            var transferModuleFunction = new TransferModuleFunction();
                transferModuleFunction.Token = token;
                transferModuleFunction.Quantity = quantity;
                transferModuleFunction.From = from;
                transferModuleFunction.To = to;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferModuleFunction, cancellationToken);
        }

        public Task<List<string>> FactoriesQueryAsync(FactoriesFunction factoriesFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<FactoriesFunction, List<string>>(factoriesFunction, blockParameter);
        }

        
        public Task<List<string>> FactoriesQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<FactoriesFunction, List<string>>(null, blockParameter);
        }

        public Task<string> BatchWithdrawModuleRequestAsync(BatchWithdrawModuleFunction batchWithdrawModuleFunction)
        {
             return ContractHandler.SendRequestAsync(batchWithdrawModuleFunction);
        }

        public Task<TransactionReceipt> BatchWithdrawModuleRequestAndWaitForReceiptAsync(BatchWithdrawModuleFunction batchWithdrawModuleFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(batchWithdrawModuleFunction, cancellationToken);
        }

        public Task<string> BatchWithdrawModuleRequestAsync(string from, string to, List<string> tokens, List<BigInteger> quantities)
        {
            var batchWithdrawModuleFunction = new BatchWithdrawModuleFunction();
                batchWithdrawModuleFunction.From = from;
                batchWithdrawModuleFunction.To = to;
                batchWithdrawModuleFunction.Tokens = tokens;
                batchWithdrawModuleFunction.Quantities = quantities;
            
             return ContractHandler.SendRequestAsync(batchWithdrawModuleFunction);
        }

        public Task<TransactionReceipt> BatchWithdrawModuleRequestAndWaitForReceiptAsync(string from, string to, List<string> tokens, List<BigInteger> quantities, CancellationTokenSource cancellationToken = null)
        {
            var batchWithdrawModuleFunction = new BatchWithdrawModuleFunction();
                batchWithdrawModuleFunction.From = from;
                batchWithdrawModuleFunction.To = to;
                batchWithdrawModuleFunction.Tokens = tokens;
                batchWithdrawModuleFunction.Quantities = quantities;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(batchWithdrawModuleFunction, cancellationToken);
        }

        public Task<bool> ValidSetsQueryAsync(ValidSetsFunction validSetsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<ValidSetsFunction, bool>(validSetsFunction, blockParameter);
        }

        
        public Task<bool> ValidSetsQueryAsync(string set, BlockParameter blockParameter = null)
        {
            var validSetsFunction = new ValidSetsFunction();
                validSetsFunction.Set = set;
            
            return ContractHandler.QueryAsync<ValidSetsFunction, bool>(validSetsFunction, blockParameter);
        }
    }
}
