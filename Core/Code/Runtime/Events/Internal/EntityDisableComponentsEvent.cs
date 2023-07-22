namespace UFlow.Addon.ECS.Core.Runtime {
    internal readonly struct EntityDisableComponentsEvent {
        public readonly Entity entity;

        public EntityDisableComponentsEvent(in Entity entity) {
            this.entity = entity;
        }
    }

    internal delegate void EntityDisableComponentsHandler(in Entity entity);
}