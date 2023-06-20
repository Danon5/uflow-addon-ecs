using UFlow.Addon.Ecs.Core.Runtime;
using UnityEngine;

namespace UFlow.Addon.Ecs.Core.Editor {
    internal static class StaticCacheClearer {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void InitializeOnLoad() {
            ExternalEngineUtilities.ClearStaticCache();
        }
    }
}