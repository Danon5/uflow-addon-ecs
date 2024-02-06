namespace UFlow.Addon.Entities.Core.Runtime {
    public interface IDynamicEntityCollection {
        int EntityCount { get; }
        void ResetCache();
    }
}