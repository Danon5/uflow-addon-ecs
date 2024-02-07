using Sirenix.OdinInspector;

namespace UFlow.Addon.Entities.Core.Runtime {
    public struct SceneEntityCd : IEcsComponentData {
        [ReadOnly] public SceneEntity value;
    }
}