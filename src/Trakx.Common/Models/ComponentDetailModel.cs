using Ardalis.GuardClauses;
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

        public string Address { get; set; }
        public string Symbol { get; set; }
        public decimal? Quantity { get; set; }
        public string? Name { get; set; }

        public string? CoinGeckoId { get; set; }

        public ushort Decimals { get; set; }
    }
}