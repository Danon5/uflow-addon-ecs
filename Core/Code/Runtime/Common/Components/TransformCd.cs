using Sirenix.OdinInspector;
using UnityEngine;

namespace UFlow.Addon.Entities.Core.Runtime {
    public struct TransformCd : IEcsComponentData {
        [ReadOnly] public Transform value;
    }
}