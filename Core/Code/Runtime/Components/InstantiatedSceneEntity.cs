using Sirenix.OdinInspector;

namespace UFlow.Addon.ECS.Core.Runtime.Components {
    public struct InstantiatedSceneEntity : IEcsComponent {
        [ReadOnly] public string persistentKey;
    }
}