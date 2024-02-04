namespace UFlow.Addon.ECS.Core.Runtime {
    internal readonly struct RemoveCommand<T> : IEcsCommand where T : IEcsComponentData {
        public void Execute(in Entity entity) => entity.Remove<T>();
    }
}