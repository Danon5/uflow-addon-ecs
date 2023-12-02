using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UFlow.Core.Runtime;

namespace UFlow.Addon.ECS.Core.Runtime {
    internal static class RawComponentMethodCache {
        private static readonly Dictionary<Type, IRawComponentMethods> s_setCache = new();
        private static readonly Dictionary<Type, IRawComponentMethods> s_getCache = new();
        private static readonly Dictionary<Type, IRawComponentMethods> s_hasCache = new();
        private static readonly Dictionary<Type, IRawComponentMethods> s_removeCache = new();
        private static readonly Dictionary<Type, IRawComponentMethods> s_tryRemoveCache = new();
        private static readonly Dictionary<Type, IRawComponentMethods> s_setEnabledCache = new();
        private static readonly Dictionary<Type, IRawComponentMethods> s_isEnabledCache = new();
        private static readonly Dictionary<Type, IRawComponentMethods> s_setWithoutEventsCache = new();
        private static readonly Dictionary<Type, IRawComponentMethods> s_invokeAddedEventsCache = new();
        private static readonly Dictionary<Type, IRawComponentMethods> s_invokeEnabledEventsCache = new();

        static RawComponentMethodCache() => UnityGlobalEventHelper.RuntimeInitializeOnLoad += ClearStaticCache;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InvokeSet(in Entity entity, Type componentType, IEcsComponent value, bool enableIfAdded) =>
            GetOrCreateRawMethod(s_setCache, componentType).InvokeSet(entity, value, enableIfAdded);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEcsComponent InvokeGet(in Entity entity, Type componentType) =>
            GetOrCreateRawMethod(s_getCache, componentType).InvokeGet(entity);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InvokeHas(in Entity entity, Type componentType) =>
            GetOrCreateRawMethod(s_hasCache, componentType).InvokeHas(entity);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InvokeRemove(in Entity entity, Type componentType) =>
            GetOrCreateRawMethod(s_removeCache, componentType).InvokeRemove(entity);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InvokeTryRemove(in Entity entity, Type componentType) =>
            GetOrCreateRawMethod(s_tryRemoveCache, componentType).InvokeTryRemove(entity);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InvokeSetEnabled(in Entity entity, Type componentType, bool enabled) =>
            GetOrCreateRawMethod(s_setEnabledCache, componentType).InvokeSetEnabled(entity, enabled);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InvokeIsEnabled(in Entity entity, Type componentType) =>
            GetOrCreateRawMethod(s_isEnabledCache, componentType).InvokeIsEnabled(entity);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void InvokeSetWithoutEvents(in Entity entity, Type componentType, IEcsComponent value, bool enableIfAdded) =>
            GetOrCreateRawMethod(s_setWithoutEventsCache, componentType).InvokeSetWithoutEvents(entity, value, enableIfAdded);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void InvokeAddedEvents(in Entity entity, Type componentType) =>
            GetOrCreateRawMethod(s_invokeAddedEventsCache, componentType).InvokeAddedEvents(entity);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void InvokeEnabledEvents(in Entity entity, Type componentType, bool enabled) =>
            GetOrCreateRawMethod(s_invokeEnabledEventsCache, componentType).InvokeEnabledEvents(entity, enabled);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Type GetComponentMethodType(Type componentType) => typeof(RawComponentMethods<>).MakeGenericType(componentType);

        private static IRawComponentMethods GetOrCreateRawMethod(IDictionary<Type, IRawComponentMethods> cache, Type componentType) {
            if (cache.TryGetValue(componentType, out var rawMethod)) return rawMethod;
            rawMethod = Activator.CreateInstance(GetComponentMethodType(componentType)) as IRawComponentMethods;
            cache.Add(componentType, rawMethod);
            return rawMethod;
        }

        private static void ClearStaticCache() {
            s_setCache.Clear();
            s_getCache.Clear();
            s_hasCache.Clear();
            s_removeCache.Clear();
            s_tryRemoveCache.Clear();
            s_setEnabledCache.Clear();
            s_isEnabledCache.Clear();
            s_setWithoutEventsCache.Clear();
        }
    }
}