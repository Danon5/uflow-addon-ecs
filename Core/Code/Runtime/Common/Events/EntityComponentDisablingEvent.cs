namespace UFlow.Addon.Entities.Core.Runtime {
    internal struct EntityComponentDisablingEvent<T> {
        public readonly Entity entity;

        public EntityComponentDisablingEvent(in Entity entity) {
            this.entity = entity;
        }
    }

    public delegate void EntityComponentDisablingHandler<T>(in Entity entity, ref T component);
}