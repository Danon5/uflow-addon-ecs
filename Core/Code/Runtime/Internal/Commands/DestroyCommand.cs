namespace UFlow.Addon.ECS.Core.Runtime {
    internal sealed class DestroyCommand : IEntityCommand {
        public void Execute(in Entity entity) => entity.Destroy();
    }
}