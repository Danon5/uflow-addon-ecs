namespace UFlow.Addon.Ecs.Core.Runtime {
    public interface IDynamicEntityCollection {
        int EntityCount { get; }
        void ResetCache();
    }
}