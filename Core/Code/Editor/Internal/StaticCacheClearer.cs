using UnityEngine;

namespace UFlow.Addon.Ecs.Core.Editor {
    internal static class StaticCacheClearer {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitializeOnLoad() {
            
        }
    }
}