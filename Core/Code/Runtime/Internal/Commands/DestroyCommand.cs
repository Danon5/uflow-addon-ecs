namespace UFlow.Addon.ECS.Core.Runtime {
    internal sealed class DestroyCommand : IEcsCommand {
        public void Execute(in Entity entity) => entity.Destroy();
    }
}