using System.Runtime.CompilerServices;
using UFlow.Core.Runtime;

namespace UFlow.Addon.ECS.Core.Runtime {
    public static class ContentExtensions {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Entity Instantiate(this ContentRef<EntityDef> def) => EntityDefExtensions.Instantiate(def.Asset);
    }
}