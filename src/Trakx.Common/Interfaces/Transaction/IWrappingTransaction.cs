using System;

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
        DateTime TimeStamp { get; }

        /// <summary>
        /// The symbol of the currency that was sent in the transaction.
        /// </summary>
        string FromCurrency { get; }

        /// <summary>
        /// The symbol of the currency that the user wants to receive in the transaction.
        /// </summary>
        string ToCurrency { get; }

        /// <summary>
        /// The State of the transaction : Pending / Complete / Failed
        /// This is an enum <see cref="TransactionState"/>
        /// </summary>
        TransactionState TransactionState { get; }

        /// <summary>
        /// The hash of the transaction on the ethereum mainnet.
        /// </summary>
        string? EthereumTransactionHash { get; }

        /// <summary>
        /// The hash of the transaction on the native chain.
        /// </summary>
        string? NativeChainTransactionHash { get; }

        /// <summary>
        /// The blockId of the transaction on the ethereum mainnet.
        /// </summary>
        int? EthereumBlockId { get; }

        /// <summary>
        /// The blockId of the transaction on the native chain.
        /// </summary>
        int? NativeChainBlockId { get; }

        /// <summary>
        /// The amount sent in the transaction, that means the amount that the user wants to convert.
        /// </summary>
        decimal Amount { get; }

        /// <summary>
        /// The address from where the user initiates the transaction. Could be an ethereum address or the address from the native token.
        /// </summary>
        string SenderAddress { get; }

        /// <summary>
        /// The address on which the user wants to receives his convert tokens.
        /// </summary>
        string ReceiverAddress { get; }

        /// <summary>
        /// The identity of the user who executed the transaction.
        /// </summary>
        string User { get; }
    }

    public enum TransactionState
    {
        Pending,
        Complete,
        Failed
    }
}
