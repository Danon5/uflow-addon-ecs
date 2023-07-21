using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UFlow.Addon.Ecs.Core.Runtime {
    public sealed class EntityPrefabSerializationKeyProvider : MonoBehaviour {
#if UNITY_EDITOR
        [BoxGroup("Data"), ShowIf("@" + nameof(IsValidPrefab)), DisableIf("@" + nameof(IsPlaying)), Required]
#endif
        [SerializeField]
        private string m_persistentKey;

#if UNITY_EDITOR
        [HideInInspector]
#endif
        [SerializeField]
        private bool m_isValidPrefab;
#if UNITY_EDITOR
        private bool m_instantiated;
#endif

        internal string PersistentKey => m_persistentKey;
        internal bool IsValidPrefab => m_isValidPrefab;

#if UNITY_EDITOR
        private bool IsPlaying => Application.isPlaying && m_instantiated;
#endif
        [UsedImplicitly]
        private void Awake() => m_instantiated = true;

        [UsedImplicitly]
        private void OnValidate() {
            if (Application.isPlaying) return;
            m_isValidPrefab = PrefabUtility.GetPrefabAssetType(gameObject) is not
                PrefabAssetType.NotAPrefab or PrefabAssetType.MissingAsset;
        }
    }
}