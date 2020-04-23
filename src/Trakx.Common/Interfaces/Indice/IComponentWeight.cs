namespace Trakx.Common.Interfaces.Indice
{
    public interface IComponentWeight
    {
        IComponentDefinition ComponentDefinition { get; }
        decimal Weight { get; }
    }
}
