namespace UFlow.Addon.ECS.Core.Runtime {
    public interface IRunSystem : ISystem {
        void PreRun();
        void Run();
        void PostRun();
    }
}