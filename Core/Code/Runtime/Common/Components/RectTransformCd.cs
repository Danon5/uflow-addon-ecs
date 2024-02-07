using Sirenix.OdinInspector;
using UnityEngine;

namespace UFlow.Addon.Entities.Core.Runtime {
    public struct RectTransformCd : IEcsComponentData {
        [ReadOnly] public RectTransform value;
    }
}