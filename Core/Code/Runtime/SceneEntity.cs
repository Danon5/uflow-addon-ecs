using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UFlow.Core.Runtime;
using UFlow.Odin.Runtime;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

[assembly: InternalsVisibleTo("UFlow.Addon.Ecs.Core.Editor")]
namespace UFlow.Addon.ECS.Core.Runtime {
    public class SceneEntity : MonoBehaviour {
#if UNITY_EDITOR
        [InlineProperty, HideLabel]
#endif
        [SerializeField]
        private EntityInspector m_inspector;
#if UNITY_EDITOR
        [HideInInspector]
#endif
        [SerializeField]
        private bool m_isValidPrefab;
#if UNITY_EDITOR
        [ColoredBoxGroup("Serialization", Color = nameof(Color))]
#endif
        [SerializeField]
        private bool m_enableSerialization;
#if UNITY_EDITOR
        [ColoredBoxGroup("Serialization", Color = nameof(Color)), 
         ShowIf("@" + nameof(m_isValidPrefab) + "&& !" + nameof(IsPlaying) + "&& " + nameof(m_enableSerialization)), 
         ValidateInput(nameof(IsValidPersistentKey), "Persistent Key is required")]
#endif
        [SerializeField]
        private string m_persistentKey;
        private bool m_destroying;
        private bool m_destroyingDirectly;
#if UNITY_EDITOR
        private bool m_instantiated;
        private bool m_requiresRuntimeRetrieval;
#endif
        
        public World World { get; private set; }
        public Entity Entity { get; private set; }
        internal bool IsEditorFocused { get; set; }
        internal string PersistentKey => m_persistentKey;
#if UNITY_EDITOR
        internal bool IsPlaying => Application.isPlaying && m_instantiated;
        private bool IsValidPersistentKey => !m_enableSerialization && !m_isValidPrefab || 
            (m_persistentKey != null && !m_persistentKey.Equals(string.Empty));
        private Color Color => m_inspector.Color;
#endif

        [UsedImplicitly]
        private void Awake() {
#if UNITY_EDITOR
            m_instantiated = true;
#endif
            World = GetWorld();
            CreateEntity();
            World.WhenEntityDestroyed((in Entity e) => {
                if (e == Entity)
                    DestroyEntity();
            });
        }

        [UsedImplicitly]
        private void OnDestroy() {
            m_destroying = true;
            if (m_destroyingDirectly) return;
            if (World == null) return;
            if (!World.IsAlive()) return;
            if (!Entity.IsAlive()) return;
            Entity.Destroy();
        }

        [UsedImplicitly]
        private void OnEnable() {
            if (World == null) return;
            if (!World.IsAlive()) return;
            if (!Entity.IsAlive()) return;
            Entity.Enable();
        }

        [UsedImplicitly]
        private void OnDisable() {
            if (World == null) return;
            if (!World.IsAlive()) return;
            if (!Entity.IsAlive()) return;
            Entity.Disable();
        }
        
#if UNITY_EDITOR
        [UsedImplicitly]
        private void OnValidate() {
            if (Application.isPlaying) return;
            m_isValidPrefab = PrefabUtility.GetPrefabAssetType(gameObject) is not
                PrefabAssetType.NotAPrefab or PrefabAssetType.MissingAsset || PrefabStageUtility.GetPrefabStage(gameObject);
        }
#endif

        public Entity CreateEntity() {
            if (World == null)
                throw new Exception("Attempting to create a SceneEntity with no valid world.");
            if (Entity.IsAlive())
                throw new Exception("Attempting to create a SceneEntity multiple times.");
            Entity = World.CreateEntity(m_inspector.EntityEnabled);
            Entity.Set(new GameObjectRef {
                value = gameObject
            });
            if (TryGetComponent(out RectTransform rectTransform)) {
                Entity.Set(new RectTransformRef {
                    value = rectTransform
                });
            }
            else {
                Entity.Set(new TransformRef {
                    value = transform
                });
            }
            m_inspector.BakeAuthoringComponents(Entity);
            gameObject.SetActive(Entity.IsEnabled());
            if (!m_isValidPrefab) return Entity;
            LogicHook<PrefabSceneEntityCreatedHook>.Execute(new PrefabSceneEntityCreatedHook(this, m_persistentKey));
            return Entity;
        }

        public void DestroyEntity() {
            if (m_destroying) return;
            m_destroyingDirectly = true;
            Destroy(gameObject);
        }
        
        internal Entity CreateEntityWithIdAndGen(int id, ushort gen) {
            if (World == null)
                throw new Exception("Attempting to create a SceneEntity with no valid world.");
            if (Entity.IsAlive())
                throw new Exception("Attempting to create a SceneEntity multiple times.");
            Entity = World.CreateEntityWithIdAndGen(id, gen, m_inspector.EntityEnabled);
            m_inspector.BakeAuthoringComponents(Entity);
            gameObject.SetActive(Entity.IsEnabled());
            if (!m_isValidPrefab) return Entity;
            LogicHook<PrefabSceneEntityCreatedHook>.Execute(new PrefabSceneEntityCreatedHook(this, m_persistentKey));
            return Entity;
        }

#if UNITY_EDITOR
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void RetrieveRuntimeInspector() {
            if (World == null) return;
            if (!World.IsAlive()) return;
            if (!Entity.IsAlive()) return;
            m_inspector.RetrieveRuntimeState();
            var isEnabled = Entity.IsEnabled();
            if (gameObject.activeSelf != isEnabled)
                gameObject.SetActive(isEnabled);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void ApplyRuntimeInspector() {
            if (World == null) return;
            if (!World.IsAlive()) return;
            if (!Entity.IsAlive()) return;
            m_inspector.ApplyRuntimeState();
        }
#endif

        protected virtual World GetWorld() => EcsModule.Get().World;
    }
}