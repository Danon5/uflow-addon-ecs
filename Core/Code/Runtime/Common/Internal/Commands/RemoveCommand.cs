namespace UFlow.Addon.Entities.Core.Runtime {
    internal readonly struct RemoveCommand<T> : IEcsCommand where T : IEcsComponentData {
        public void Execute(in Entity entity) => entity.Remove<T>();
    }
}