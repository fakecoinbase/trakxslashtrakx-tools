using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Trakx.Common.Interfaces.Indice;
using Trakx.Common.Interfaces.Transaction;

namespace Trakx.Persistence.DAO
{
    public class IndiceSupplyTransactionDao : IIndiceSupplyTransaction
    {
        // Non-nullable field is uninitialized. Consider declaring as nullable.
        // This constructor is for serialisation only
        #pragma warning disable CS8618
        public IndiceSupplyTransactionDao() { }

        public IndiceSupplyTransactionDao(DateTime creationTimestamp, IndiceCompositionDao indiceCompositionDao,
            TransactionType transactionType, decimal quantity, string senderAddress, string user, string? transactionHash, int? ethereumBlockId)
        {
            CreationTimestamp = creationTimestamp;
            IndiceCompositionDao = indiceCompositionDao;
            TransactionType = transactionType;
            Quantity = quantity;
            SenderAddress = senderAddress;
            User = user;
            TransactionHash = transactionHash;
            EthereumBlockId = ethereumBlockId;
        }
        #pragma warning restore CS8618

        
        
        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public uint Id { get; set; }

        /// <inheritdoc />
        [Required]
        public DateTime CreationTimestamp { get; set; }


        /// <inheritdoc />
        [NotMapped]
        public IIndiceComposition IndiceComposition => IndiceCompositionDao;

        [Required] 
        public IndiceCompositionDao IndiceCompositionDao { get; set; }

        /// <inheritdoc />
        [Required]
        public TransactionType TransactionType { get; set; }

        /// <inheritdoc />
        public int? EthereumBlockId { get; set; }

        /// <inheritdoc />
        public string? TransactionHash { get; set; }

        /// <inheritdoc />
        [Required, Column(TypeName = "decimal(38, 18)")]
        public decimal Quantity { get; set; }

        /// <inheritdoc />
        [MaxLength(256)]
        public string SenderAddress { get; set; }


        /// <inheritdoc />
        [Required]
        public string User { get; set; }
    }
}
