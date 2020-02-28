using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.ContractHandlers;
using System.Threading;
using Trakx.Contracts.Set.RebalancingSetTokenFactory.ContractDefinition;

namespace Trakx.Contracts.Set.RebalancingSetTokenFactory
{
    public partial class RebalancingSetTokenFactoryService
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.Web3 web3, RebalancingSetTokenFactoryDeployment rebalancingSetTokenFactoryDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<RebalancingSetTokenFactoryDeployment>().SendRequestAndWaitForReceiptAsync(rebalancingSetTokenFactoryDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.Web3 web3, RebalancingSetTokenFactoryDeployment rebalancingSetTokenFactoryDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<RebalancingSetTokenFactoryDeployment>().SendRequestAsync(rebalancingSetTokenFactoryDeployment);
        }

        public static async Task<RebalancingSetTokenFactoryService> DeployContractAndGetServiceAsync(Nethereum.Web3.Web3 web3, RebalancingSetTokenFactoryDeployment rebalancingSetTokenFactoryDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, rebalancingSetTokenFactoryDeployment, cancellationTokenSource);
            return new RebalancingSetTokenFactoryService(web3, receipt.ContractAddress);
        }

        protected Nethereum.Web3.Web3 Web3{ get; }

        public ContractHandler ContractHandler { get; }

        public RebalancingSetTokenFactoryService(Nethereum.Web3.Web3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }

        public Task<BigInteger> MinimumTimeToPivotQueryAsync(MinimumTimeToPivotFunction minimumTimeToPivotFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<MinimumTimeToPivotFunction, BigInteger>(minimumTimeToPivotFunction, blockParameter);
        }

        
        public Task<BigInteger> MinimumTimeToPivotQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<MinimumTimeToPivotFunction, BigInteger>(null, blockParameter);
        }

        public Task<string> CreateSetRequestAsync(CreateSetFunction createSetFunction)
        {
             return ContractHandler.SendRequestAsync(createSetFunction);
        }

        public Task<TransactionReceipt> CreateSetRequestAndWaitForReceiptAsync(CreateSetFunction createSetFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(createSetFunction, cancellationToken);
        }

        public Task<string> CreateSetRequestAsync(List<string> components, List<BigInteger> units, BigInteger naturalUnit, byte[] name, byte[] symbol, byte[] callData)
        {
            var createSetFunction = new CreateSetFunction();
                createSetFunction.Components = components;
                createSetFunction.Units = units;
                createSetFunction.NaturalUnit = naturalUnit;
                createSetFunction.Name = name;
                createSetFunction.Symbol = symbol;
                createSetFunction.CallData = callData;
            
             return ContractHandler.SendRequestAsync(createSetFunction);
        }

        public Task<TransactionReceipt> CreateSetRequestAndWaitForReceiptAsync(List<string> components, List<BigInteger> units, BigInteger naturalUnit, byte[] name, byte[] symbol, byte[] callData, CancellationTokenSource cancellationToken = null)
        {
            var createSetFunction = new CreateSetFunction();
                createSetFunction.Components = components;
                createSetFunction.Units = units;
                createSetFunction.NaturalUnit = naturalUnit;
                createSetFunction.Name = name;
                createSetFunction.Symbol = symbol;
                createSetFunction.CallData = callData;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(createSetFunction, cancellationToken);
        }

        public Task<BigInteger> MaximumTimeToPivotQueryAsync(MaximumTimeToPivotFunction maximumTimeToPivotFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<MaximumTimeToPivotFunction, BigInteger>(maximumTimeToPivotFunction, blockParameter);
        }

        
        public Task<BigInteger> MaximumTimeToPivotQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<MaximumTimeToPivotFunction, BigInteger>(null, blockParameter);
        }

        public Task<string> RebalanceComponentWhitelistQueryAsync(RebalanceComponentWhitelistFunction rebalanceComponentWhitelistFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<RebalanceComponentWhitelistFunction, string>(rebalanceComponentWhitelistFunction, blockParameter);
        }

        
        public Task<string> RebalanceComponentWhitelistQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<RebalanceComponentWhitelistFunction, string>(null, blockParameter);
        }

        public Task<BigInteger> MinimumRebalanceIntervalQueryAsync(MinimumRebalanceIntervalFunction minimumRebalanceIntervalFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<MinimumRebalanceIntervalFunction, BigInteger>(minimumRebalanceIntervalFunction, blockParameter);
        }

        
        public Task<BigInteger> MinimumRebalanceIntervalQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<MinimumRebalanceIntervalFunction, BigInteger>(null, blockParameter);
        }

        public Task<BigInteger> MaximumNaturalUnitQueryAsync(MaximumNaturalUnitFunction maximumNaturalUnitFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<MaximumNaturalUnitFunction, BigInteger>(maximumNaturalUnitFunction, blockParameter);
        }

        
        public Task<BigInteger> MaximumNaturalUnitQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<MaximumNaturalUnitFunction, BigInteger>(null, blockParameter);
        }

        public Task<BigInteger> MinimumNaturalUnitQueryAsync(MinimumNaturalUnitFunction minimumNaturalUnitFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<MinimumNaturalUnitFunction, BigInteger>(minimumNaturalUnitFunction, blockParameter);
        }

        
        public Task<BigInteger> MinimumNaturalUnitQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<MinimumNaturalUnitFunction, BigInteger>(null, blockParameter);
        }

        public Task<string> CoreQueryAsync(CoreFunction coreFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<CoreFunction, string>(coreFunction, blockParameter);
        }

        
        public Task<string> CoreQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<CoreFunction, string>(null, blockParameter);
        }

        public Task<BigInteger> MinimumProposalPeriodQueryAsync(MinimumProposalPeriodFunction minimumProposalPeriodFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<MinimumProposalPeriodFunction, BigInteger>(minimumProposalPeriodFunction, blockParameter);
        }

        
        public Task<BigInteger> MinimumProposalPeriodQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<MinimumProposalPeriodFunction, BigInteger>(null, blockParameter);
        }
    }
}
