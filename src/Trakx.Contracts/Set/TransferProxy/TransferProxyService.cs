using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.ContractHandlers;
using System.Threading;
using Trakx.Contracts.Set.TransferProxy.ContractDefinition;

namespace Trakx.Contracts.Set.TransferProxy
{
    public partial class TransferProxyService
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.Web3 web3, TransferProxyDeployment transferProxyDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<TransferProxyDeployment>().SendRequestAndWaitForReceiptAsync(transferProxyDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.Web3 web3, TransferProxyDeployment transferProxyDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<TransferProxyDeployment>().SendRequestAsync(transferProxyDeployment);
        }

        public static async Task<TransferProxyService> DeployContractAndGetServiceAsync(Nethereum.Web3.Web3 web3, TransferProxyDeployment transferProxyDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, transferProxyDeployment, cancellationTokenSource);
            return new TransferProxyService(web3, receipt.ContractAddress);
        }

        protected Nethereum.Web3.Web3 Web3{ get; }

        public ContractHandler ContractHandler { get; }

        public TransferProxyService(Nethereum.Web3.Web3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
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

        public Task<string> BatchTransferRequestAsync(BatchTransferFunction batchTransferFunction)
        {
             return ContractHandler.SendRequestAsync(batchTransferFunction);
        }

        public Task<TransactionReceipt> BatchTransferRequestAndWaitForReceiptAsync(BatchTransferFunction batchTransferFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(batchTransferFunction, cancellationToken);
        }

        public Task<string> BatchTransferRequestAsync(List<string> tokens, List<BigInteger> quantities, string from, string to)
        {
            var batchTransferFunction = new BatchTransferFunction();
                batchTransferFunction.Tokens = tokens;
                batchTransferFunction.Quantities = quantities;
                batchTransferFunction.From = from;
                batchTransferFunction.To = to;
            
             return ContractHandler.SendRequestAsync(batchTransferFunction);
        }

        public Task<TransactionReceipt> BatchTransferRequestAndWaitForReceiptAsync(List<string> tokens, List<BigInteger> quantities, string from, string to, CancellationTokenSource cancellationToken = null)
        {
            var batchTransferFunction = new BatchTransferFunction();
                batchTransferFunction.Tokens = tokens;
                batchTransferFunction.Quantities = quantities;
                batchTransferFunction.From = from;
                batchTransferFunction.To = to;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(batchTransferFunction, cancellationToken);
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

        public Task<string> TransferRequestAsync(TransferFunction transferFunction)
        {
             return ContractHandler.SendRequestAsync(transferFunction);
        }

        public Task<TransactionReceipt> TransferRequestAndWaitForReceiptAsync(TransferFunction transferFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferFunction, cancellationToken);
        }

        public Task<string> TransferRequestAsync(string token, BigInteger quantity, string from, string to)
        {
            var transferFunction = new TransferFunction();
                transferFunction.Token = token;
                transferFunction.Quantity = quantity;
                transferFunction.From = from;
                transferFunction.To = to;
            
             return ContractHandler.SendRequestAsync(transferFunction);
        }

        public Task<TransactionReceipt> TransferRequestAndWaitForReceiptAsync(string token, BigInteger quantity, string from, string to, CancellationTokenSource cancellationToken = null)
        {
            var transferFunction = new TransferFunction();
                transferFunction.Token = token;
                transferFunction.Quantity = quantity;
                transferFunction.From = from;
                transferFunction.To = to;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferFunction, cancellationToken);
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
