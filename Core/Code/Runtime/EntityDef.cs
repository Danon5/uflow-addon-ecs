using Sirenix.OdinInspector;
using UnityEngine;

namespace UFlow.Addon.Ecs.Core.Runtime {
    [CreateAssetMenu(
        fileName = "New" + nameof(EntityDef),
        menuName = "UFlow/ECS/" + nameof(EntityDef))]
    public sealed class EntityDef : ScriptableObject {
        [SerializeField, InlineProperty, HideLabel] internal EntityInspector inspector;
    }
}