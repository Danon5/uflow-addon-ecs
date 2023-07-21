using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UFlow.Addon.Ecs.Core.Runtime.Components;
using UFlow.Core.Runtime;
using UnityEngine;

[assembly: InternalsVisibleTo("UFlow.Addon.Ecs.Core.Editor")]
namespace UFlow.Addon.Ecs.Core.Runtime {
    public class SceneEntity : MonoBehaviour {
#if UNITY_EDITOR
        [InlineProperty, HideLabel]
#endif
        [SerializeField]
        private EntityInspector m_inspector;
        
        public World World { get; private set; }
        public Entity Entity { get; private set; }

        [UsedImplicitly]
        private void Awake() {
            World = GetWorld();
            Entity = World.CreateEntity(m_inspector.EntityEnabled);
            m_inspector.BakeAuthoringComponents(Entity);
            gameObject.SetActive(Entity.IsEnabled());
            if (!TryGetComponent(out EntityPrefabSerializationKeyProvider keyProvider)) return;
            Entity.Set(new InstantiatedSceneEntity {
                persistentKey = keyProvider.PersistentKey
            });
        }

        [UsedImplicitly]
        private void OnDestroy() {
            if (!World.IsAlive()) return;
            Entity.Destroy();
        }

        [UsedImplicitly]
        private void OnEnable() => Entity.Enable();

        [UsedImplicitly]
        private void OnDisable() => Entity.Disable();

#if UNITY_EDITOR
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void RetrieveRuntimeInspector() {
            m_inspector.RetrieveRuntimeState();
            var isEnabled = Entity.IsEnabled();
            if (gameObject.activeSelf != isEnabled)
                gameObject.SetActive(isEnabled);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void ApplyRuntimeInspector() {
            m_inspector.ApplyRuntimeState();
        }
#endif

        protected virtual World GetWorld() => UFlowUtils.Modules.Get<EcsModule>().World;
    }
}