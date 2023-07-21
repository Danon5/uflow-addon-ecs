using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UFlow.Addon.Ecs.Core.Runtime.Components;
using UFlow.Core.Runtime;
using UFlow.Odin.Runtime;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[assembly: InternalsVisibleTo("UFlow.Addon.Ecs.Core.Editor")]
namespace UFlow.Addon.Ecs.Core.Runtime {
    public class SceneEntity : MonoBehaviour {
        [SerializeField, InlineProperty, HideLabel] private EntityInspector m_inspector;
        [SerializeField, HideInInspector] private bool m_isValidPrefab;
        [SerializeField, ColoredBoxGroup("Serialization", Color = nameof(Color)), 
         ShowIf("@" + nameof(m_isValidPrefab) + "&& !" + nameof(IsPlaying)), 
         ValidateInput(nameof(IsValidPersistentKey), "Persistent Key is required")]
        private string m_persistentKey;
#if UNITY_EDITOR
        private bool m_instantiated;
#endif

        public World World { get; private set; }
        public Entity Entity { get; private set; }
        internal string PersistentKey => m_persistentKey;
#if UNITY_EDITOR
        private bool IsPlaying => Application.isPlaying && m_instantiated;
        private bool IsValidPersistentKey => !m_isValidPrefab || !m_persistentKey.Equals(string.Empty);
        private Color Color => m_inspector.Color;
#endif

        [UsedImplicitly]
        private void Awake() {
            m_instantiated = true;
            World = GetWorld();
            Entity = World.CreateEntity(m_inspector.EntityEnabled);
            m_inspector.BakeAuthoringComponents(Entity);
            gameObject.SetActive(Entity.IsEnabled());
            if (!m_isValidPrefab) return;
            Entity.Set(new InstantiatedSceneEntity {
                persistentKey = m_persistentKey
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
            m_isValidPrefab = PrefabUtility.GetPrefabAssetType(gameObject) is not
                PrefabAssetType.NotAPrefab or PrefabAssetType.MissingAsset;
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