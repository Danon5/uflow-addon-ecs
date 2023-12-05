using Sirenix.OdinInspector;
using UnityEngine;

namespace UFlow.Addon.ECS.Core.Runtime {
    public struct RectTransformRef : IEcsComponent {
        [ReadOnly] public RectTransform value;
    }
}