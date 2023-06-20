using JetBrains.Annotations;

namespace UFlow.Core.Runtime {
    public sealed class LocalSystemRunner : BaseSystemRunner<LocalSystemRunTiming> {
        [UsedImplicitly]
        private void Update() {
            RunGroup(LocalSystemRunTiming.Update);
        }

        [UsedImplicitly]
        private void FixedUpdate() {
            RunGroup(LocalSystemRunTiming.FixedUpdate);
        }

        [UsedImplicitly]
        private void LateUpdate() {
            RunGroup(LocalSystemRunTiming.LateUpdate);
        }

        [UsedImplicitly]
        private void OnDrawGizmos() {
            RunGroup(LocalSystemRunTiming.OnDrawGizmos);
        }

        [UsedImplicitly]
        private void OnGUI() {
            RunGroup(LocalSystemRunTiming.OnGUI);
        }
    }
}