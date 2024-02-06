namespace UFlow.Addon.Entities.Core.Runtime {
    internal readonly struct EntityComponentRemovedEvent<T> {
        public readonly Entity entity;
        public readonly T component;

        public EntityComponentRemovedEvent(in Entity entity, in T component) {
            this.entity = entity;
            this.component = component;
        }
    }
    
    public delegate void EntityComponentRemovedHandler<T>(in Entity entity, in T component);
}