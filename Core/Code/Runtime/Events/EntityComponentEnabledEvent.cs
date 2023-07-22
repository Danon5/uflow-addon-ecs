namespace UFlow.Addon.ECS.Core.Runtime {
    internal readonly struct EntityComponentEnabledEvent<T> {
        public readonly Entity entity;
        
        public EntityComponentEnabledEvent(in Entity entity) {
            this.entity = entity;
        }
    }

    public delegate void EntityComponentEnabledHandler<T>(in Entity entity, ref T component);
}