using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UFlow.Addon.Ecs.Core.Runtime.Components;
using UFlow.Core.Runtime;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[assembly: InternalsVisibleTo("UFlow.Addon.Ecs.Core.Editor")]
namespace UFlow.Addon.Ecs.Core.Runtime {
    public class SceneEntity : MonoBehaviour {
        [SerializeField, InlineProperty, HideLabel] private EntityInspector m_inspector;
        [SerializeField, HideInInspector] private bool m_isPrefab;
        [SerializeField, HideInInspector] private string m_assetGuid;
        
        public World World { get; private set; }
        public Entity Entity { get; private set; }

        [UsedImplicitly]
        private void Awake() {
            World = GetWorld();
            Entity = World.CreateEntity(m_inspector.EntityEnabled);
            m_inspector.BakeAuthoringComponents(Entity);
            gameObject.SetActive(Entity.IsEnabled());
            if (!m_isPrefab) return;
            Entity.Set(new InstantiatedSceneEntity {
                assetGuid = m_assetGuid
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
        [UsedImplicitly]
        private void OnValidate() {
            if (Application.isPlaying) return;
            m_isPrefab = PrefabUtility.GetPrefabAssetType(gameObject) is not PrefabAssetType.NotAPrefab or PrefabAssetType.MissingAsset;
            if (!m_isPrefab) return;
            m_assetGuid = AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(gameObject)).ToString();
        }

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