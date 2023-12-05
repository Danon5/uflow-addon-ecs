namespace UFlow.Addon.ECS.Core.Runtime {
    public interface IDynamicEntityCollection {
        int EntityCount { get; }
        void ResetCache();
    }
}