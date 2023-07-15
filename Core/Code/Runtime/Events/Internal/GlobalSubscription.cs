using System;

namespace UFlow.Addon.Ecs.Core.Runtime {
    internal readonly struct GlobalSubscription<T> : IDisposable {
        public readonly GenericHandler<T> action;

        public GlobalSubscription(in GenericHandler<T> action) {
            this.action = action;
        }

        public void Dispose() {
            Publishers<T>.Global.Unsubscribe(this);
        }
    }
}