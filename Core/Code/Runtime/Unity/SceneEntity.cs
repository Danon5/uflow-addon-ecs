using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UFlow.Core.Runtime;
using UnityEngine;

[assembly: InternalsVisibleTo("UFlow.Addon.Ecs.Core.Editor")]
namespace UFlow.Addon.Ecs.Core.Runtime {
    public class SceneEntity : MonoBehaviour {
        [SerializeField, InlineProperty, HideLabel] private EntityComponentInspector m_inspector;
        
        public World World { get; private set; }
        public Entity Entity { get; private set; }
        
        [UsedImplicitly]
        private void Awake() {
            World = GetWorld();
            Entity = World.CreateEntity();
            m_inspector.BakeAuthoringComponents(Entity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void UpdateRuntimeInspector() {
            m_inspector.UpdateRuntimeComponents(Entity);
        }

        protected virtual World GetWorld() => UFlowUtils.Modules.Get<EcsModule>().World;
    }
}