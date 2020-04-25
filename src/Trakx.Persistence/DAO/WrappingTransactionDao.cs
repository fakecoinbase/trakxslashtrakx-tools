using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Trakx.Common.Interfaces.Transaction;

namespace Trakx.Persistence.DAO
{
    public class WrappingTransactionDao :IWrappingTransaction
    {
        public WrappingTransactionDao(DateTime timeStamp, string fromCurrency, string toCurrency,
            TransactionState transactionState, string? ethereumTransactionHash, string? nativeChainTransactionHash,
            int? nativeChainBlockId, int? ethereumBlockId, int amountSent, string senderAddress,
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

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public uint Id { get; set; }

        /// <inheritdoc />
        [Required]
        public DateTime TimeStamp { get; set; }

        /// <inheritdoc />
        [Required, MaxLength(50)]
        public string FromCurrency { get; set; }

        /// <inheritdoc />
        [Required,MaxLength(50)]
        public string ToCurrency { get; set; }

        /// <inheritdoc />
        [Required]
        public TransactionState TransactionState { get; set; }

        /// <inheritdoc />
        public string? EthereumTransactionHash { get; set; }

        /// <inheritdoc />
        public string? NativeChainTransactionHash { get; set; }

        /// <inheritdoc />
        public int? NativeChainBlockId { get; set; }

        /// <inheritdoc />
        public int? EthereumBlockId { get; set; }

        /// <inheritdoc />
        public int AmountSent { get; set; }

        /// <inheritdoc />
        [Required, MaxLength(256)]
        public string SenderAddress { get; set; }

        /// <inheritdoc />
        [Required, MaxLength(256)]
        public string ReceiverAddress { get; set; }

        /// <inheritdoc />
        [Required]
        public string User { get; set; }
    }
}
