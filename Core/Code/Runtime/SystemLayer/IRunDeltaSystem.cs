namespace UFlow.Addon.Entities.Core.Runtime {
    public interface IRunDeltaSystem : ISystem {
        void PreRun(float delta);
        void Run(float delta);
        void PostRun(float delta);
    }
}