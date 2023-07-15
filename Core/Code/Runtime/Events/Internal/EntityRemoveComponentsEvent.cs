namespace UFlow.Addon.Ecs.Core.Runtime {
    internal readonly struct EntityRemoveComponentsEvent {
        public readonly Entity entity;

        public EntityRemoveComponentsEvent(in Entity entity) {
            this.entity = entity;
        }
    }

    internal delegate void EntityRemoveComponentsHandler(in Entity entity);
}