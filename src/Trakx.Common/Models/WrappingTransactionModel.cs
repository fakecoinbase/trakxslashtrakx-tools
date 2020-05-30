using System;
using Trakx.Common.Interfaces.Transaction;

namespace Trakx.Common.Models
{
    public class WrappingTransactionModel
    {
        public WrappingTransactionModel()
        {
            
        }

        public WrappingTransactionModel(IWrappingTransaction transaction)
        {
            SenderAddress = transaction.SenderAddress;
            ReceiverAddress = transaction.ReceiverAddress;
            Date = transaction.TimeStamp;
            FromCurrency = transaction.FromCurrency;
            ToCurrency = transaction.ToCurrency;
            EthereumBlockId = transaction.EthereumBlockId;
            NativeBlockId = transaction.NativeChainBlockId;
            EthereumTransactionHash=transaction.NativeChainTransactionHash;
            NativeChainTransactionHash= transaction.EthereumTransactionHash;
        }
        public string TrakxAddress { get; set; }

        public string SenderAddress { get; set; }

        public string ReceiverAddress { get; set; }

        public DateTime Date { get; set; }

        public string FromCurrency { get; set; }

        public string ToCurrency { get; set; }

        public int? EthereumBlockId { get; set; }

        public int? NativeBlockId { get; set; }

        public string? EthereumTransactionHash { get; set; }

        public string? NativeChainTransactionHash { get; set; }
    }
}
