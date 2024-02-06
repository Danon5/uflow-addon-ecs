namespace UFlow.Addon.Entities.Core.Runtime {
    internal readonly struct EntityComponentDisabledEvent<T> {
        public readonly Entity entity;
        
        public EntityComponentDisabledEvent(in Entity entity) {
            this.entity = entity;
        }
    }

    public delegate void EntityComponentDisabledHandler<T>(in Entity entity, ref T component);
}