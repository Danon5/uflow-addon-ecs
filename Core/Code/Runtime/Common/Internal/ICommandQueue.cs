namespace UFlow.Addon.ECS.Core.Runtime {
    internal interface ICommandQueue {
        bool TryExecuteNextCommand();
    }
}