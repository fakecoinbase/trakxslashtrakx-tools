using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Trakx.Common.Interfaces.Transaction;

namespace Trakx.Persistence.DAO
{
    public class WrappingTransactionDao : IWrappingTransaction
    {
        public WrappingTransactionDao(IWrappingTransaction transaction)
        {
            TimeStamp = transaction.TimeStamp;
            FromCurrency = transaction.FromCurrency;
            ToCurrency = transaction.ToCurrency;
            TransactionState = transaction.TransactionState;
            EthereumBlockId = transaction.EthereumBlockId;
            EthereumTransactionHash = transaction.EthereumTransactionHash;
            NativeChainBlockId = transaction.NativeChainBlockId;
            NativeChainTransactionHash = transaction.NativeChainTransactionHash;
            Amount = transaction.Amount;
            SenderAddress = transaction.SenderAddress;
            ReceiverAddress = transaction.ReceiverAddress;
            User = transaction.User;
            TransactionType = transaction.TransactionType;
        }

        public WrappingTransactionDao(DateTime timeStamp, string fromCurrency, string toCurrency,
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
        [Required, Column(TypeName = "decimal(38, 18)")] 
        public decimal Amount { get; set; }

        /// <inheritdoc />
        [Required, MaxLength(256)]
        public string SenderAddress { get; set; }

        /// <inheritdoc />
        [Required, MaxLength(256)]
        public string ReceiverAddress { get; set; }

        /// <inheritdoc />
        [Required]
        public string User { get; set; }

        /// <inheritdoc />
        [Required]
        public TransactionType TransactionType { get; set; }
    }
}
