namespace Trakx.Data.Common.Interfaces.Index
{
    public interface IComponentWeight
    {
        IComponentDefinition ComponentDefinition { get; }
        decimal Weight { get; }
    }
}
