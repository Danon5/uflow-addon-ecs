using Sirenix.OdinInspector;
using UnityEngine;

namespace UFlow.Addon.ECS.Core.Runtime {
    public struct GameObjectRef : IEcsComponent {
        [ReadOnly] public GameObject value;
    }
}