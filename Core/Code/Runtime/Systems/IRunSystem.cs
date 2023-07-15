namespace UFlow.Addon.Ecs.Core.Runtime {
    public interface IRunSystem : ISystem {
        void PreRun();
        void Run();
        void PostRun();
    }
}