using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.ContractHandlers;
using System.Threading;
using Trakx.Contracts.Set.Vault.ContractDefinition;

namespace Trakx.Contracts.Set.Vault
{
    public partial class VaultService
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.Web3 web3, VaultDeployment vaultDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<VaultDeployment>().SendRequestAndWaitForReceiptAsync(vaultDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.Web3 web3, VaultDeployment vaultDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<VaultDeployment>().SendRequestAsync(vaultDeployment);
        }

        public static async Task<VaultService> DeployContractAndGetServiceAsync(Nethereum.Web3.Web3 web3, VaultDeployment vaultDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, vaultDeployment, cancellationTokenSource);
            return new VaultService(web3, receipt.ContractAddress);
        }

        protected Nethereum.Web3.Web3 Web3{ get; }

        public ContractHandler ContractHandler { get; }

        public VaultService(Nethereum.Web3.Web3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }

        public Task<string> BatchIncrementTokenOwnerRequestAsync(BatchIncrementTokenOwnerFunction batchIncrementTokenOwnerFunction)
        {
             return ContractHandler.SendRequestAsync(batchIncrementTokenOwnerFunction);
        }

        public Task<TransactionReceipt> BatchIncrementTokenOwnerRequestAndWaitForReceiptAsync(BatchIncrementTokenOwnerFunction batchIncrementTokenOwnerFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(batchIncrementTokenOwnerFunction, cancellationToken);
        }

        public Task<string> BatchIncrementTokenOwnerRequestAsync(List<string> tokens, string owner, List<BigInteger> quantities)
        {
            var batchIncrementTokenOwnerFunction = new BatchIncrementTokenOwnerFunction();
                batchIncrementTokenOwnerFunction.Tokens = tokens;
                batchIncrementTokenOwnerFunction.Owner = owner;
                batchIncrementTokenOwnerFunction.Quantities = quantities;
            
             return ContractHandler.SendRequestAsync(batchIncrementTokenOwnerFunction);
        }

        public Task<TransactionReceipt> BatchIncrementTokenOwnerRequestAndWaitForReceiptAsync(List<string> tokens, string owner, List<BigInteger> quantities, CancellationTokenSource cancellationToken = null)
        {
            var batchIncrementTokenOwnerFunction = new BatchIncrementTokenOwnerFunction();
                batchIncrementTokenOwnerFunction.Tokens = tokens;
                batchIncrementTokenOwnerFunction.Owner = owner;
                batchIncrementTokenOwnerFunction.Quantities = quantities;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(batchIncrementTokenOwnerFunction, cancellationToken);
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

        public Task<BigInteger> GetOwnerBalanceQueryAsync(GetOwnerBalanceFunction getOwnerBalanceFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetOwnerBalanceFunction, BigInteger>(getOwnerBalanceFunction, blockParameter);
        }

        
        public Task<BigInteger> GetOwnerBalanceQueryAsync(string token, string owner, BlockParameter blockParameter = null)
        {
            var getOwnerBalanceFunction = new GetOwnerBalanceFunction();
                getOwnerBalanceFunction.Token = token;
                getOwnerBalanceFunction.Owner = owner;
            
            return ContractHandler.QueryAsync<GetOwnerBalanceFunction, BigInteger>(getOwnerBalanceFunction, blockParameter);
        }

        public Task<string> BatchTransferBalanceRequestAsync(BatchTransferBalanceFunction batchTransferBalanceFunction)
        {
             return ContractHandler.SendRequestAsync(batchTransferBalanceFunction);
        }

        public Task<TransactionReceipt> BatchTransferBalanceRequestAndWaitForReceiptAsync(BatchTransferBalanceFunction batchTransferBalanceFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(batchTransferBalanceFunction, cancellationToken);
        }

        public Task<string> BatchTransferBalanceRequestAsync(List<string> tokens, string from, string to, List<BigInteger> quantities)
        {
            var batchTransferBalanceFunction = new BatchTransferBalanceFunction();
                batchTransferBalanceFunction.Tokens = tokens;
                batchTransferBalanceFunction.From = from;
                batchTransferBalanceFunction.To = to;
                batchTransferBalanceFunction.Quantities = quantities;
            
             return ContractHandler.SendRequestAsync(batchTransferBalanceFunction);
        }

        public Task<TransactionReceipt> BatchTransferBalanceRequestAndWaitForReceiptAsync(List<string> tokens, string from, string to, List<BigInteger> quantities, CancellationTokenSource cancellationToken = null)
        {
            var batchTransferBalanceFunction = new BatchTransferBalanceFunction();
                batchTransferBalanceFunction.Tokens = tokens;
                batchTransferBalanceFunction.From = from;
                batchTransferBalanceFunction.To = to;
                batchTransferBalanceFunction.Quantities = quantities;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(batchTransferBalanceFunction, cancellationToken);
        }

        public Task<string> BatchWithdrawToRequestAsync(BatchWithdrawToFunction batchWithdrawToFunction)
        {
             return ContractHandler.SendRequestAsync(batchWithdrawToFunction);
        }

        public Task<TransactionReceipt> BatchWithdrawToRequestAndWaitForReceiptAsync(BatchWithdrawToFunction batchWithdrawToFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(batchWithdrawToFunction, cancellationToken);
        }

        public Task<string> BatchWithdrawToRequestAsync(List<string> tokens, string to, List<BigInteger> quantities)
        {
            var batchWithdrawToFunction = new BatchWithdrawToFunction();
                batchWithdrawToFunction.Tokens = tokens;
                batchWithdrawToFunction.To = to;
                batchWithdrawToFunction.Quantities = quantities;
            
             return ContractHandler.SendRequestAsync(batchWithdrawToFunction);
        }

        public Task<TransactionReceipt> BatchWithdrawToRequestAndWaitForReceiptAsync(List<string> tokens, string to, List<BigInteger> quantities, CancellationTokenSource cancellationToken = null)
        {
            var batchWithdrawToFunction = new BatchWithdrawToFunction();
                batchWithdrawToFunction.Tokens = tokens;
                batchWithdrawToFunction.To = to;
                batchWithdrawToFunction.Quantities = quantities;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(batchWithdrawToFunction, cancellationToken);
        }

        public Task<string> BatchDecrementTokenOwnerRequestAsync(BatchDecrementTokenOwnerFunction batchDecrementTokenOwnerFunction)
        {
             return ContractHandler.SendRequestAsync(batchDecrementTokenOwnerFunction);
        }

        public Task<TransactionReceipt> BatchDecrementTokenOwnerRequestAndWaitForReceiptAsync(BatchDecrementTokenOwnerFunction batchDecrementTokenOwnerFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(batchDecrementTokenOwnerFunction, cancellationToken);
        }

        public Task<string> BatchDecrementTokenOwnerRequestAsync(List<string> tokens, string owner, List<BigInteger> quantities)
        {
            var batchDecrementTokenOwnerFunction = new BatchDecrementTokenOwnerFunction();
                batchDecrementTokenOwnerFunction.Tokens = tokens;
                batchDecrementTokenOwnerFunction.Owner = owner;
                batchDecrementTokenOwnerFunction.Quantities = quantities;
            
             return ContractHandler.SendRequestAsync(batchDecrementTokenOwnerFunction);
        }

        public Task<TransactionReceipt> BatchDecrementTokenOwnerRequestAndWaitForReceiptAsync(List<string> tokens, string owner, List<BigInteger> quantities, CancellationTokenSource cancellationToken = null)
        {
            var batchDecrementTokenOwnerFunction = new BatchDecrementTokenOwnerFunction();
                batchDecrementTokenOwnerFunction.Tokens = tokens;
                batchDecrementTokenOwnerFunction.Owner = owner;
                batchDecrementTokenOwnerFunction.Quantities = quantities;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(batchDecrementTokenOwnerFunction, cancellationToken);
        }

        public Task<string> AddAuthorizedAddressRequestAsync(AddAuthorizedAddressFunction addAuthorizedAddressFunction)
        {
             return ContractHandler.SendRequestAsync(addAuthorizedAddressFunction);
        }

        public Task<TransactionReceipt> AddAuthorizedAddressRequestAndWaitForReceiptAsync(AddAuthorizedAddressFunction addAuthorizedAddressFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(addAuthorizedAddressFunction, cancellationToken);
        }

        public Task<string> AddAuthorizedAddressRequestAsync(string authTarget)
        {
            var addAuthorizedAddressFunction = new AddAuthorizedAddressFunction();
                addAuthorizedAddressFunction.AuthTarget = authTarget;
            
             return ContractHandler.SendRequestAsync(addAuthorizedAddressFunction);
        }

        public Task<TransactionReceipt> AddAuthorizedAddressRequestAndWaitForReceiptAsync(string authTarget, CancellationTokenSource cancellationToken = null)
        {
            var addAuthorizedAddressFunction = new AddAuthorizedAddressFunction();
                addAuthorizedAddressFunction.AuthTarget = authTarget;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(addAuthorizedAddressFunction, cancellationToken);
        }

        public Task<string> AuthoritiesQueryAsync(AuthoritiesFunction authoritiesFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<AuthoritiesFunction, string>(authoritiesFunction, blockParameter);
        }

        
        public Task<string> AuthoritiesQueryAsync(BigInteger returnValue1, BlockParameter blockParameter = null)
        {
            var authoritiesFunction = new AuthoritiesFunction();
                authoritiesFunction.ReturnValue1 = returnValue1;
            
            return ContractHandler.QueryAsync<AuthoritiesFunction, string>(authoritiesFunction, blockParameter);
        }

        public Task<string> RemoveAuthorizedAddressRequestAsync(RemoveAuthorizedAddressFunction removeAuthorizedAddressFunction)
        {
             return ContractHandler.SendRequestAsync(removeAuthorizedAddressFunction);
        }

        public Task<TransactionReceipt> RemoveAuthorizedAddressRequestAndWaitForReceiptAsync(RemoveAuthorizedAddressFunction removeAuthorizedAddressFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(removeAuthorizedAddressFunction, cancellationToken);
        }

        public Task<string> RemoveAuthorizedAddressRequestAsync(string authTarget)
        {
            var removeAuthorizedAddressFunction = new RemoveAuthorizedAddressFunction();
                removeAuthorizedAddressFunction.AuthTarget = authTarget;
            
             return ContractHandler.SendRequestAsync(removeAuthorizedAddressFunction);
        }

        public Task<TransactionReceipt> RemoveAuthorizedAddressRequestAndWaitForReceiptAsync(string authTarget, CancellationTokenSource cancellationToken = null)
        {
            var removeAuthorizedAddressFunction = new RemoveAuthorizedAddressFunction();
                removeAuthorizedAddressFunction.AuthTarget = authTarget;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(removeAuthorizedAddressFunction, cancellationToken);
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

        public Task<BigInteger> TimeLockPeriodQueryAsync(TimeLockPeriodFunction timeLockPeriodFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<TimeLockPeriodFunction, BigInteger>(timeLockPeriodFunction, blockParameter);
        }

        
        public Task<BigInteger> TimeLockPeriodQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<TimeLockPeriodFunction, BigInteger>(null, blockParameter);
        }

        public Task<string> DecrementTokenOwnerRequestAsync(DecrementTokenOwnerFunction decrementTokenOwnerFunction)
        {
             return ContractHandler.SendRequestAsync(decrementTokenOwnerFunction);
        }

        public Task<TransactionReceipt> DecrementTokenOwnerRequestAndWaitForReceiptAsync(DecrementTokenOwnerFunction decrementTokenOwnerFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(decrementTokenOwnerFunction, cancellationToken);
        }

        public Task<string> DecrementTokenOwnerRequestAsync(string token, string owner, BigInteger quantity)
        {
            var decrementTokenOwnerFunction = new DecrementTokenOwnerFunction();
                decrementTokenOwnerFunction.Token = token;
                decrementTokenOwnerFunction.Owner = owner;
                decrementTokenOwnerFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAsync(decrementTokenOwnerFunction);
        }

        public Task<TransactionReceipt> DecrementTokenOwnerRequestAndWaitForReceiptAsync(string token, string owner, BigInteger quantity, CancellationTokenSource cancellationToken = null)
        {
            var decrementTokenOwnerFunction = new DecrementTokenOwnerFunction();
                decrementTokenOwnerFunction.Token = token;
                decrementTokenOwnerFunction.Owner = owner;
                decrementTokenOwnerFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(decrementTokenOwnerFunction, cancellationToken);
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

        public Task<string> TransferBalanceRequestAsync(TransferBalanceFunction transferBalanceFunction)
        {
             return ContractHandler.SendRequestAsync(transferBalanceFunction);
        }

        public Task<TransactionReceipt> TransferBalanceRequestAndWaitForReceiptAsync(TransferBalanceFunction transferBalanceFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferBalanceFunction, cancellationToken);
        }

        public Task<string> TransferBalanceRequestAsync(string token, string from, string to, BigInteger quantity)
        {
            var transferBalanceFunction = new TransferBalanceFunction();
                transferBalanceFunction.Token = token;
                transferBalanceFunction.From = from;
                transferBalanceFunction.To = to;
                transferBalanceFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAsync(transferBalanceFunction);
        }

        public Task<TransactionReceipt> TransferBalanceRequestAndWaitForReceiptAsync(string token, string from, string to, BigInteger quantity, CancellationTokenSource cancellationToken = null)
        {
            var transferBalanceFunction = new TransferBalanceFunction();
                transferBalanceFunction.Token = token;
                transferBalanceFunction.From = from;
                transferBalanceFunction.To = to;
                transferBalanceFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferBalanceFunction, cancellationToken);
        }

        public Task<bool> AuthorizedQueryAsync(AuthorizedFunction authorizedFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<AuthorizedFunction, bool>(authorizedFunction, blockParameter);
        }

        
        public Task<bool> AuthorizedQueryAsync(string returnValue1, BlockParameter blockParameter = null)
        {
            var authorizedFunction = new AuthorizedFunction();
                authorizedFunction.ReturnValue1 = returnValue1;
            
            return ContractHandler.QueryAsync<AuthorizedFunction, bool>(authorizedFunction, blockParameter);
        }

        public Task<string> IncrementTokenOwnerRequestAsync(IncrementTokenOwnerFunction incrementTokenOwnerFunction)
        {
             return ContractHandler.SendRequestAsync(incrementTokenOwnerFunction);
        }

        public Task<TransactionReceipt> IncrementTokenOwnerRequestAndWaitForReceiptAsync(IncrementTokenOwnerFunction incrementTokenOwnerFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(incrementTokenOwnerFunction, cancellationToken);
        }

        public Task<string> IncrementTokenOwnerRequestAsync(string token, string owner, BigInteger quantity)
        {
            var incrementTokenOwnerFunction = new IncrementTokenOwnerFunction();
                incrementTokenOwnerFunction.Token = token;
                incrementTokenOwnerFunction.Owner = owner;
                incrementTokenOwnerFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAsync(incrementTokenOwnerFunction);
        }

        public Task<TransactionReceipt> IncrementTokenOwnerRequestAndWaitForReceiptAsync(string token, string owner, BigInteger quantity, CancellationTokenSource cancellationToken = null)
        {
            var incrementTokenOwnerFunction = new IncrementTokenOwnerFunction();
                incrementTokenOwnerFunction.Token = token;
                incrementTokenOwnerFunction.Owner = owner;
                incrementTokenOwnerFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(incrementTokenOwnerFunction, cancellationToken);
        }

        public Task<BigInteger> BalancesQueryAsync(BalancesFunction balancesFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<BalancesFunction, BigInteger>(balancesFunction, blockParameter);
        }

        
        public Task<BigInteger> BalancesQueryAsync(string returnValue1, string returnValue2, BlockParameter blockParameter = null)
        {
            var balancesFunction = new BalancesFunction();
                balancesFunction.ReturnValue1 = returnValue1;
                balancesFunction.ReturnValue2 = returnValue2;
            
            return ContractHandler.QueryAsync<BalancesFunction, BigInteger>(balancesFunction, blockParameter);
        }

        public Task<string> WithdrawToRequestAsync(WithdrawToFunction withdrawToFunction)
        {
             return ContractHandler.SendRequestAsync(withdrawToFunction);
        }

        public Task<TransactionReceipt> WithdrawToRequestAndWaitForReceiptAsync(WithdrawToFunction withdrawToFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(withdrawToFunction, cancellationToken);
        }

        public Task<string> WithdrawToRequestAsync(string token, string to, BigInteger quantity)
        {
            var withdrawToFunction = new WithdrawToFunction();
                withdrawToFunction.Token = token;
                withdrawToFunction.To = to;
                withdrawToFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAsync(withdrawToFunction);
        }

        public Task<TransactionReceipt> WithdrawToRequestAndWaitForReceiptAsync(string token, string to, BigInteger quantity, CancellationTokenSource cancellationToken = null)
        {
            var withdrawToFunction = new WithdrawToFunction();
                withdrawToFunction.Token = token;
                withdrawToFunction.To = to;
                withdrawToFunction.Quantity = quantity;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(withdrawToFunction, cancellationToken);
        }

        public Task<List<string>> GetAuthorizedAddressesQueryAsync(GetAuthorizedAddressesFunction getAuthorizedAddressesFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetAuthorizedAddressesFunction, List<string>>(getAuthorizedAddressesFunction, blockParameter);
        }

        
        public Task<List<string>> GetAuthorizedAddressesQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetAuthorizedAddressesFunction, List<string>>(null, blockParameter);
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
    }
}
