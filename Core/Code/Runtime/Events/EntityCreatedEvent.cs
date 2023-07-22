namespace UFlow.Addon.ECS.Core.Runtime {
    internal readonly struct EntityCreatedEvent {
        public readonly Entity entity;

        public EntityCreatedEvent(in Entity entity) {
            this.entity = entity;
        }
    }

    public delegate void EntityCreatedHandler(in Entity entity);
}