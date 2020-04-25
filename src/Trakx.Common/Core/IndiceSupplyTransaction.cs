using System;
using Trakx.Common.Interfaces.Indice;
using Trakx.Common.Interfaces.Transaction;

namespace Trakx.Common.Core
{
    public class IndiceSupplyTransaction :IIndiceSupplyTransaction
    {
        public IndiceSupplyTransaction(DateTime creationTimestamp, IIndiceComposition indiceComposition,
            TransactionType transactionType, decimal quantity, string senderAddress, string user,  string? transactionHash, int? ethereumBlockId)
        {
            CreationTimestamp = creationTimestamp;
            IndiceComposition = indiceComposition;
            TransactionType = transactionType;
            Quantity = quantity;
            SenderAddress = senderAddress;
            User = user;
            TransactionHash = transactionHash;
            EthereumBlockId = ethereumBlockId;
        }

        #region Implementation of IIndiceSupplyTransaction

        /// <inheritdoc />
        public DateTime CreationTimestamp { get; }

        /// <inheritdoc />
        public IIndiceComposition IndiceComposition { get; }

        /// <inheritdoc />
        public TransactionType TransactionType { get; }

        /// <inheritdoc />
        public int? EthereumBlockId { get; }

        /// <inheritdoc />
        public string? TransactionHash { get; }


        /// <inheritdoc />
        public decimal Quantity { get; }

        /// <inheritdoc />
        public string SenderAddress { get; }


        /// <inheritdoc />
        public string User { get; }

        #endregion
    }
}
