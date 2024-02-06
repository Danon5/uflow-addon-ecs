namespace UFlow.Addon.Entities.Core.Runtime {
    internal readonly struct NotifyChangedCommand<T> : IEcsCommand where T : IEcsComponentData {
        public void Execute(in Entity entity) => entity.NotifyChanged<T>();
    }
}