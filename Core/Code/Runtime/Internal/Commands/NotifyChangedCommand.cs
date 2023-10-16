namespace UFlow.Addon.ECS.Core.Runtime {
    public sealed class NotifyChangedCommand<T> : IEcsCommand where T : IEcsComponent {
        public void Execute(in Entity entity) => entity.NotifyChanged<T>();
    }
}