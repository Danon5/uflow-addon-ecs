using UnityEngine;

namespace UFlow.Addon.Ecs.Core.Runtime {
    public static class WorldExtensions {
        public static LocalSystemRunner CreateLocalSystemRunner(this World world) {
            var systemRunner = new GameObject("LocalSystemRunner") {
                hideFlags = HideFlags.HideInHierarchy
            }.AddComponent<LocalSystemRunner>();
            return systemRunner;
        }
        
        public static SystemGroup CreateSystemGroup(this World world) {
            return new SystemGroup(world);
        }
    }
}