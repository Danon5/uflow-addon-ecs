using Sirenix.OdinInspector;
using UnityEngine;

namespace UFlow.Addon.ECS.Core.Runtime {
    public struct TransformRef : IEcsComponent {
        [ReadOnly] public Transform value;
    }
}