//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Trakx.Data.Market.Server.Hubs
//{
//    public class IndexDefinitionDto
//    {
//        [MessagePack.Key()]
//        public Guid Id { get; set; }

//        /// <summary>
//        /// Symbol of the token associated with this index (ex: L1MKC005).
//        /// </summary>
//        [Message(50)]
//        public string Symbol { get; set; }

//        /// <summary>
//        /// Name (long) given to the index (ex: Top 5 Market Cap)
//        /// </summary>
//        [Required]
//        [MaxLength(512)]
//        public string Name { get; set; }

//        /// <summary>
//        /// A brief explanation of the index and the choice of components it contains.
//        /// </summary>
//        [Required]
//        public string Description { get; set; }

//        /// <summary>
//        /// If the index was created, the address at which the corresponding smart contract
//        /// can be found on chain.
//        /// </summary>
//        [MaxLength(256)]
//        public string Address { get; set; }

//        /// <summary>
//        /// List of the components contained in the index.
//        /// </summary>
//        public List<ComponentDefinitionDto> ComponentDefinitions { get; set; }

//        /// <summary>
//        /// Net Asset Value of the index at creation time.
//        /// </summary>
//        public IndexValuationDto InitialValuation { get; set; }

//        /// <summary>
//        /// Expressed as a power of 10, it represents the minimal amount of the token
//        /// representing the index that one can buy. 
//        /// </summary>
//        public int NaturalUnit { get; set; }

//        /// <summary>
//        /// Date at which the index was created.
//        /// </summary>
//        public DateTime? CreationDate { get; set; }
//    }
//}
