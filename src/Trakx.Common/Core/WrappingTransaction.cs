using System;
using System.Collections.Generic;
using System.Text;
using Trakx.Common.Interfaces.Transaction;

namespace Trakx.Common.Core
{
    class WrappingTransaction :IWrappingTransaction
    {
        public WrappingTransaction(DateTime timeStamp, string fromCurrency, string toCurrency,
            TransactionState transactionState, string? ethereumTransactionHash, string? nativeChainTransactionHash,
            int? nativeChainBlockId,  int? ethereumBlockId,int amountSent, string senderAddress,
            string receiverAddress, string user)
        {
            TimeStamp = timeStamp;
            FromCurrency = fromCurrency;
            ToCurrency = toCurrency;
            TransactionState = transactionState;
            EthereumBlockId = ethereumBlockId;
            EthereumTransactionHash = ethereumTransactionHash;
            NativeChainBlockId = nativeChainBlockId;
            NativeChainTransactionHash = nativeChainTransactionHash;
            AmountSent = amountSent;
            SenderAddress = senderAddress;
            ReceiverAddress = receiverAddress;
            User = user;
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
        public int AmountSent { get; }

        /// <inheritdoc />
        public string SenderAddress { get; }

        /// <inheritdoc />
        public string ReceiverAddress { get; }

        /// <inheritdoc />
        public string User { get; }
        #endregion
    }
}
