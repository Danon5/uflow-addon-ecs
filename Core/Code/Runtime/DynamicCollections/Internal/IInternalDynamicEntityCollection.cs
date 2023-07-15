namespace UFlow.Addon.Ecs.Core.Runtime {
    internal interface IInternalDynamicEntityCollection {
        void Add(in Entity entity);
        void Remove(in Entity entity);
    }
}