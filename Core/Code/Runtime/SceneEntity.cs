using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UFlow.Core.Runtime;
using UnityEngine;

[assembly: InternalsVisibleTo("UFlow.Addon.Ecs.Core.Editor")]
namespace UFlow.Addon.Ecs.Core.Runtime {
    public class SceneEntity : MonoBehaviour {
        [SerializeField, InlineProperty, HideLabel] internal EntityInspector inspector;
        
        public World World { get; private set; }
        public Entity Entity { get; private set; }
        
        [UsedImplicitly]
        private void Awake() {
            World = GetWorld();
            Entity = World.CreateEntity(inspector.EntityEnabled);
            inspector.BakeAuthoringComponents(Entity);
        }

        [UsedImplicitly]
        private void OnDestroy() {
            if (!World.IsAlive()) return;
            Entity.Destroy();
        }

#if UNITY_EDITOR
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void RetrieveRuntimeInspector() {
            inspector.RetrieveRuntimeState();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void ApplyRuntimeInspector() {
            inspector.ApplyRuntimeState();
        }
#endif

        protected virtual World GetWorld() => UFlowUtils.Modules.Get<EcsModule>().World;
    }
}