namespace UFlow.Addon.Ecs.Core.Runtime {
    public interface IRunDeltaSystem : ISystem {
        void PreRun(float delta);
        void Run(float delta);
        void PostRun(float delta);
    }
}