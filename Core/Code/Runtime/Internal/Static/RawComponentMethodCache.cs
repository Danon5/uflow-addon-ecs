using System;
using System.Collections.Generic;
using UFlow.Core.Runtime;

namespace UFlow.Addon.ECS.Core.Runtime {
    internal static class RawComponentMethodCache {
        private static readonly Dictionary<Type, IRawComponentMethod> s_setCache = new();
        private static readonly Dictionary<Type, IRawComponentMethod> s_getCache = new();
        private static readonly Dictionary<Type, IRawComponentMethod> s_hasCache = new();
        private static readonly Dictionary<Type, IRawComponentMethod> s_removeCache = new();
        private static readonly Dictionary<Type, IRawComponentMethod> s_tryRemoveCache = new();
        private static readonly Dictionary<Type, IRawComponentMethod> s_setEnabledCache = new();
        private static readonly Dictionary<Type, IRawComponentMethod> s_isEnabledCache = new();

        static RawComponentMethodCache() => UnityGlobalEventHelper.RuntimeInitializeOnLoad += ClearStaticCache;

        public static void InvokeSet(in Entity entity, Type componentType, IEcsComponent value, bool enableIfAdded) =>
            GetOrCreateRawMethod(s_setCache, componentType).InvokeSet(entity, value, enableIfAdded);
        
        public static IEcsComponent InvokeGet(in Entity entity, Type componentType) =>
            GetOrCreateRawMethod(s_getCache, componentType).InvokeGet(entity);
        
        public static bool InvokeHas(in Entity entity, Type componentType) =>
            GetOrCreateRawMethod(s_hasCache, componentType).InvokeHas(entity);

        public static void InvokeRemove(in Entity entity, Type componentType) =>
            GetOrCreateRawMethod(s_removeCache, componentType).InvokeRemove(entity);

        public static bool InvokeTryRemove(in Entity entity, Type componentType) =>
            GetOrCreateRawMethod(s_tryRemoveCache, componentType).InvokeTryRemove(entity);
        
        public static void InvokeSetEnabled(in Entity entity, Type componentType, bool enabled) =>
            GetOrCreateRawMethod(s_setEnabledCache, componentType).InvokeSetEnabled(entity, enabled);
        
        public static bool InvokeIsEnabled(in Entity entity, Type componentType) =>
            GetOrCreateRawMethod(s_isEnabledCache, componentType).InvokeIsEnabled(entity);

        private static Type GetComponentMethodType(Type componentType) => typeof(RawComponentMethod<>).MakeGenericType(componentType);

        private static IRawComponentMethod GetOrCreateRawMethod(IDictionary<Type, IRawComponentMethod> cache, Type componentType) {
            if (cache.TryGetValue(componentType, out var rawMethod)) return rawMethod;
            rawMethod = Activator.CreateInstance(GetComponentMethodType(componentType)) as IRawComponentMethod;
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
        }
    }
}