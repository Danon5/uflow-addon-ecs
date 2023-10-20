namespace UFlow.Addon.ECS.Core.Runtime {
    internal readonly struct NotifyChangedCommand<T> : IEcsCommand where T : IEcsComponent {
        public void Execute(in Entity entity) => entity.NotifyChanged<T>();
    }
}