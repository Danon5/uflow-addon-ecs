using JetBrains.Annotations;
using UFlow.Addon.ECS.Core.Runtime;
using UFlow.Core.Runtime;

namespace UFlow.Addon.ECS.Core.Editor {
    [UsedImplicitly]
    internal static class StaticCacheClearer {
        static StaticCacheClearer() {
            UnityGlobalEventHelper.RuntimeInitializeOnLoad += () => ExternalEngineEvents.clearStaticCachesEvent?.Invoke();
        }
    }
}