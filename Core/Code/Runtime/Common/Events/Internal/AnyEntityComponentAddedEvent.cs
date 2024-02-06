using System;

namespace UFlow.Addon.Entities.Core.Runtime {
    internal readonly struct AnyEntityComponentAddedEvent {
        public readonly Entity entity;
        public readonly Type type;

        public AnyEntityComponentAddedEvent(in Entity entity, Type type) {
            this.entity = entity;
            this.type = type;
        }
    }

    internal delegate void AnyEntityComponentAddedHandler(in Entity entity, Type type);
}