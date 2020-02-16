using Trakx.Data.Common.Interfaces.Index;

namespace Trakx.Data.Market.Server.Models
{
    public partial class ComponentModel
    {
        public string Address { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public long Decimals { get; set; }
        public decimal Quantity { get; set; }
        public string IconUrl { get; set; }

        public static ComponentModel FromIComponent(IComponentQuantity componentQuantity)
        {
            var result = new ComponentModel()
            {
                Address = componentQuantity.ComponentDefinition.Address,
                Decimals = componentQuantity.ComponentDefinition.Decimals,
                Name = componentQuantity.ComponentDefinition.Name,
                Symbol = componentQuantity.ComponentDefinition.Symbol.ToUpper(),
                Quantity = componentQuantity.Quantity
            };
            return result;
        }
    }
}