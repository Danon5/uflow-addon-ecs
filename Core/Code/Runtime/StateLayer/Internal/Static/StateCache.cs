using System.Runtime.CompilerServices;
using UFlow.Core.Shared;

namespace UFlow.Addon.Entities.Core.Runtime {
#if UFLOW_IL2CPP_ENABLED
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    internal static class StateCache<T> where T : ICacheElement {
        private static readonly SparseList<SparseList<T>> s_caches = new();

        static StateCache() => StaticEventBus<RuntimeInitializeOnLoadEvent>.Subscribe(ClearStaticCache);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Set(ushort worldId, int id, in T value) =>
            ref (s_caches.Has(worldId) ? s_caches.Get(worldId) : s_caches.Set(worldId, new SparseList<T>(1))).Set(id, value);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Get(ushort worldId, int id) => ref s_caches.Get(worldId).Get(id);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T GetOrCreate(ushort worldId, int id) =>
            ref (s_caches.Has(worldId) ? s_caches.Get(worldId) : s_caches.Set(worldId, new SparseList<T>(1))).Get(id);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Has(ushort worldId, int id) => s_caches.Has(worldId) && s_caches.Get(worldId).Has(id);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Remove(ushort worldId, int id) => s_caches.Get(worldId).Remove(id);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Remove(ushort worldId) => s_caches.Remove(worldId);
        
        private static void ClearStaticCache() => s_caches.Clear();
    }
}