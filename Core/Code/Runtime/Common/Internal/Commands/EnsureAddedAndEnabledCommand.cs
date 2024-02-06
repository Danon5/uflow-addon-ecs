namespace UFlow.Addon.Entities.Core.Runtime {
    internal readonly struct EnsureAddedAndEnabledCommand<T> : IEcsCommand where T : IEcsComponentData {
        public void Execute(in Entity entity) => entity.EnsureAddedAndEnabled<T>();
    }
}