namespace UFlow.Addon.Entities.Core.Runtime {
    internal readonly struct EntityComponentRemovingEvent<T> {
        public readonly Entity entity;

        public EntityComponentRemovingEvent(in Entity entity) {
            this.entity = entity;
        }
    }

    public delegate void EntityComponentRemovingHandler<T>(in Entity entity, ref T component);
}