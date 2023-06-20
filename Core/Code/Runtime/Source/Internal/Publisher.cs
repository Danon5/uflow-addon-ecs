using System;
using System.Runtime.CompilerServices;
using UFlow.Core.Runtime.Extensions;

namespace UFlow.Core.Runtime {
    internal static class Publisher {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IDisposable Subscribe<T>(PublishedEventHandler<T> action) => Publisher<T>.Subscribe(action);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IDisposable Subscribe<T>(ushort worldId, PublishedEventHandler<T> action) => Publisher<T>.Subscribe(worldId, action);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Publish<T>(in T @event) => Publisher<T>.Publish(@event);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Publish<T>(ushort worldId, in T @event) => Publisher<T>.Publish(worldId, @event);
    }

    internal static class Publisher<T> {
        public static PublishedEventHandler<T>[] actions;

        static Publisher() {
            actions = new PublishedEventHandler<T>[2];
            Publisher.Subscribe<WorldDestroyedEvent>(On);
            ExternalEngineUtilities.ClearStaticCacheEvent += ClearStaticCache;
        }

        public static IDisposable Subscribe(PublishedEventHandler<T> action) {
            actions[0] += action;
            return new Subscription(0, action);
        }
        
        public static IDisposable Subscribe(ushort worldId, PublishedEventHandler<T> action) {
            ArrayExtensions.EnsureLength(ref actions, worldId + 1);
            actions[worldId] += action;
            return new Subscription(worldId, action);
        }

        public static void Publish(in T @event) {
            actions[0]?.Invoke(@event);
        }
        
        public static void Publish(int worldId, in T @event) {
            if (worldId >= actions.Length) return;
            actions[worldId]?.Invoke(@event);
        }

        private static void On(in WorldDestroyedEvent @event) {
            if (@event.worldId >= actions.Length) return;
            actions[@event.worldId] = null;
        }

        private static void ClearStaticCache() {
            actions = new PublishedEventHandler<T>[2];
            Publisher.Subscribe<WorldDestroyedEvent>(On);
        }

        private readonly struct Subscription : IDisposable {
            private readonly ushort m_worldId;
            private readonly PublishedEventHandler<T> m_action;

            public Subscription(ushort worldId, PublishedEventHandler<T> action) {
                m_worldId = worldId;
                m_action = action;
            }
            
            public void Dispose() {
                actions[m_worldId] -= m_action;
            }
        }
    }
}