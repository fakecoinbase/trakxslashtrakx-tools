using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.ContractHandlers;
using System.Threading;
using Trakx.Contracts.Set.WhiteList.ContractDefinition;

namespace Trakx.Contracts.Set.WhiteList
{
    public partial class WhiteListService
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.Web3 web3, WhiteListDeployment whiteListDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<WhiteListDeployment>().SendRequestAndWaitForReceiptAsync(whiteListDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.Web3 web3, WhiteListDeployment whiteListDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<WhiteListDeployment>().SendRequestAsync(whiteListDeployment);
        }

        public static async Task<WhiteListService> DeployContractAndGetServiceAsync(Nethereum.Web3.Web3 web3, WhiteListDeployment whiteListDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, whiteListDeployment, cancellationTokenSource);
            return new WhiteListService(web3, receipt.ContractAddress);
        }

        protected Nethereum.Web3.Web3 Web3{ get; }

        public ContractHandler ContractHandler { get; }

        public WhiteListService(Nethereum.Web3.Web3 web3, string contractAddress)
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

        public Task<bool> AreValidAddressesQueryAsync(AreValidAddressesFunction areValidAddressesFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<AreValidAddressesFunction, bool>(areValidAddressesFunction, blockParameter);
        }

        
        public Task<bool> AreValidAddressesQueryAsync(List<string> addresses, BlockParameter blockParameter = null)
        {
            var areValidAddressesFunction = new AreValidAddressesFunction();
                areValidAddressesFunction.Addresses = addresses;
            
            return ContractHandler.QueryAsync<AreValidAddressesFunction, bool>(areValidAddressesFunction, blockParameter);
        }

        public Task<bool> WhiteListQueryAsync(WhiteListFunction whiteListFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<WhiteListFunction, bool>(whiteListFunction, blockParameter);
        }

        
        public Task<bool> WhiteListQueryAsync(string returnValue1, BlockParameter blockParameter = null)
        {
            var whiteListFunction = new WhiteListFunction();
                whiteListFunction.ReturnValue1 = returnValue1;
            
            return ContractHandler.QueryAsync<WhiteListFunction, bool>(whiteListFunction, blockParameter);
        }

        public Task<string> AddAddressRequestAsync(AddAddressFunction addAddressFunction)
        {
             return ContractHandler.SendRequestAsync(addAddressFunction);
        }

        public Task<TransactionReceipt> AddAddressRequestAndWaitForReceiptAsync(AddAddressFunction addAddressFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(addAddressFunction, cancellationToken);
        }

        public Task<string> AddAddressRequestAsync(string address)
        {
            var addAddressFunction = new AddAddressFunction();
                addAddressFunction.Address = address;
            
             return ContractHandler.SendRequestAsync(addAddressFunction);
        }

        public Task<TransactionReceipt> AddAddressRequestAndWaitForReceiptAsync(string address, CancellationTokenSource cancellationToken = null)
        {
            var addAddressFunction = new AddAddressFunction();
                addAddressFunction.Address = address;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(addAddressFunction, cancellationToken);
        }

        public Task<string> RemoveAddressRequestAsync(RemoveAddressFunction removeAddressFunction)
        {
             return ContractHandler.SendRequestAsync(removeAddressFunction);
        }

        public Task<TransactionReceipt> RemoveAddressRequestAndWaitForReceiptAsync(RemoveAddressFunction removeAddressFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(removeAddressFunction, cancellationToken);
        }

        public Task<string> RemoveAddressRequestAsync(string address)
        {
            var removeAddressFunction = new RemoveAddressFunction();
                removeAddressFunction.Address = address;
            
             return ContractHandler.SendRequestAsync(removeAddressFunction);
        }

        public Task<TransactionReceipt> RemoveAddressRequestAndWaitForReceiptAsync(string address, CancellationTokenSource cancellationToken = null)
        {
            var removeAddressFunction = new RemoveAddressFunction();
                removeAddressFunction.Address = address;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(removeAddressFunction, cancellationToken);
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

        public Task<List<string>> ValidAddressesQueryAsync(ValidAddressesFunction validAddressesFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<ValidAddressesFunction, List<string>>(validAddressesFunction, blockParameter);
        }

        
        public Task<List<string>> ValidAddressesQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<ValidAddressesFunction, List<string>>(null, blockParameter);
        }

        public Task<string> AddressesQueryAsync(AddressesFunction addressesFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<AddressesFunction, string>(addressesFunction, blockParameter);
        }

        
        public Task<string> AddressesQueryAsync(BigInteger returnValue1, BlockParameter blockParameter = null)
        {
            var addressesFunction = new AddressesFunction();
                addressesFunction.ReturnValue1 = returnValue1;
            
            return ContractHandler.QueryAsync<AddressesFunction, string>(addressesFunction, blockParameter);
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
