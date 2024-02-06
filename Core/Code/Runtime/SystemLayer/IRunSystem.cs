namespace UFlow.Addon.Entities.Core.Runtime {
    public interface IRunSystem : ISystem {
        void PreRun();
        void Run();
        void PostRun();
    }
}