namespace UFlow.Addon.ECS.Core.Runtime {
    internal readonly struct EntityDisabledEvent {
        public readonly Entity entity;

        public EntityDisabledEvent(in Entity entity) {
            this.entity = entity;
        }
    }

    public delegate void EntityDisabledHandler(in Entity entity);
}