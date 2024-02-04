using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UFlow.Core.Runtime;
using UFlow.Core.Shared;

// ReSharper disable StaticMemberInGenericType

namespace UFlow.Addon.ECS.Core.Runtime {
#if IL2CPP_ENABLED
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    internal static class Stashes<T> where T : IEcsComponentData {
        public static readonly Bit Bit = Bit.GetNextBit();
        internal static readonly bool IsComponentTypeDisposable;
        private static Stash<T>[] s_stashes;
        private static Stash<T>[] s_previousStashes;
        private static IDisposable[] s_subscriptions;

        static Stashes() {
            IsComponentTypeDisposable = typeof(T).GetMethod("Dispose", BindingFlags.Public | BindingFlags.Instance) != null;
            s_stashes = Array.Empty<Stash<T>>();
            s_previousStashes = Array.Empty<Stash<T>>();
            s_subscriptions = Array.Empty<IDisposable>();
            Publishers<WorldDestroyedEvent>.Global.Subscribe(On);
            ExternalEngineEvents.clearStaticCachesEvent += ClearStaticCaches;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Stash<T> Get(short worldId) => s_stashes[worldId];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Stash<T> GetPrevious(short worldId) => s_previousStashes[worldId];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Stash<T> GetOrCreate(short worldId) {
            UFlowUtils.Collections.EnsureIndex(ref s_stashes, worldId);
            UFlowUtils.Collections.EnsureIndex(ref s_subscriptions, worldId);
            s_stashes[worldId] ??= new Stash<T>();
            var world = Worlds.Get(worldId);
            s_subscriptions[worldId] ??= new[] {
                world.SubscribeEntityEnabled((in Entity entity) => {
                    if (!entity.Has<T>()) return;
                    world.Publish(new EntityComponentParentEnabledEvent<T>(entity));
                }),
                world.SubscribeEntityDisabled((in Entity entity) => {
                    if (!entity.Has<T>()) return;
                    world.Publish(new EntityComponentParentDisabledEvent<T>(entity));
                }),
                world.SubscribeEntityDisableComponents((in Entity entity) => {
                    if (!entity.Has<T>()) return;
                    entity.Disable<T>();
                }),
                world.WhenEntityRemoveComponents((in Entity entity) => {
                    if (!entity.Has<T>()) return;
                    entity.Remove<T>();
                }),
                world.SubscribeReset(() => {
                    Remove(worldId);
                })
            }.MergeIntoGroup();
            return s_stashes[worldId];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Stash<T> GetOrCreatePrevious(short worldId) {
            UFlowUtils.Collections.EnsureIndex(ref s_previousStashes, worldId);
            s_previousStashes[worldId] ??= new Stash<T>();
            return s_previousStashes[worldId];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGet(short worldId, out Stash<T> stash) {
            if (worldId >= s_stashes.Length) {
                stash = default;
                return false;
            }
            stash = s_stashes[worldId];
            return stash != null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetPrevious(short worldId, out Stash<T> stash) {
            if (worldId >= s_previousStashes.Length) {
                stash = default;
                return false;
            }
            stash = s_previousStashes[worldId];
            return stash != null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool Has(short worldId) => worldId < s_stashes.Length && s_stashes[worldId] != null;

        private static void Remove(short worldId) {
            s_stashes[worldId].Clear();
            s_stashes[worldId] = null;
            s_previousStashes[worldId].Clear();
            s_previousStashes[worldId] = null;
        }

        private static void On(in WorldDestroyedEvent @event) {
            if (!Has(@event.worldId)) return;
            s_subscriptions[@event.worldId]?.Dispose();
            s_subscriptions[@event.worldId] = null;
            Remove(@event.worldId);
        }

        private static void ClearStaticCaches() {
            s_stashes = Array.Empty<Stash<T>>();
            s_previousStashes = Array.Empty<Stash<T>>();
            s_subscriptions = Array.Empty<IDisposable>();
        }
    }
}