using Sirenix.OdinInspector;

namespace UFlow.Addon.ECS.Core.Runtime {
    public struct SceneEntityRef : IEcsComponent {
        [ReadOnly] public SceneEntity value;
    }
}