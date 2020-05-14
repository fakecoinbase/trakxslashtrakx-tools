using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.Contracts;
using System.Threading;
using Trakx.Contracts.Wrapping.WrappedToken.ContractDefinition;

namespace Trakx.Contracts.Wrapping.WrappedToken
{
    public partial class WrappedTokenService
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(IWeb3 web3, WrappedTokenDeployment wrappedTokenDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<WrappedTokenDeployment>().SendRequestAndWaitForReceiptAsync(wrappedTokenDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(IWeb3 web3, WrappedTokenDeployment wrappedTokenDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<WrappedTokenDeployment>().SendRequestAsync(wrappedTokenDeployment);
        }

        public static async Task<WrappedTokenService> DeployContractAndGetServiceAsync(IWeb3 web3, WrappedTokenDeployment wrappedTokenDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, wrappedTokenDeployment, cancellationTokenSource);
            return new WrappedTokenService(web3, receipt.ContractAddress);
        }

        protected IWeb3 Web3{ get; }

        public ContractHandler ContractHandler { get; }

        public WrappedTokenService(IWeb3 web3, string tokenSymbol)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(RinkebyDeployedContractAddresses.AddressByName[tokenSymbol.ToLower()]);
        }

        public Task<string> AddBurnerRequestAsync(AddBurnerFunction addBurnerFunction)
        {
             return ContractHandler.SendRequestAsync(addBurnerFunction);
        }

        public Task<TransactionReceipt> AddBurnerRequestAndWaitForReceiptAsync(AddBurnerFunction addBurnerFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(addBurnerFunction, cancellationToken);
        }

        public Task<string> AddBurnerRequestAsync(string addressToAdd)
        {
            var addBurnerFunction = new AddBurnerFunction();
                addBurnerFunction.AddressToAdd = addressToAdd;
            
             return ContractHandler.SendRequestAsync(addBurnerFunction);
        }

        public Task<TransactionReceipt> AddBurnerRequestAndWaitForReceiptAsync(string addressToAdd, CancellationTokenSource cancellationToken = null)
        {
            var addBurnerFunction = new AddBurnerFunction();
                addBurnerFunction.AddressToAdd = addressToAdd;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(addBurnerFunction, cancellationToken);
        }

        public Task<string> AddMinterRequestAsync(AddMinterFunction addMinterFunction)
        {
             return ContractHandler.SendRequestAsync(addMinterFunction);
        }

        public Task<TransactionReceipt> AddMinterRequestAndWaitForReceiptAsync(AddMinterFunction addMinterFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(addMinterFunction, cancellationToken);
        }

        public Task<string> AddMinterRequestAsync(string addressToAdd)
        {
            var addMinterFunction = new AddMinterFunction();
                addMinterFunction.AddressToAdd = addressToAdd;
            
             return ContractHandler.SendRequestAsync(addMinterFunction);
        }

        public Task<TransactionReceipt> AddMinterRequestAndWaitForReceiptAsync(string addressToAdd, CancellationTokenSource cancellationToken = null)
        {
            var addMinterFunction = new AddMinterFunction();
                addMinterFunction.AddressToAdd = addressToAdd;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(addMinterFunction, cancellationToken);
        }

