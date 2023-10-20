namespace UFlow.Addon.ECS.Core.Runtime {
    internal readonly struct RemoveCommand<T> : IEcsCommand where T : IEcsComponent {
        public void Execute(in Entity entity) => entity.Remove<T>();
    }
}