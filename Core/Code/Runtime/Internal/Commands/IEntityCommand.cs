namespace UFlow.Addon.ECS.Core.Runtime {
    internal interface IEntityCommand {
        void Execute(in Entity entity);
    }
}