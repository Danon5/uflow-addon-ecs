namespace UFlow.Addon.ECS.Core.Runtime {
    public interface IEnableDisableSystem {
        void SetEnabled(bool value);
        void Enable();
        void Disable();
        bool IsEnabled();
    }
}