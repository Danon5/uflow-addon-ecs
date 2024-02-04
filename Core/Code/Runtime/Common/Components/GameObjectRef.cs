using Sirenix.OdinInspector;
using UnityEngine;

namespace UFlow.Addon.ECS.Core.Runtime {
    public struct GameObjectRef : IEcsComponentData {
        [ReadOnly] public GameObject value;
    }
}