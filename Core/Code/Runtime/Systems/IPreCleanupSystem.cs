namespace UFlow.Addon.Ecs.Core.Runtime {
    public interface IPreCleanupSystem : ISystem {
        void PreCleanup();
    }
}