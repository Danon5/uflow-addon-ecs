using UFlow.Addon.ECS.Core.Runtime;
using UFlow.Core.Runtime;
using UnityEditor;

namespace UFlow.Addon.ECS.Core.Editor {
    [InitializeOnLoad]
    internal static class StaticCacheClearer {
        static StaticCacheClearer() {
            UnityGlobalEventHelper.RuntimeInitializeOnLoad += () => ExternalEngineEvents.clearStaticCachesEvent?.Invoke();
        }
    }
}