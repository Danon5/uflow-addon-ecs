namespace UFlow.Addon.ECS.Core.Runtime {
    public sealed class RemoveCommand<T> : IEcsCommand where T : IEcsComponent {
        public void Execute(in Entity entity) => entity.Remove<T>();
    }
}