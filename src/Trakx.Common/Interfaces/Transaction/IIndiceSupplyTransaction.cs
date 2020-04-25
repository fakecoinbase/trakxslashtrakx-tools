using System;
using Trakx.Common.Core;
using Trakx.Common.Interfaces.Indice;

namespace Trakx.Common.Interfaces.Transaction
{
    /// <summary>
    /// Represents the creation of an indice via a transaction by a user.
    /// </summary>
    public interface IIndiceSupplyTransaction
    {
        /// <summary>
        /// The Timestamp of the transaction.
        /// </summary>
        public DateTime CreationTimestamp { get;}

        /// <summary>
        /// The <see cref="IIndiceComposition"/> that was traded in the transaction.
        /// </summary>
        public IIndiceComposition IndiceComposition { get; }

        /// <summary>
        /// The type of the transaction : Issue or Redeem of indices.
        /// This is an enum <see cref="TransactionType"/>
        /// </summary>
        public TransactionType TransactionType { get;}

        /// <summary>
        /// The block Id of the transaction on the ethereum mainnet.
        /// </summary>
        public int? EthereumBlockId { get;}

        /// <summary>
        /// The Hash of the transaction.
        /// </summary>
        public string? TransactionHash { get; }


        /// <summary>
        /// The quantity of indices that were issued or redeemed in the transaction.
        /// </summary>
        public decimal Quantity { get;  } 

        /// <summary>
        /// The ethereum address from where the user send his tokens.
        /// </summary>
        public string SenderAddress { get; }


        /// <summary>
        /// The identity of the user who executed the transaction.
        /// </summary>
        public string User { get;  } 
    }
    public enum TransactionType
        {
            Redeem,
            Issue
        }
}
