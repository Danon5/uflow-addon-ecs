namespace UFlow.Addon.Entities.Core.Runtime {
    internal readonly struct EntityDisabledEvent {
        public readonly Entity entity;

        public EntityDisabledEvent(in Entity entity) {
            this.entity = entity;
        }
    }

    public delegate void EntityDisabledHandler(in Entity entity);
}