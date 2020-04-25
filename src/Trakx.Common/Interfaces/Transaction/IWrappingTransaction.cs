using System;
using System.Collections.Generic;
using System.Text;

namespace Trakx.Common.Interfaces.Transaction
{
    /// <summary>
    /// Represents a transaction made by a user in order to wrap or unwrap tokens, that means to convert native tokens to ERC20 tokens or the reverse.
    /// </summary>
    public interface IWrappingTransaction
    {
        /// <summary>
        /// The time when the transaction was initiated.
        /// </summary>
        public DateTime TimeStamp { get;  }

        /// <summary>
        /// The symbol of the currency that was sent in the transaction.
        /// </summary>
        public string FromCurrency { get; }

        /// <summary>
        /// The symbol of the currency that the user wants to receive in the transaction.
        /// </summary>
        public string ToCurrency { get; }

        /// <summary>
        /// The State of the transaction : Pending / Complete / Failed
        /// This is an enum <see cref="TransactionState"/>
        /// </summary>
        public TransactionState TransactionState { get;}

        /// <summary>
        /// The hash of the transaction on the ethereum mainnet.
        /// </summary>
        public string? EthereumTransactionHash { get; }

        /// <summary>
        /// The hash of the transaction on the native chain.
        /// </summary>
        public string? NativeChainTransactionHash { get;  }

        /// <summary>
        /// The blockId of the transaction on the ethereum mainnet.
        /// </summary>
        public int? EthereumBlockId { get; }

        /// <summary>
        /// The blockId of the transaction on the native chain.
        /// </summary>
        public int? NativeChainBlockId { get; }

        /// <summary>
        /// The amount sent in the transaction, that means the amount that the user wants to convert.
        /// </summary>
        public int AmountSent { get;  }

        /// <summary>
        /// The address from where the user initiates the transaction. Could be an ethereum address or the address from the native token.
        /// </summary>
        public string SenderAddress { get;  } 

        /// <summary>
        /// The address on which the user wants to receives his convert tokens.
        /// </summary>
        public string ReceiverAddress { get;  }

        /// <summary>
        /// The identity of the user who executed the transaction.
        /// </summary>
        public string User { get;  } 
    }
    public enum TransactionState
    {
        Pending,
        Complete,
        Failed
    }
}
