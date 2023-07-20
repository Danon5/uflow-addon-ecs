using System;
using System.Collections.Generic;
using UFlow.Core.Runtime;

namespace UFlow.Addon.Ecs.Core.Runtime {
    internal static class Systems {
        private static readonly Dictionary<short, Dictionary<Type, BaseSystemGroup>> s_groups;
        private static IDisposable[] s_subscriptions;

        static Systems() {
            s_groups = new Dictionary<short, Dictionary<Type, BaseSystemGroup>>();
            s_subscriptions = Array.Empty<IDisposable>();
            Publishers<WorldDestroyedEvent>.Global.Subscribe((in WorldDestroyedEvent @event) => ClearGroups(@event.worldId));
            ExternalEngineEvents.clearStaticCachesEvent += ClearStaticCache;
        }

        public static T GetGroup<T>(short worldId) where T : BaseSystemGroup {
            return s_groups[worldId][typeof(T)] as T;
        }
        
        public static T GetOrCreateGroup<T>(short worldId) where T : BaseSystemGroup {
            return GetOrCreateGroup(worldId, typeof(T)) as T;
        }
        
        public static BaseSystemGroup GetOrCreateGroup(short worldId, in Type type) {
            var world = Worlds.Get(worldId);
            if (!s_groups.ContainsKey(worldId)) s_groups.Add(worldId, new Dictionary<Type, BaseSystemGroup>());
            if (s_groups[worldId].TryGetValue(type, out var group)) return group;
            if (!typeof(BaseSystemGroup).IsAssignableFrom(type))
                throw new Exception($"Type {type} is not a valid SystemGroup");
            group = Activator.CreateInstance(type) as BaseSystemGroup;
            s_groups[worldId].Add(type, group);
            UFlowUtils.Collections.EnsureIndex(ref s_subscriptions, worldId);
            s_subscriptions[worldId] = new[] {
                world.WhenReset(() => {
                    ClearGroups(worldId);
                })
            }.MergeIntoGroup();
            return group;
        }

        public static void SetupGroup(short worldId, in Type type) {
            if (!s_groups.ContainsKey(worldId)) return;
            if (!s_groups[worldId].TryGetValue(type, out var group)) return;
            group.Setup();
        }
        
        public static void SetupGroup<T>(short worldId) {
            var type = typeof(T);
            if (!s_groups.ContainsKey(worldId)) return;
            if (!s_groups[worldId].TryGetValue(type, out var group)) return;
            group.Setup();
        }
        
        public static void RunGroup(short worldId, in Type type) {
            if (!s_groups.ContainsKey(worldId)) return;
            if (!s_groups[worldId].TryGetValue(type, out var group)) return;
            group.Run();
        }
        
        public static void RunGroup<T>(short worldId) {
            var type = typeof(T);
            if (!s_groups.ContainsKey(worldId)) return;
            if (!s_groups[worldId].TryGetValue(type, out var group)) return;
            group.Run();
        }
        
        public static void CleanupGroup(short worldId, in Type type) {
            if (!s_groups.ContainsKey(worldId)) return;
            if (!s_groups[worldId].TryGetValue(type, out var group)) return;
            group.Cleanup();
        }
        
        public static void CleanupGroup<T>(short worldId) {
            var type = typeof(T);
            if (!s_groups.ContainsKey(worldId)) return;
            if (!s_groups[worldId].TryGetValue(type, out var group)) return;
            group.Cleanup();
        }

        public static void SetupGroups(short worldId) {
            if (!s_groups.TryGetValue(worldId, out var groups)) return;
            foreach (var group in groups)
                group.Value.Setup();
        }
        
        public static void CleanupGroups(short worldId) {
            if (!s_groups.TryGetValue(worldId, out var groups)) return;
            foreach (var group in groups)
                group.Value.Cleanup();
        }
        
        internal static void SortSystems(short worldId) {
            if (!s_groups.TryGetValue(worldId, out var groups)) return;
            foreach (var group in groups)
                group.Value.Sort();
        }

        private static void ClearGroups(short worldId) {
            if (!s_groups.ContainsKey(worldId)) return;
            foreach (var group in s_groups[worldId])
                group.Value.Cleanup();
            s_groups[worldId].Clear();
        }

        private static void ClearStaticCache() {
            s_groups.Clear();
        }
    }
}