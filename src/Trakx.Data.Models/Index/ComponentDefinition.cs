using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Trakx.Data.Models.Converters;

namespace Trakx.Data.Models.Index
{
    public class ComponentDefinition
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ComponentDefinitionId { get; set; }
        
        /// <summary>
        /// Address of the smart contract defining the ERC20 contract of the component.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Common (longer) name for the component (ex: Bitcoin, Ethereum, Litecoin).
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Symbol of the token associated with this component (ex: ETH, BTC, LTC).
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// Number of decimals by which 1 unit of the ERC20 can be divided.
        /// </summary>
        public int Decimals { get; set; }

        /// <summary>
        /// Units of the component contained in each unit of the index containing it. This is
        /// always expressed in the smallest unit of the component's currency.
        /// </summary>
        [JsonConverter(typeof(JsonStringBigIntegerConverter))]
        public ulong Quantity { get; set; }

        /// <summary>
        /// Valuation of the component at the time of creation
        /// </summary>
        public ComponentValuation InitialValuation { get; set; }

        public ComponentDefinition() { }

        public ComponentDefinition(string address,
            string name,
            string symbol,
            int decimals,
            ulong quantity,
            decimal initialPrice,
            string quoteCurrency,
            DateTime valuationDateTime)
        {
            Address = address;
            Name = name;
            Symbol = symbol;
            Decimals = decimals;
            Quantity = quantity;
            InitialValuation = new ComponentValuation(this, quoteCurrency, initialPrice, valuationDateTime);
        }
    }
}