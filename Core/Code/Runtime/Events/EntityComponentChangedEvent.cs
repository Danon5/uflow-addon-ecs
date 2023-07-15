namespace UFlow.Addon.Ecs.Core.Runtime {
    internal readonly struct EntityComponentChangedEvent<T> {
        public readonly Entity entity;

        public EntityComponentChangedEvent(in Entity entity) {
            this.entity = entity;
        }
    }

    public delegate void EntityComponentChangedHandler<T>(in Entity entity, in T oldValue, ref T newValue);
}