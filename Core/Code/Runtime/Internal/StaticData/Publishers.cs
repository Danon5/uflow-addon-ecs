using System;
using UFlow.Core.Runtime;

namespace UFlow.Addon.Ecs.Core.Runtime {
#if IL2CPP_ENABLED
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    internal static class Publishers<T> {
        public static class Global {
            private static GenericHandler<T> s_action;

            static Global() {
                ExternalEngineEvents.clearStaticCachesEvent += ClearStaticCaches;
            }
            
            public static IDisposable Subscribe(in GenericHandler<T> action) {
                s_action += action;
                return new GlobalSubscription<T>(action);
            }
            
            public static void Unsubscribe(in GlobalSubscription<T> subscription) {
                s_action -= subscription.action;
            }

            public static void Publish(in T @event) {
                s_action?.Invoke(@event);
            }

            private static void ClearStaticCaches() {
                s_action = null;
            }
        }

        public static class WorldInstance {
            private static GenericHandler<T>[] s_actions;

            static WorldInstance() {
                s_actions = Array.Empty<GenericHandler<T>>();
                ExternalEngineEvents.clearStaticCachesEvent += ClearStaticCaches;
            }
            
            public static IDisposable Subscribe(in GenericHandler<T> action, int worldId) {
                UFlowUtils.Collections.EnsureIndex(ref s_actions, worldId);
                s_actions[worldId] += action;
                return new WorldInstanceSubscription<T>(action, worldId);
            }

            public static void Unsubscribe(in WorldInstanceSubscription<T> subscription) {
                s_actions[subscription.worldId] -= subscription.action;
            }
            
            public static void Publish(in T @event, int worldId) {
                if (worldId >= s_actions.Length) return;
                s_actions[worldId]?.Invoke(@event);
            }
            
            private static void ClearStaticCaches() {
                for (var i = 0; i < s_actions.Length; i++)
                    s_actions[i] = null;
                s_actions = Array.Empty<GenericHandler<T>>();
            }
        }
    }
}