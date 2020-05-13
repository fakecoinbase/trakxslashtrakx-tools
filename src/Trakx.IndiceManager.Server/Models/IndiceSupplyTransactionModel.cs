using System;
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
                   && TransactionType != default 
                   && IndiceQuantity > 0m;
        }
    }
}
