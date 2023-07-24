namespace UFlow.Addon.ECS.Core.Runtime {
    public interface IRunSystem : ISystem {
        void PreRun();
        void Run();
        void PostRun();
        void SetEnabled(bool value);
        void Enable();
        void Disable();
        bool IsEnabled();
    }
}