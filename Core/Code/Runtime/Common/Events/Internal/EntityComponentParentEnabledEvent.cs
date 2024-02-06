namespace UFlow.Addon.Entities.Core.Runtime {
    internal readonly struct EntityComponentParentEnabledEvent<T> {
        public readonly Entity entity;

        public EntityComponentParentEnabledEvent(in Entity entity) {
            this.entity = entity;
        }
    }

    internal delegate void EntityComponentParentEnabledHandler<T>(in Entity entity, ref T component);
}