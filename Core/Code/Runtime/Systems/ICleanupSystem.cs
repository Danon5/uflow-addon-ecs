namespace UFlow.Addon.Ecs.Core.Runtime {
    public interface ICleanupSystem : ISystem {
        void Cleanup();
    }
}