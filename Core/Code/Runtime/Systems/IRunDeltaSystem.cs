namespace UFlow.Addon.ECS.Core.Runtime {
    public interface IRunDeltaSystem : ISystem {
        void PreRun(float delta);
        void Run(float delta);
        void PostRun(float delta);
        void SetEnabled(bool value);
        void Enable();
        void Disable();
        bool IsEnabled();
    }
}