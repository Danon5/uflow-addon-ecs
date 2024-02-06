namespace UFlow.Addon.Entities.Core.Runtime {
    internal interface ICommandQueue {
        bool TryExecuteNextCommand();
    }
}