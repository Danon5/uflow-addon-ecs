using Sirenix.OdinInspector;

namespace UFlow.Addon.Ecs.Core.Runtime.Components {
    public struct InstantiatedSceneEntity : IEcsComponent {
        [ReadOnly] public string persistentKey;
    }
}