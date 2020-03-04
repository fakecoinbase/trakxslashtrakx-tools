using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.ContractHandlers;
using System.Threading;
using Nethereum.Web3;
using Trakx.Contracts.Set.SetTokenFactory.ContractDefinition;

namespace Trakx.Contracts.Set.SetTokenFactory
{
    public partial class SetTokenFactoryService
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(IWeb3 web3, SetTokenFactoryDeployment setTokenFactoryDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<SetTokenFactoryDeployment>().SendRequestAndWaitForReceiptAsync(setTokenFactoryDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(IWeb3 web3, SetTokenFactoryDeployment setTokenFactoryDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<SetTokenFactoryDeployment>().SendRequestAsync(setTokenFactoryDeployment);
        }

        public static async Task<SetTokenFactoryService> DeployContractAndGetServiceAsync(IWeb3 web3, SetTokenFactoryDeployment setTokenFactoryDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, setTokenFactoryDeployment, cancellationTokenSource);
            return new SetTokenFactoryService(web3, receipt.ContractAddress);
        }

        protected IWeb3 Web3{ get; }

        public ContractHandler ContractHandler { get; }

        public SetTokenFactoryService(IWeb3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
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

        public Task<string> CoreQueryAsync(CoreFunction coreFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<CoreFunction, string>(coreFunction, blockParameter);
        }

        
        public Task<string> CoreQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<CoreFunction, string>(null, blockParameter);
        }
    }
}
