using Sirenix.OdinInspector;
using UnityEngine;

namespace UFlow.Addon.Entities.Core.Runtime {
    public struct GameObjectCd : IEcsComponentData {
        [ReadOnly] public GameObject value;
    }
}