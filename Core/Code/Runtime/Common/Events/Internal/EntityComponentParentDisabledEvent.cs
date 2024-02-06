namespace UFlow.Addon.Entities.Core.Runtime {
    internal readonly struct EntityComponentParentDisabledEvent<T> {
        public readonly Entity entity;

        public EntityComponentParentDisabledEvent(in Entity entity) {
            this.entity = entity;
        }
    }

    internal delegate void EntityComponentParentDisabledHandler<T>(in Entity entity, ref T component);
}