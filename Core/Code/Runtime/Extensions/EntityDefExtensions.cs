using System.Runtime.CompilerServices;

namespace UFlow.Addon.ECS.Core.Runtime {
    public static class EntityDefExtensions {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Entity Instantiate(this EntityDef def) => def.CreateEntity(EcsModule.Get().World);
    }
}