using Sirenix.OdinInspector;
using UnityEngine;

namespace UFlow.Addon.Entities.Core.Runtime {
    public struct RectTransformRef : IEcsComponentData {
        [ReadOnly] public RectTransform value;
    }
}