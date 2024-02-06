using System.Runtime.CompilerServices;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UFlow.Addon.Entities.Core.Runtime {
    [CreateAssetMenu(
        fileName = "New" + nameof(EntityDef),
        menuName = "UFlow/ECS/" + nameof(EntityDef))]
    public sealed class EntityDef : ScriptableObject {
        [SerializeField, InlineProperty, HideLabel] internal EntityInspector inspector;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity CreateEntity(in World world) {
            var entity = world.CreateEntity();
            inspector.BakeAuthoringComponents(entity);
            return entity;
        }
    }
}