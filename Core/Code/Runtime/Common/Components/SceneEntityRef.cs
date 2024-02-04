using Sirenix.OdinInspector;

namespace UFlow.Addon.ECS.Core.Runtime {
    public struct SceneEntityRef : IEcsComponentData {
        [ReadOnly] public SceneEntity value;
    }
}