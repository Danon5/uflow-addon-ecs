namespace UFlow.Addon.Entities.Core.Runtime {
    internal readonly struct EntityDestroyingEvent {
        public readonly Entity entity;

        public EntityDestroyingEvent(in Entity entity) {
            this.entity = entity;
        }
    }

    public delegate void EntityDestroyingHandler(in Entity entity);
}