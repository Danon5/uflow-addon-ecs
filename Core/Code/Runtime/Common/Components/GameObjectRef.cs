using Sirenix.OdinInspector;
using UnityEngine;

namespace UFlow.Addon.Entities.Core.Runtime {
    public struct GameObjectRef : IEcsComponentData {
        [ReadOnly] public GameObject value;
    }
}