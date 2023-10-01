using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UFlow.Addon.ECS.Core.Runtime {
    internal static class Systems {
        private static readonly Dictionary<short, Dictionary<Type, BaseSystemGroup>> s_groups;

        static Systems() {
            s_groups = new Dictionary<short, Dictionary<Type, BaseSystemGroup>>();
            Publishers<WorldDestroyedEvent>.Global.Subscribe((in WorldDestroyedEvent @event) => ClearGroups(@event.worldId));
            ExternalEngineEvents.clearStaticCachesEvent += ClearStaticCache;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetGroup<T>(short worldId) where T : BaseSystemGroup => s_groups[worldId][typeof(T)] as T;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetOrCreateGroup<T>(short worldId) where T : BaseSystemGroup => GetOrCreateGroup(worldId, typeof(T)) as T;
        
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetupGroup(short worldId, in Type type) {
            if (!s_groups.ContainsKey(worldId)) return;
            if (!s_groups[worldId].TryGetValue(type, out var group)) return;
            group.Setup();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetupGroup<T>(short worldId) where T : BaseSystemGroup => SetupGroup(worldId, typeof(T));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetGroupEnabled(short worldId, in Type type, bool value) {
            if (!s_groups.ContainsKey(worldId)) return;
            if (!s_groups[worldId].TryGetValue(type, out var group)) return;
            group.SetEnabled(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetGroupEnabled<T>(short worldId, bool value) where T : BaseSystemGroup => 
            SetGroupEnabled(worldId, typeof(T), value);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnableGroup(short worldId, in Type type) {
            if (!s_groups.ContainsKey(worldId)) return;
            if (!s_groups[worldId].TryGetValue(type, out var group)) return;
            group.Enable();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnableGroup<T>(short worldId) where T : BaseSystemGroup => EnableGroup(worldId, typeof(T));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DisableGroup(short worldId, in Type type) {
            if (!s_groups.ContainsKey(worldId)) return;
            if (!s_groups[worldId].TryGetValue(type, out var group)) return;
            group.Disable();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DisableGroup<T>(short worldId) where T : BaseSystemGroup => DisableGroup(worldId, typeof(T));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsGroupEnabled(short worldId, in Type type) => s_groups.ContainsKey(worldId) &&
            s_groups[worldId].TryGetValue(type, out var group) && group.IsEnabled();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsGroupEnabled<T>(short worldId) where T : BaseSystemGroup => IsGroupEnabled(worldId, typeof(T));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RunGroup(short worldId, in Type type) {
            if (!s_groups.ContainsKey(worldId)) return;
            if (!s_groups[worldId].TryGetValue(type, out var group) || !group.IsEnabled()) return;
            group.Run();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RunGroup<T>(short worldId) where T : BaseSystemGroup => RunGroup(worldId, typeof(T));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CleanupGroup(short worldId, in Type type) {
            if (!s_groups.ContainsKey(worldId)) return;
            if (!s_groups[worldId].TryGetValue(type, out var group)) return;
            group.Cleanup();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CleanupGroup<T>(short worldId) where T : BaseSystemGroup => CleanupGroup(worldId, typeof(T));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ResetGroup(short worldId, in Type type) {
            if (!s_groups.ContainsKey(worldId)) return;
            if (!s_groups[worldId].TryGetValue(type, out var group)) return;
            group.Reset();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ResetGroup<T>(short worldId) where T : BaseSystemGroup => SetupGroup(worldId, typeof(T));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetupGroups(short worldId) {
            if (!s_groups.TryGetValue(worldId, out var groups)) return;
            foreach (var group in groups)
                group.Value.Setup();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CleanupGroups(short worldId) {
            if (!s_groups.TryGetValue(worldId, out var groups)) return;
            foreach (var group in groups)
                group.Value.Cleanup();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            foreach (var group in s_groups)
                group.Value.Clear();
            s_groups.Clear();
        }
    }
}