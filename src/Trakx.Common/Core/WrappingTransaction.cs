using System;
using Trakx.Common.Interfaces.Transaction;

namespace Trakx.Common.Core
{
    public class WrappingTransaction : IWrappingTransaction
    {
        public WrappingTransaction(DateTime timeStamp, string fromCurrency, string toCurrency,
            TransactionState transactionState, string? ethereumTransactionHash, string? nativeChainTransactionHash,
            int? nativeChainBlockId, int? ethereumBlockId, decimal amount, string senderAddress,
            string receiverAddress, string user,TransactionType transactionType)
        {
            TimeStamp = timeStamp;
            FromCurrency = fromCurrency;
            ToCurrency = toCurrency;
            TransactionState = transactionState;
            EthereumBlockId = ethereumBlockId;
            EthereumTransactionHash = ethereumTransactionHash;
            NativeChainBlockId = nativeChainBlockId;
            NativeChainTransactionHash = nativeChainTransactionHash;
            Amount = amount;
            SenderAddress = senderAddress;
            ReceiverAddress = receiverAddress;
            User = user;
            TransactionType = transactionType;
        }

        #region Implementation of IWrappingTransaction

        /// <inheritdoc />
        public DateTime TimeStamp { get; }

        /// <inheritdoc />
        public string FromCurrency { get; }

        /// <inheritdoc />
        public string ToCurrency { get; }

        /// <inheritdoc />
        public TransactionState TransactionState { get; }

        /// <inheritdoc />
        public string? EthereumTransactionHash { get; }

        /// <inheritdoc />
        public string? NativeChainTransactionHash { get; }

        /// <inheritdoc />
        public int? NativeChainBlockId { get; }

        /// <inheritdoc />
        public int? EthereumBlockId { get; }

        /// <inheritdoc />
        public decimal Amount { get; }

        /// <inheritdoc />
        public string SenderAddress { get; }

        /// <inheritdoc />
        public string ReceiverAddress { get; }

        /// <inheritdoc />
        public string User { get; }

        /// <inheritdoc />
        public TransactionType TransactionType { get; }

        #endregion
    }
}
