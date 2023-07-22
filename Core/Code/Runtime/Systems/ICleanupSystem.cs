namespace UFlow.Addon.ECS.Core.Runtime {
    public interface ICleanupSystem : ISystem {
        void Cleanup();
    }
}