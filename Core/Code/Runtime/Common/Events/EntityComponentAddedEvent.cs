namespace UFlow.Addon.Entities.Core.Runtime {
    internal readonly struct EntityComponentAddedEvent<T> {
        public readonly Entity entity;

        public EntityComponentAddedEvent(in Entity entity) {
            this.entity = entity;
        }
    }
    
    public delegate void EntityComponentAddedHandler<T>(in Entity entity, ref T component);
}