namespace UFlow.Addon.ECS.Core.Runtime {
    internal readonly struct EntityDisablingEvent {
        public readonly Entity entity;

        public EntityDisablingEvent(in Entity entity) {
            this.entity = entity;
        }
    }

    public delegate void EntityDisablingHandler(in Entity entity);
}