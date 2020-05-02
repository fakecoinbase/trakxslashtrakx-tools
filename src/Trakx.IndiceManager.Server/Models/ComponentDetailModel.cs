using Ardalis.GuardClauses;
using Trakx.Common.Interfaces.Indice;

namespace Trakx.IndiceManager.Server.Models
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
        }

        public string Address { get; set; }
        public string Symbol { get; set; }
        public decimal? Quantity { get; set; }
    }
}