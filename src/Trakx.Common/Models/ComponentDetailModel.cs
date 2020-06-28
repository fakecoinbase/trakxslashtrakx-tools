using Ardalis.GuardClauses;
using System.ComponentModel.DataAnnotations;
using Trakx.Common.Core;
using Trakx.Common.Interfaces.Indice;

namespace Trakx.Common.Models
{
    public class ComponentDetailModel
    {
        // Non-nullable field is uninitialized. Consider declaring as nullable.
        // The empty constructor is for serialisation only.
        #pragma warning disable CS8618
        public ComponentDetailModel() { }
        #pragma warning restore CS8618
        
        public ComponentDetailModel(IComponentQuantity quantity)
        {
            Guard.Against.Null(quantity.ComponentDefinition, nameof(quantity.ComponentDefinition));

            Address = quantity.ComponentDefinition.Address;
            Symbol = quantity.ComponentDefinition.Symbol;
            Quantity = quantity.Quantity;
            Name = quantity.ComponentDefinition.Name;
            Decimals = quantity.ComponentDefinition.Decimals;
            CoinGeckoId = quantity.ComponentDefinition.CoinGeckoId;
        }

        public ComponentDetailModel(IComponentDefinition componentDefinition)
        {
            Address = componentDefinition.Address;
            Symbol = componentDefinition.Symbol;
            Name = componentDefinition.Name;
            Decimals = componentDefinition.Decimals;
            CoinGeckoId = componentDefinition.CoinGeckoId;
        }

        public string Address { get; set; }

        [Required]
        public string Symbol { get; set; }
        public decimal? Quantity { get; set; }
        public string? Name { get; set; }

        public string? CoinGeckoId { get; set; }

        public ushort Decimals { get; set; }
        public decimal? UsdcValue { get; set; }

        [Required, Range(1e-6, 1)]
        public decimal? Weight { get; set; }

        public bool IsValid()
        {
            if (CoinGeckoId != null && Name != null && !string.IsNullOrEmpty(Symbol) &&
                !string.IsNullOrEmpty(Address) && Decimals != default)
                return true;

            return false;
        }

        public IComponentDefinition ConvertToIComponentDefinition()
        {
            return new ComponentDefinition(Address, Name, Symbol, CoinGeckoId, Decimals);
        }
    }
}