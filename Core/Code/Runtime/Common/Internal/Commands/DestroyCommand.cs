namespace UFlow.Addon.ECS.Core.Runtime {
    internal readonly struct DestroyCommand : IEcsCommand {
        public void Execute(in Entity entity) => entity.Destroy();
    }
}