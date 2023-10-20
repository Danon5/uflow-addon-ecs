namespace UFlow.Addon.ECS.Core.Runtime {
    public interface IEcsCommand {
        void Execute(in Entity entity) { } 
    }
}