        public Task<BigInteger> AllowanceQueryAsync(AllowanceFunction allowanceFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<AllowanceFunction, BigInteger>(allowanceFunction, blockParameter);
        }

        
        public Task<BigInteger> AllowanceQueryAsync(string holder, string spender, BlockParameter blockParameter = null)
        {
            var allowanceFunction = new AllowanceFunction();
                allowanceFunction.Holder = holder;
                allowanceFunction.Spender = spender;
            
            return ContractHandler.QueryAsync<AllowanceFunction, BigInteger>(allowanceFunction, blockParameter);
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

        public Task<string> AuthorizeOperatorRequestAsync(AuthorizeOperatorFunction authorizeOperatorFunction)
        {
             return ContractHandler.SendRequestAsync(authorizeOperatorFunction);
        }

        public Task<TransactionReceipt> AuthorizeOperatorRequestAndWaitForReceiptAsync(AuthorizeOperatorFunction authorizeOperatorFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(authorizeOperatorFunction, cancellationToken);
        }

        public Task<string> AuthorizeOperatorRequestAsync(string operatr)
        {
            var authorizeOperatorFunction = new AuthorizeOperatorFunction();
                authorizeOperatorFunction.Operator = operatr;
            
             return ContractHandler.SendRequestAsync(authorizeOperatorFunction);
        }

        public Task<TransactionReceipt> AuthorizeOperatorRequestAndWaitForReceiptAsync(string operatr, CancellationTokenSource cancellationToken = null)
        {
            var authorizeOperatorFunction = new AuthorizeOperatorFunction();
                authorizeOperatorFunction.Operator = operatr;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(authorizeOperatorFunction, cancellationToken);
        }

        public Task<BigInteger> BalanceOfQueryAsync(BalanceOfFunction balanceOfFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<BalanceOfFunction, BigInteger>(balanceOfFunction, blockParameter);
        }

        
        public Task<BigInteger> BalanceOfQueryAsync(string tokenHolder, BlockParameter blockParameter = null)
        {
            var balanceOfFunction = new BalanceOfFunction();
                balanceOfFunction.TokenHolder = tokenHolder;
            
            return ContractHandler.QueryAsync<BalanceOfFunction, BigInteger>(balanceOfFunction, blockParameter);
        }

        public Task<string> BurnRequestAsync(BurnFunction burnFunction)
        {
             return ContractHandler.SendRequestAsync(burnFunction);
        }

        public Task<TransactionReceipt> BurnRequestAndWaitForReceiptAsync(BurnFunction burnFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(burnFunction, cancellationToken);
        }

        public Task<string> BurnRequestAsync(BigInteger amount, byte[] data)
        {
            var burnFunction = new BurnFunction();
                burnFunction.Amount = amount;
                burnFunction.Data = data;
            
             return ContractHandler.SendRequestAsync(burnFunction);
        }

        public Task<TransactionReceipt> BurnRequestAndWaitForReceiptAsync(BigInteger amount, byte[] data, CancellationTokenSource cancellationToken = null)
        {
            var burnFunction = new BurnFunction();
                burnFunction.Amount = amount;
                burnFunction.Data = data;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(burnFunction, cancellationToken);
        }

        public Task<byte> DecimalsQueryAsync(DecimalsFunction decimalsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<DecimalsFunction, byte>(decimalsFunction, blockParameter);
        }

        
        public Task<byte> DecimalsQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<DecimalsFunction, byte>(null, blockParameter);
        }

