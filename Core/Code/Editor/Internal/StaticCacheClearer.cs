using JetBrains.Annotations;
using UFlow.Addon.Ecs.Core.Runtime;
using UFlow.Core.Editor;

namespace UFlow.Addon.Ecs.Core.Editor {
    [UsedImplicitly]
    internal static class StaticCacheClearer {
        static StaticCacheClearer() {
            UFlowStaticCacheClearer.ClearEvent += () => ExternalEngineEvents.clearStaticCachesEvent?.Invoke();
        }
    }
}