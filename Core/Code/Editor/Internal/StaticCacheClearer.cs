using UFlow.Addon.ECS.Core.Runtime;
using UFlow.Core.Runtime;
using UFlow.Core.Shared;
using UnityEditor;

namespace UFlow.Addon.ECS.Core.Editor {
    [InitializeOnLoad]
    internal static class StaticCacheClearer {
        static StaticCacheClearer() =>
            StaticEventBus<RuntimeInitializeOnLoadEvent>.Subscribe(() => ExternalEngineEvents.clearStaticCachesEvent?.Invoke());
    }
}