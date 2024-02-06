namespace UFlow.Addon.Entities.Core.Runtime {
    public interface IEnableDisableSystem {
        void SetEnabled(bool value);
        void Enable();
        void Disable();
        bool IsEnabled();
    }
}