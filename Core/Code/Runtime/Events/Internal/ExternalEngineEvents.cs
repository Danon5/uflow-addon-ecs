using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("UFlow.Addon.Ecs.Core.Editor")]
namespace UFlow.Addon.Ecs.Core.Runtime {
    public static class ExternalEngineEvents {
        public static Action clearStaticCachesEvent;
    }
}