        public Task<List<string>> DefaultOperatorsQueryAsync(DefaultOperatorsFunction defaultOperatorsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<DefaultOperatorsFunction, List<string>>(defaultOperatorsFunction, blockParameter);
        }

        
        public Task<List<string>> DefaultOperatorsQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<DefaultOperatorsFunction, List<string>>(null, blockParameter);
        }

        public Task<BigInteger> GranularityQueryAsync(GranularityFunction granularityFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GranularityFunction, BigInteger>(granularityFunction, blockParameter);
        }

        
        public Task<BigInteger> GranularityQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GranularityFunction, BigInteger>(null, blockParameter);
        }

        public Task<bool> IsAdminQueryAsync(IsAdminFunction isAdminFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<IsAdminFunction, bool>(isAdminFunction, blockParameter);
        }

        
        public Task<bool> IsAdminQueryAsync(string addressToTest, BlockParameter blockParameter = null)
        {
            var isAdminFunction = new IsAdminFunction();
                isAdminFunction.AddressToTest = addressToTest;
            
            return ContractHandler.QueryAsync<IsAdminFunction, bool>(isAdminFunction, blockParameter);
        }

        public Task<bool> IsBurnerQueryAsync(IsBurnerFunction isBurnerFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<IsBurnerFunction, bool>(isBurnerFunction, blockParameter);
        }

        
        public Task<bool> IsBurnerQueryAsync(string addressToTest, BlockParameter blockParameter = null)
        {
            var isBurnerFunction = new IsBurnerFunction();
                isBurnerFunction.AddressToTest = addressToTest;
            
            return ContractHandler.QueryAsync<IsBurnerFunction, bool>(isBurnerFunction, blockParameter);
        }

        public Task<bool> IsMinterQueryAsync(IsMinterFunction isMinterFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<IsMinterFunction, bool>(isMinterFunction, blockParameter);
        }

        
        public Task<bool> IsMinterQueryAsync(string addressToTest, BlockParameter blockParameter = null)
        {
            var isMinterFunction = new IsMinterFunction();
                isMinterFunction.AddressToTest = addressToTest;
            
            return ContractHandler.QueryAsync<IsMinterFunction, bool>(isMinterFunction, blockParameter);
        }

        public Task<bool> IsOperatorForQueryAsync(IsOperatorForFunction isOperatorForFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<IsOperatorForFunction, bool>(isOperatorForFunction, blockParameter);
        }

        
        public Task<bool> IsOperatorForQueryAsync(string operatr, string tokenHolder, BlockParameter blockParameter = null)
        {
            var isOperatorForFunction = new IsOperatorForFunction();
                isOperatorForFunction.Operator = operatr;
                isOperatorForFunction.TokenHolder = tokenHolder;
            
            return ContractHandler.QueryAsync<IsOperatorForFunction, bool>(isOperatorForFunction, blockParameter);
        }

        public Task<string> MintRequestAsync(MintFunction mintFunction)
        {
             return ContractHandler.SendRequestAsync(mintFunction);
        }

        public Task<TransactionReceipt> MintRequestAndWaitForReceiptAsync(MintFunction mintFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(mintFunction, cancellationToken);
        }

        public Task<string> MintRequestAsync(string account, BigInteger amount, byte[] userData, byte[] operatorData)
        {
            var mintFunction = new MintFunction();
                mintFunction.Account = account;
                mintFunction.Amount = amount;
                mintFunction.UserData = userData;
                mintFunction.OperatorData = operatorData;
            
             return ContractHandler.SendRequestAsync(mintFunction);
        }

        public Task<TransactionReceipt> MintRequestAndWaitForReceiptAsync(string account, BigInteger amount, byte[] userData, byte[] operatorData, CancellationTokenSource cancellationToken = null)
        {
            var mintFunction = new MintFunction();
                mintFunction.Account = account;
                mintFunction.Amount = amount;
                mintFunction.UserData = userData;
                mintFunction.OperatorData = operatorData;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(mintFunction, cancellationToken);
        }

        public Task<string> NameQueryAsync(NameFunction nameFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<NameFunction, string>(nameFunction, blockParameter);
        }

        
        public Task<string> NameQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<NameFunction, string>(null, blockParameter);
        }

        public Task<string> OperatorBurnRequestAsync(OperatorBurnFunction operatorBurnFunction)
        {
             return ContractHandler.SendRequestAsync(operatorBurnFunction);
        }

        public Task<TransactionReceipt> OperatorBurnRequestAndWaitForReceiptAsync(OperatorBurnFunction operatorBurnFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(operatorBurnFunction, cancellationToken);
        }

        public Task<string> OperatorBurnRequestAsync(string account, BigInteger amount, byte[] data, byte[] operatorData)
        {
            var operatorBurnFunction = new OperatorBurnFunction();
                operatorBurnFunction.Account = account;
                operatorBurnFunction.Amount = amount;
                operatorBurnFunction.Data = data;
                operatorBurnFunction.OperatorData = operatorData;
            
             return ContractHandler.SendRequestAsync(operatorBurnFunction);
        }

        public Task<TransactionReceipt> OperatorBurnRequestAndWaitForReceiptAsync(string account, BigInteger amount, byte[] data, byte[] operatorData, CancellationTokenSource cancellationToken = null)
        {
            var operatorBurnFunction = new OperatorBurnFunction();
                operatorBurnFunction.Account = account;
                operatorBurnFunction.Amount = amount;
                operatorBurnFunction.Data = data;
                operatorBurnFunction.OperatorData = operatorData;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(operatorBurnFunction, cancellationToken);
        }

        public Task<string> OperatorSendRequestAsync(OperatorSendFunction operatorSendFunction)
        {
             return ContractHandler.SendRequestAsync(operatorSendFunction);
        }

        public Task<TransactionReceipt> OperatorSendRequestAndWaitForReceiptAsync(OperatorSendFunction operatorSendFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(operatorSendFunction, cancellationToken);
        }

        public Task<string> OperatorSendRequestAsync(string sender, string recipient, BigInteger amount, byte[] data, byte[] operatorData)
        {
            var operatorSendFunction = new OperatorSendFunction();
                operatorSendFunction.Sender = sender;
                operatorSendFunction.Recipient = recipient;
                operatorSendFunction.Amount = amount;
                operatorSendFunction.Data = data;
                operatorSendFunction.OperatorData = operatorData;
            
             return ContractHandler.SendRequestAsync(operatorSendFunction);
        }

        public Task<TransactionReceipt> OperatorSendRequestAndWaitForReceiptAsync(string sender, string recipient, BigInteger amount, byte[] data, byte[] operatorData, CancellationTokenSource cancellationToken = null)
        {
            var operatorSendFunction = new OperatorSendFunction();
                operatorSendFunction.Sender = sender;
                operatorSendFunction.Recipient = recipient;
                operatorSendFunction.Amount = amount;
                operatorSendFunction.Data = data;
                operatorSendFunction.OperatorData = operatorData;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(operatorSendFunction, cancellationToken);
        }

        public Task<string> RemoveBurnerRequestAsync(RemoveBurnerFunction removeBurnerFunction)
        {
             return ContractHandler.SendRequestAsync(removeBurnerFunction);
        }

        public Task<TransactionReceipt> RemoveBurnerRequestAndWaitForReceiptAsync(RemoveBurnerFunction removeBurnerFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(removeBurnerFunction, cancellationToken);
        }

        public Task<string> RemoveBurnerRequestAsync(string addressToRemove)
        {
            var removeBurnerFunction = new RemoveBurnerFunction();
                removeBurnerFunction.AddressToRemove = addressToRemove;
            
             return ContractHandler.SendRequestAsync(removeBurnerFunction);
        }

        public Task<TransactionReceipt> RemoveBurnerRequestAndWaitForReceiptAsync(string addressToRemove, CancellationTokenSource cancellationToken = null)
        {
            var removeBurnerFunction = new RemoveBurnerFunction();
                removeBurnerFunction.AddressToRemove = addressToRemove;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(removeBurnerFunction, cancellationToken);
        }

        public Task<string> RemoveMinterRequestAsync(RemoveMinterFunction removeMinterFunction)
        {
             return ContractHandler.SendRequestAsync(removeMinterFunction);
        }

        public Task<TransactionReceipt> RemoveMinterRequestAndWaitForReceiptAsync(RemoveMinterFunction removeMinterFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(removeMinterFunction, cancellationToken);
        }

        public Task<string> RemoveMinterRequestAsync(string addressToRemove)
        {
            var removeMinterFunction = new RemoveMinterFunction();
                removeMinterFunction.AddressToRemove = addressToRemove;
            
             return ContractHandler.SendRequestAsync(removeMinterFunction);
        }

        public Task<TransactionReceipt> RemoveMinterRequestAndWaitForReceiptAsync(string addressToRemove, CancellationTokenSource cancellationToken = null)
        {
            var removeMinterFunction = new RemoveMinterFunction();
                removeMinterFunction.AddressToRemove = addressToRemove;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(removeMinterFunction, cancellationToken);
        }

        public Task<string> RevokeOperatorRequestAsync(RevokeOperatorFunction revokeOperatorFunction)
        {
             return ContractHandler.SendRequestAsync(revokeOperatorFunction);
        }

        public Task<TransactionReceipt> RevokeOperatorRequestAndWaitForReceiptAsync(RevokeOperatorFunction revokeOperatorFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(revokeOperatorFunction, cancellationToken);
        }

        public Task<string> RevokeOperatorRequestAsync(string operatr)
        {
            var revokeOperatorFunction = new RevokeOperatorFunction();
                revokeOperatorFunction.Operator = operatr;
            
             return ContractHandler.SendRequestAsync(revokeOperatorFunction);
        }

        public Task<TransactionReceipt> RevokeOperatorRequestAndWaitForReceiptAsync(string operatr, CancellationTokenSource cancellationToken = null)
        {
            var revokeOperatorFunction = new RevokeOperatorFunction();
                revokeOperatorFunction.Operator = operatr;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(revokeOperatorFunction, cancellationToken);
        }

        public Task<string> SendRequestAsync(SendFunction sendFunction)
        {
             return ContractHandler.SendRequestAsync(sendFunction);
        }

        public Task<TransactionReceipt> SendRequestAndWaitForReceiptAsync(SendFunction sendFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(sendFunction, cancellationToken);
        }

        public Task<string> SendRequestAsync(string recipient, BigInteger amount, byte[] data)
        {
            var sendFunction = new SendFunction();
                sendFunction.Recipient = recipient;
                sendFunction.Amount = amount;
                sendFunction.Data = data;
            
             return ContractHandler.SendRequestAsync(sendFunction);
        }

        public Task<TransactionReceipt> SendRequestAndWaitForReceiptAsync(string recipient, BigInteger amount, byte[] data, CancellationTokenSource cancellationToken = null)
        {
            var sendFunction = new SendFunction();
                sendFunction.Recipient = recipient;
                sendFunction.Amount = amount;
                sendFunction.Data = data;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(sendFunction, cancellationToken);
        }

        public Task<string> SymbolQueryAsync(SymbolFunction symbolFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<SymbolFunction, string>(symbolFunction, blockParameter);
        }

        
        public Task<string> SymbolQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<SymbolFunction, string>(null, blockParameter);
        }

        public Task<BigInteger> TotalSupplyQueryAsync(TotalSupplyFunction totalSupplyFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<TotalSupplyFunction, BigInteger>(totalSupplyFunction, blockParameter);
        }

        
        public Task<BigInteger> TotalSupplyQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<TotalSupplyFunction, BigInteger>(null, blockParameter);
        }

        public Task<string> TransferRequestAsync(TransferFunction transferFunction)
        {
             return ContractHandler.SendRequestAsync(transferFunction);
        }

        public Task<TransactionReceipt> TransferRequestAndWaitForReceiptAsync(TransferFunction transferFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferFunction, cancellationToken);
        }

        public Task<string> TransferRequestAsync(string recipient, BigInteger amount)
        {
            var transferFunction = new TransferFunction();
                transferFunction.Recipient = recipient;
                transferFunction.Amount = amount;
            
             return ContractHandler.SendRequestAsync(transferFunction);
        }

        public Task<TransactionReceipt> TransferRequestAndWaitForReceiptAsync(string recipient, BigInteger amount, CancellationTokenSource cancellationToken = null)
        {
            var transferFunction = new TransferFunction();
                transferFunction.Recipient = recipient;
                transferFunction.Amount = amount;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferFunction, cancellationToken);
        }

        public Task<string> TransferFromRequestAsync(TransferFromFunction transferFromFunction)
        {
             return ContractHandler.SendRequestAsync(transferFromFunction);
        }

        public Task<TransactionReceipt> TransferFromRequestAndWaitForReceiptAsync(TransferFromFunction transferFromFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferFromFunction, cancellationToken);
        }

        public Task<string> TransferFromRequestAsync(string holder, string recipient, BigInteger amount)
        {
            var transferFromFunction = new TransferFromFunction();
                transferFromFunction.Holder = holder;
                transferFromFunction.Recipient = recipient;
                transferFromFunction.Amount = amount;
            
             return ContractHandler.SendRequestAsync(transferFromFunction);
        }

        public Task<TransactionReceipt> TransferFromRequestAndWaitForReceiptAsync(string holder, string recipient, BigInteger amount, CancellationTokenSource cancellationToken = null)
        {
            var transferFromFunction = new TransferFromFunction();
                transferFromFunction.Holder = holder;
                transferFromFunction.Recipient = recipient;
                transferFromFunction.Amount = amount;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferFromFunction, cancellationToken);
        }
    }
}
