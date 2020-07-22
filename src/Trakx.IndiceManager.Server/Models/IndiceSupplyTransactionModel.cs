using System;
using System.Linq;
using Trakx.Common.Core;
using Trakx.Common.Interfaces.Indice;
using Trakx.Common.Interfaces.Transaction;

namespace Trakx.IndiceManager.Server.Models
{
    public class IndiceSupplyTransactionModel
    {
        public IndiceSupplyTransactionModel()
        {
            
        }

        public IndiceSupplyTransactionModel(IIndiceSupplyTransaction transaction)
        {
            SenderAddress = transaction.SenderAddress;
            CreationTimestamp = transaction.CreationTimestamp;
            TransactionType = transaction.TransactionType;
            IndiceQuantity = transaction.Quantity;
            User = transaction.User;
            EthereumBlockId = transaction.EthereumBlockId;
            TransactionHash = transaction.TransactionHash;
            IndiceComposition = new IndiceCompositionModel( transaction.IndiceComposition);
        }
        public DateTime CreationTimestamp { get; set; }

        public IndiceCompositionModel IndiceComposition { get; set; }

        public SupplyTransactionType? TransactionType { get; set; }

        public decimal IndiceQuantity { get; set; }

        public string User { get; set; }

        public string? TransactionHash { get; set; }

        public string SenderAddress { get; set; }

        public int? EthereumBlockId { get; set; }

        public bool IsValid()
        {
            return IndiceComposition != default 
                   && !string.IsNullOrWhiteSpace(User)
                   && !string.IsNullOrWhiteSpace(SenderAddress)
                   && CreationTimestamp != default
                   && TransactionType != null 
                   && IndiceQuantity > 0m;
        }

        public IIndiceSupplyTransaction ConvertToIIndiceSupplyTransaction()
        {
            var indiceDefinition = new IndiceDefinition(IndiceComposition.IndiceDetail.Symbol, IndiceComposition.IndiceDetail.Name, 
                IndiceComposition.IndiceDetail.Description, IndiceComposition.IndiceDetail.NaturalUnit, IndiceComposition.IndiceDetail.Address, 
                IndiceComposition.IndiceDetail.CreationDate);

            var components = IndiceComposition.Components.Select(c =>
                new ComponentQuantity(new ComponentDefinition(c.Address, c.Name, c.Symbol, c.CoinGeckoId, c.Decimals),
                    Convert.ToUInt64(c.Quantity), IndiceComposition.IndiceDetail.NaturalUnit)).ToList<IComponentQuantity>();

            var composition = new IndiceComposition(indiceDefinition, components,IndiceComposition.Version,IndiceComposition.CreationDate,IndiceComposition.Address);
            var supplyTransaction = new IndiceSupplyTransaction(CreationTimestamp, composition,TransactionType,IndiceQuantity,SenderAddress,User,TransactionHash,EthereumBlockId);

            return supplyTransaction;
        }
    }
}
