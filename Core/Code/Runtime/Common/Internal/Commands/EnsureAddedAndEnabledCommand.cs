namespace UFlow.Addon.ECS.Core.Runtime {
    internal readonly struct EnsureAddedAndEnabledCommand<T> : IEcsCommand where T : IEcsComponentData {
        public void Execute(in Entity entity) => entity.EnsureAddedAndEnabled<T>();
    }
}