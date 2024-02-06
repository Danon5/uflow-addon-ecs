namespace UFlow.Addon.Entities.Core.Runtime {
    public interface IEcsCommand {
        void Execute(in Entity entity) { } 
    }
}