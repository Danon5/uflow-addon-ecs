using Sirenix.OdinInspector;

namespace UFlow.Addon.Entities.Core.Runtime {
    public struct SceneEntityRef : IEcsComponentData {
        [ReadOnly] public SceneEntity value;
    }
}