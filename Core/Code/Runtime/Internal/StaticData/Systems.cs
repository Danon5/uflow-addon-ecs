using System;
using System.Collections.Generic;

namespace UFlow.Addon.ECS.Core.Runtime {
    internal static class Systems {
        private static readonly Dictionary<short, Dictionary<Type, BaseSystemGroup>> s_groups;

        static Systems() {
            s_groups = new Dictionary<short, Dictionary<Type, BaseSystemGroup>>();
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
            if (!s_groups.ContainsKey(worldId)) s_groups.Add(worldId, new Dictionary<Type, BaseSystemGroup>());
            if (s_groups[worldId].TryGetValue(type, out var group)) return group;
            if (!typeof(BaseSystemGroup).IsAssignableFrom(type))
                throw new Exception($"Type {type} is not a valid SystemGroup");
            group = Activator.CreateInstance(type) as BaseSystemGroup;
            group!.SetEnabled(true);
            s_groups[worldId].Add(type, group);
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

        public static void SetGroupEnabled(short worldId, in Type type, bool value) {
            if (!s_groups.ContainsKey(worldId)) return;
            if (!s_groups[worldId].TryGetValue(type, out var group)) return;
            group.SetEnabled(value);
        }
        
        public static void EnableGroup(short worldId, in Type type) {
            if (!s_groups.ContainsKey(worldId)) return;
            if (!s_groups[worldId].TryGetValue(type, out var group)) return;
            group.Enable();
        }
        
        public static void DisableGroup(short worldId, in Type type) {
            if (!s_groups.ContainsKey(worldId)) return;
            if (!s_groups[worldId].TryGetValue(type, out var group)) return;
            group.Disable();
        }
        
        public static bool IsGroupEnabled(short worldId, in Type type) {
            if (!s_groups.ContainsKey(worldId)) return false;
            if (!s_groups[worldId].TryGetValue(type, out var group)) return false;
            return group.IsEnabled();
        }
        
        public static void RunGroup(short worldId, in Type type) {
            if (!s_groups.ContainsKey(worldId)) return;
            if (!s_groups[worldId].TryGetValue(type, out var group) || !group.IsEnabled()) return;
            group.Run();
        }
        
        public static void RunGroup<T>(short worldId) {
            var type = typeof(T);
            if (!s_groups.ContainsKey(worldId)) return;
            if (!s_groups[worldId].TryGetValue(type, out var group) || !group.IsEnabled()) return;
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

        public static void ResetGroup<T>(short worldId) {
            var type = typeof(T);
            if (!s_groups.ContainsKey(worldId)) return;
            if (!s_groups[worldId].TryGetValue(type, out var group)) return;
            group.Reset();
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

        public static void ResetGroups(short worldId) {
            if (!s_groups.TryGetValue(worldId, out var groups)) return;
            foreach (var group in groups)
                group.Value.Reset();
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