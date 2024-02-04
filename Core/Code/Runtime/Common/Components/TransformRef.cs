using Sirenix.OdinInspector;
using UnityEngine;

namespace UFlow.Addon.ECS.Core.Runtime {
    public struct TransformRef : IEcsComponentData {
        [ReadOnly] public Transform value;
    }
}