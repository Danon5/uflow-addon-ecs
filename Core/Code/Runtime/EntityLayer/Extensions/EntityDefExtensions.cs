using System.Runtime.CompilerServices;

namespace UFlow.Addon.Entities.Core.Runtime {
    public static class EntityDefExtensions {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Entity Instantiate(this EntityDef def) => def.CreateEntity(EcsModule<DefaultWorld>.Get().World);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Entity Instantiate(this EntityDef def, in World world) => def.CreateEntity(world);
    }
}