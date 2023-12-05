namespace UFlow.Addon.ECS.Core.Runtime {
    public interface IPreCleanupSystem : ISystem {
        void PreCleanup();
    }
}