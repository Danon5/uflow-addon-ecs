namespace UFlow.Addon.ECS.Core.Runtime {
    internal readonly struct EntityEnabledEvent {
        public readonly Entity entity;

        public EntityEnabledEvent(in Entity entity) {
            this.entity = entity;
        }
    }

    public delegate void EntityEnabledHandler(in Entity entity);
}