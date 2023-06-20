namespace UFlow.Core.Runtime {
    internal readonly struct WorldDestroyedEvent {
        public readonly ushort worldId;

        public WorldDestroyedEvent(ushort worldId) {
            this.worldId = worldId;
        }
    }
    
    internal readonly struct EntityCreatedEvent {
        public readonly int entityId;

        public EntityCreatedEvent(in int entityId) {
            this.entityId = entityId;
        }
    }
    
    internal readonly struct EntityDestroyedEvent {
        public readonly int entityId;
        public readonly ushort worldId;

        public EntityDestroyedEvent(int entityId, ushort worldId) {
            this.entityId = entityId;
            this.worldId = worldId;
        }
    }
    
    internal readonly struct EntityEnabledEvent {
        public readonly int entityId;

        public EntityEnabledEvent(in int entityId) {
            this.entityId = entityId;
        }
    }
    
    internal readonly struct EntityDisabledEvent {
        public readonly int entityId;

        public EntityDisabledEvent(in int entityId) {
            this.entityId = entityId;
        }
    }
    
    internal readonly struct EntityComponentAddedEvent<T> {
        public readonly int entityId;
        public readonly T component;

        public EntityComponentAddedEvent(int entityId, in T component) {
            this.entityId = entityId;
            this.component = component;
        }
    }
    
    internal readonly struct EntityComponentRemovedEvent<T> {
        public readonly int entityId;
        public readonly T component;

        public EntityComponentRemovedEvent(int entityId, T component) {
            this.entityId = entityId;
            this.component = component;
        }
    }
}