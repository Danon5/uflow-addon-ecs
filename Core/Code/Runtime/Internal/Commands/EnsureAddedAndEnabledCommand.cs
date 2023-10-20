namespace UFlow.Addon.ECS.Core.Runtime {
    internal readonly struct EnsureAddedAndEnabledCommand<T> : IEcsCommand where T : IEcsComponent {
        public void Execute(in Entity entity) => entity.EnsureAddedAndEnabled<T>();
    }
}