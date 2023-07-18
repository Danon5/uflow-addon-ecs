namespace UFlow.Addon.Ecs.Core.Runtime {
    internal interface IInternalDynamicEntityCollection {
        void EnsureAdded(in Entity entity);
        void EnsureRemoved(in Entity entity);
    }
}