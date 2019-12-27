using Trakx.Data.Models.Index;

namespace Trakx.Data.Market.Server.Models.Index
{
    public class ComponentDefinitionWithIcon : ComponentDefinition
    {
        public string IconUrl { get; }

        public ComponentDefinitionWithIcon(string iconUrl)
        {
            IconUrl = iconUrl;
        }
    }

    public static class ComponentDefinitionExtension
    {
        public static ComponentDefinitionWithIcon ToComponentDefinitionWithIcon(this ComponentDefinition definition, string iconUrl)
        {
            return new ComponentDefinitionWithIcon(iconUrl)
            {
                Id = definition.Id,
                InitialValuation = new ComponentValuation()
                {
                    ComponentDefinition = definition.InitialValuation.ComponentDefinition,
                    Id = definition.InitialValuation.Id,
                    Price = definition.InitialValuation.Price,
                    QuoteCurrency = definition.InitialValuation.QuoteCurrency,
                    TimeStamp = definition.InitialValuation.TimeStamp,
                    Value = definition.InitialValuation.Value
                },
                Name = definition.Name,
                Address = definition.Address,
                Decimals = definition.Decimals,
                Symbol = definition.Symbol,
                Quantity = definition.Quantity
            };
        }
    }
}