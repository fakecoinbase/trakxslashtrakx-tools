namespace Trakx.Data.Models.Index
{
    public class ComponentPriced : ComponentDefinition
    {
        public ComponentValuation CurrentValuation { get; set; }

        public ComponentPriced() {}

        public ComponentPriced(ComponentDefinition definition,
            ComponentValuation currentValuation)
        {
            Address = definition.Address;
            Quantity = definition.Quantity;
            Decimals = definition.Decimals;
            Name = definition.Name;
            Symbol = definition.Symbol;
            InitialValuation = definition.InitialValuation;
            CurrentValuation = currentValuation;
        }
    }
}