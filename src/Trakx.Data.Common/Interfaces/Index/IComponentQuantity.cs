namespace Trakx.Data.Common.Interfaces.Index
{
    /// <summary>
    /// Component definition, attached to the index for which it is a component
    /// </summary>
    public interface IComponentQuantity
    {
        /// <summary>
        /// Details of the component used in the index.
        /// </summary>
        IComponentDefinition ComponentDefinition { get; }

        /// <summary>
        /// Units of the component contained in each unit of the index containing it. This is
        /// always expressed in the smallest unit of the component's currency and scaled to take
        /// into account the <see cref="IIndexDefinition.NaturalUnit"/>.
        /// </summary>
        decimal Quantity { get; }
    }

    //public static class ComponentQuantityExtensions
    //{
    //    public static string GetId(this IComponentQuantity componentQuantity, IIndexComposition indexComopositi)
    //    {
    //        return $"{indexDefinition.IndexDefinition}|{componentQuantity.ComponentDefinition.Symbol}"
    //    }
    //}
}