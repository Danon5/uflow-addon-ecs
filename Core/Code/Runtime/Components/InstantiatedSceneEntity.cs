using Sirenix.OdinInspector;

namespace UFlow.Addon.ECS.Core.Runtime.Components {
    [EcsSerializable("InstantiatedSceneEntity")]
    public struct InstantiatedSceneEntity : IEcsComponent {
        [ReadOnly] public SceneEntity sceneEntity;
        [ReadOnly] public string persistentKey;
    }
}