using UFlow.Core.Runtime;

namespace UFlow.Addon.Entities.Core.Runtime {
    public sealed class RuntimeEntitiesTester : BaseBehaviour {
        protected override void Awake() {
            SystemCache.Test();
        }
    }
}