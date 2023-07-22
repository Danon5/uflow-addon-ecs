using System;

namespace UFlow.Addon.ECS.Core.Runtime {
    internal readonly struct WorldInstanceSubscription<T> : IDisposable {
        public readonly GenericHandler<T> action;
        public readonly int worldId;

        public WorldInstanceSubscription(in GenericHandler<T> action, int worldId) {
            this.action = action;
            this.worldId = worldId;
        }

        public void Dispose() {
            Publishers<T>.WorldInstance.Unsubscribe(this);
        }
    }
}