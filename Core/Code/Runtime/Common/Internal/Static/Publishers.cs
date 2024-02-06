using System;
using System.Collections.Generic;
using UFlow.Core.Shared;

namespace UFlow.Addon.Entities.Core.Runtime {
#if IL2CPP_ENABLED
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    internal static class Publishers<T> {
        public static class Global {
            private static readonly Queue<T> s_delayedActionQueue = new();
            private static GenericHandler<T> s_action;

            static Global() {
                ExternalEngineEvents.clearStaticCachesEvent += ClearStaticCaches;
                PublisherDelay.DelayEndedEvent += InvokeDelayedEvents;
            }

            public static IDisposable Subscribe(in GenericHandler<T> action) {
                s_action += action;
                return new GlobalSubscription<T>(action);
            }
            
            public static void Unsubscribe(in GlobalSubscription<T> subscription) {
                s_action -= subscription.action;
            }

            public static void Publish(in T @event) {
                if (PublisherDelay.IsDelaying)
                    s_delayedActionQueue.Enqueue(@event);
                else
                    s_action?.Invoke(@event);
            }

            private static void InvokeDelayedEvents() {
                while (s_delayedActionQueue.TryDequeue(out var @event))
                    Publish(@event);
            }

            private static void ClearStaticCaches() {
                s_delayedActionQueue.Clear();
                s_action = null;
            }
        }

        public static class WorldInstance {
            private static readonly Dictionary<short, Queue<T>> s_delayedActionQueues = new();
            private static GenericHandler<T>[] s_actions = Array.Empty<GenericHandler<T>>();

            static WorldInstance() {
                ExternalEngineEvents.clearStaticCachesEvent += ClearStaticCaches;
                PublisherDelay.DelayEndedEvent += InvokeDelayedEvents;
            }

            public static IDisposable Subscribe(in GenericHandler<T> action, short worldId) {
                UFlowUtils.Collections.EnsureIndex(ref s_actions, worldId);
                s_actions[worldId] += action;
                return new WorldInstanceSubscription<T>(action, worldId);
            }

            public static void Unsubscribe(in WorldInstanceSubscription<T> subscription) {
                if (subscription.worldId >= s_actions.Length) return;
                s_actions[subscription.worldId] -= subscription.action;
            }
            
            public static void Publish(in T @event, short worldId) {
                if (worldId >= s_actions.Length) return;
                if (PublisherDelay.IsDelaying) {
                    if (!s_delayedActionQueues.TryGetValue(worldId, out var delayedActionQueue)) {
                        delayedActionQueue = new Queue<T>();
                        s_delayedActionQueues.Add(worldId, delayedActionQueue);
                    }
                    delayedActionQueue.Enqueue(@event);
                }
                else
                    s_actions[worldId]?.Invoke(@event);
            }

            private static void InvokeDelayedEvents() {
                foreach (var (worldId, delayedActionQueue) in s_delayedActionQueues) {
                    while (delayedActionQueue.TryDequeue(out var @event))
                        Publish(@event, worldId);
                }
            }
            
            private static void ClearStaticCaches() {
                s_delayedActionQueues.Clear();
                s_actions = Array.Empty<GenericHandler<T>>();
            }
        }
    }

    public static class PublisherDelay {
        internal static event Action DelayEndedEvent;
        
        internal static bool IsDelaying { get; private set; }

        public static void StartDelay() => IsDelaying = true;

        public static void EndDelay() {
            if (!IsDelaying) return;
            IsDelaying = false;
            DelayEndedEvent?.Invoke();
        }
    }
}