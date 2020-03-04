using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.ContractHandlers;
using System.Threading;
using Nethereum.Web3;
using Trakx.Contracts.Set.CoreIssuanceLibrary.ContractDefinition;

namespace Trakx.Contracts.Set.CoreIssuanceLibrary
{
    public partial class CoreIssuanceLibraryService
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(IWeb3 web3, CoreIssuanceLibraryDeployment coreIssuanceLibraryDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<CoreIssuanceLibraryDeployment>().SendRequestAndWaitForReceiptAsync(coreIssuanceLibraryDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(IWeb3 web3, CoreIssuanceLibraryDeployment coreIssuanceLibraryDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<CoreIssuanceLibraryDeployment>().SendRequestAsync(coreIssuanceLibraryDeployment);
        }

        public static async Task<CoreIssuanceLibraryService> DeployContractAndGetServiceAsync(IWeb3 web3, CoreIssuanceLibraryDeployment coreIssuanceLibraryDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, coreIssuanceLibraryDeployment, cancellationTokenSource);
            return new CoreIssuanceLibraryService(web3, receipt.ContractAddress);
        }

        protected IWeb3 Web3{ get; }

        public ContractHandler ContractHandler { get; }

        public CoreIssuanceLibraryService(IWeb3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }

        public Task<CalculateWithdrawAndIncrementQuantitiesOutputDTO> CalculateWithdrawAndIncrementQuantitiesQueryAsync(CalculateWithdrawAndIncrementQuantitiesFunction calculateWithdrawAndIncrementQuantitiesFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<CalculateWithdrawAndIncrementQuantitiesFunction, CalculateWithdrawAndIncrementQuantitiesOutputDTO>(calculateWithdrawAndIncrementQuantitiesFunction, blockParameter);
        }

        public Task<CalculateWithdrawAndIncrementQuantitiesOutputDTO> CalculateWithdrawAndIncrementQuantitiesQueryAsync(List<BigInteger> componentQuantities, BigInteger toExclude, BlockParameter blockParameter = null)
        {
            var calculateWithdrawAndIncrementQuantitiesFunction = new CalculateWithdrawAndIncrementQuantitiesFunction();
                calculateWithdrawAndIncrementQuantitiesFunction.ComponentQuantities = componentQuantities;
                calculateWithdrawAndIncrementQuantitiesFunction.ToExclude = toExclude;
            
            return ContractHandler.QueryDeserializingToObjectAsync<CalculateWithdrawAndIncrementQuantitiesFunction, CalculateWithdrawAndIncrementQuantitiesOutputDTO>(calculateWithdrawAndIncrementQuantitiesFunction, blockParameter);
        }

        public Task<List<BigInteger>> CalculateRequiredComponentQuantitiesQueryAsync(CalculateRequiredComponentQuantitiesFunction calculateRequiredComponentQuantitiesFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<CalculateRequiredComponentQuantitiesFunction, List<BigInteger>>(calculateRequiredComponentQuantitiesFunction, blockParameter);
        }

        
        public Task<List<BigInteger>> CalculateRequiredComponentQuantitiesQueryAsync(List<BigInteger> componentUnits, BigInteger naturalUnit, BigInteger quantity, BlockParameter blockParameter = null)
        {
            var calculateRequiredComponentQuantitiesFunction = new CalculateRequiredComponentQuantitiesFunction();
                calculateRequiredComponentQuantitiesFunction.ComponentUnits = componentUnits;
                calculateRequiredComponentQuantitiesFunction.NaturalUnit = naturalUnit;
                calculateRequiredComponentQuantitiesFunction.Quantity = quantity;
            
            return ContractHandler.QueryAsync<CalculateRequiredComponentQuantitiesFunction, List<BigInteger>>(calculateRequiredComponentQuantitiesFunction, blockParameter);
        }

        public Task<CalculateDepositAndDecrementQuantitiesOutputDTO> CalculateDepositAndDecrementQuantitiesQueryAsync(CalculateDepositAndDecrementQuantitiesFunction calculateDepositAndDecrementQuantitiesFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<CalculateDepositAndDecrementQuantitiesFunction, CalculateDepositAndDecrementQuantitiesOutputDTO>(calculateDepositAndDecrementQuantitiesFunction, blockParameter);
        }

        public Task<CalculateDepositAndDecrementQuantitiesOutputDTO> CalculateDepositAndDecrementQuantitiesQueryAsync(List<string> components, List<BigInteger> componentQuantities, string owner, string vault, BlockParameter blockParameter = null)
        {
            var calculateDepositAndDecrementQuantitiesFunction = new CalculateDepositAndDecrementQuantitiesFunction();
                calculateDepositAndDecrementQuantitiesFunction.Components = components;
                calculateDepositAndDecrementQuantitiesFunction.ComponentQuantities = componentQuantities;
                calculateDepositAndDecrementQuantitiesFunction.Owner = owner;
                calculateDepositAndDecrementQuantitiesFunction.Vault = vault;
            
            return ContractHandler.QueryDeserializingToObjectAsync<CalculateDepositAndDecrementQuantitiesFunction, CalculateDepositAndDecrementQuantitiesOutputDTO>(calculateDepositAndDecrementQuantitiesFunction, blockParameter);
        }
    }
}
