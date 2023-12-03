using System;

namespace UFlow.Addon.ECS.Core.Runtime {
    internal readonly struct AnyEntityComponentRemovedEvent {
        public readonly Entity entity;
        public readonly Type type;

        public AnyEntityComponentRemovedEvent(in Entity entity, Type type) {
            this.entity = entity;
            this.type = type;
        }
    }

    internal delegate void AnyEntityComponentRemovedHandler(in Entity entity, Type type);
}