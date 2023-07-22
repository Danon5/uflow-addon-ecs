using System.Collections.Generic;
using UFlow.Core.Runtime;
using UnityEngine;

namespace UFlow.Addon.ECS.Core.Runtime {
    internal static class EntityPrefabMap {
        private static readonly Dictionary<string, int> s_keyToHash = new();
        private static readonly Dictionary<int, GameObject> S_hashToPrefab = new();

        static EntityPrefabMap() {
            foreach (var contentRef in Root.Singleton.Context.GetAllContentRefsEnumerable()) {
                if (contentRef is not ContentRef<GameObject> objectRef) continue;
                if (!objectRef.IsAssetAssigned() || !objectRef.Asset.TryGetComponent(out SceneEntity sceneEntity)) continue;
                
            }
        }

        public static int GetHash(in string key) => s_keyToHash[key];

        public static GameObject GetPrefab(int hash) => S_hashToPrefab[hash];
    }
}