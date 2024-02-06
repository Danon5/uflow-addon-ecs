using Sirenix.OdinInspector;
using UnityEngine;

namespace UFlow.Addon.Entities.Core.Runtime {
    public struct TransformRef : IEcsComponentData {
        [ReadOnly] public Transform value;
    }
}