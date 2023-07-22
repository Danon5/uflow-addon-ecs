using JetBrains.Annotations;
using UFlow.Addon.ECS.Core.Runtime;
using UFlow.Core.Editor;

namespace UFlow.Addon.ECS.Core.Editor {
    [UsedImplicitly]
    internal static class StaticCacheClearer {
        static StaticCacheClearer() {
            UFlowStaticCacheClearer.ClearEvent += () => ExternalEngineEvents.clearStaticCachesEvent?.Invoke();
        }
    }
}