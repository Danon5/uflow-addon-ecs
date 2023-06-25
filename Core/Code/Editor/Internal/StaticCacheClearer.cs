using UFlow.Addon.Ecs.Core.Runtime;
using UnityEngine;

namespace UFlow.Addon.Ecs.Core.Editor {
    internal static class StaticCacheClearer {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitializeOnLoad() {
            Publishers<StaticCacheClearEvent>.Global.Publish(new StaticCacheClearEvent());
        }
    }
}