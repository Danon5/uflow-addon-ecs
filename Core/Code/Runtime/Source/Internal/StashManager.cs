using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using UFlow.Core.Runtime.DataStructures;
using UFlow.Core.Runtime.Extensions;

namespace UFlow.Core.Runtime {
#if IL2CPP_ENABLED
    using Ecs;
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [SuppressMessage("ReSharper", "InconsistentlySynchronizedField")]
    [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
    internal static class StashManager<T> {
        internal static readonly ComponentBit Bit;
        private static Stash<T>[] s_stashes;

        static StashManager() {
            Bit = ComponentBit.GetNextBit();
            s_stashes = Array.Empty<Stash<T>>();
            Publisher.Subscribe<WorldDestroyedEvent>(On);
            Publisher.Subscribe<EntityDestroyedEvent>(On);
            ExternalEngineUtilities.ClearStaticCacheEvent += ClearStaticCache;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Stash<T> Get(ushort worldId) {
            return worldId < s_stashes.Length ? s_stashes[worldId] : null;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Stash<T> GetOrCreate(ushort worldId) {
            ArrayExtensions.EnsureLength(ref s_stashes, worldId + 1);
            ref var stash = ref s_stashes[worldId];
            stash ??= new Stash<T>();
            return stash;
        }

        private static void On(in WorldDestroyedEvent @event) {
            if (@event.worldId >= s_stashes.Length) return;
            s_stashes[@event.worldId] = null;
        }

        private static void On(in EntityDestroyedEvent @event) {
            var entity = @event.entityId;
            var worldId = @event.worldId;
            if (worldId >= s_stashes.Length) return;
            var stash = s_stashes[worldId];
            if (stash == null) return;
            if (!stash.Has(entity)) return;
            stash.Remove(entity);
        }

        private static void ClearStaticCache() {
            s_stashes = Array.Empty<Stash<T>>();
        }
    }
}