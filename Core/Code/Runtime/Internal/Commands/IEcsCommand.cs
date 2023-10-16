namespace UFlow.Addon.ECS.Core.Runtime {
    internal interface IEcsCommand {
        void Execute(in Entity entity);
    }
}