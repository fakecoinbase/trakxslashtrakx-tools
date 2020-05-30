namespace Trakx.Common.Interfaces.Indice
{
    /// <summary>
    /// Component definition, attached to the indice for which it is a component
    /// </summary>
    public interface IComponentQuantity
    {
        /// <summary>
        /// Details of the component used in the indice.
        /// </summary>
        IComponentDefinition ComponentDefinition { get; }

        /// <summary>
        /// Units of the component contained in each unit of the indice containing it. This is
        /// always expressed in the smallest unit of the component's currency and scaled to take
        /// into account the <see cref="IIndiceDefinition.NaturalUnit"/>.
        /// </summary>
        decimal Quantity { get; }
    }

    public static class ComponentQuantityExtensions
    {
        public static string GetId(this IComponentQuantity componentQuantity, IIndiceComposition indiceComposition)=>
            $"{indiceComposition.GetCompositionId()}|{componentQuantity.ComponentDefinition.Symbol}";
    }
}