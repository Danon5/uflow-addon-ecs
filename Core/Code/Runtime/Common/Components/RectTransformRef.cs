using Sirenix.OdinInspector;
using UnityEngine;

namespace UFlow.Addon.ECS.Core.Runtime {
    public struct RectTransformRef : IEcsComponentData {
        [ReadOnly] public RectTransform value;
    }
}