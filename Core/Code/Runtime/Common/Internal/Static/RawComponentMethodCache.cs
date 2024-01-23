using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UFlow.Core.Runtime;
using UFlow.Core.Shared;

namespace UFlow.Addon.ECS.Core.Runtime {
    internal static class RawComponentMethodCache {
        private static readonly Dictionary<Type, IRawComponentMethods> s_cache = new();

        static RawComponentMethodCache() => StaticEventBus<RuntimeInitializeOnLoadEvent>.Subscribe(ClearStaticCache);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InvokeSet(in Entity entity, Type componentType, IEcsComponent value, bool enableIfAdded) =>
            GetOrCreateRawMethods(s_cache, componentType).InvokeSet(entity, value, enableIfAdded);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InvokeSet(in Entity entity, Type componentType, bool enableIfAdded) =>
            GetOrCreateRawMethods(s_cache, componentType).InvokeSet(entity, enableIfAdded);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEcsComponent InvokeGet(in Entity entity, Type componentType) =>
            GetOrCreateRawMethods(s_cache, componentType).InvokeGet(entity);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InvokeHas(in Entity entity, Type componentType) =>
            GetOrCreateRawMethods(s_cache, componentType).InvokeHas(entity);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InvokeRemove(in Entity entity, Type componentType) =>
            GetOrCreateRawMethods(s_cache, componentType).InvokeRemove(entity);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InvokeTryRemove(in Entity entity, Type componentType) =>
            GetOrCreateRawMethods(s_cache, componentType).InvokeTryRemove(entity);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InvokeSetEnabled(in Entity entity, Type componentType, bool enabled) =>
            GetOrCreateRawMethods(s_cache, componentType).InvokeSetEnabled(entity, enabled);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InvokeIsEnabled(in Entity entity, Type componentType) =>
            GetOrCreateRawMethods(s_cache, componentType).InvokeIsEnabled(entity);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Type GetComponentMethodType(Type componentType) => typeof(RawComponentMethods<>).MakeGenericType(componentType);

        private static IRawComponentMethods GetOrCreateRawMethods(IDictionary<Type, IRawComponentMethods> cache, Type componentType) {
            if (cache.TryGetValue(componentType, out var rawMethod)) return rawMethod;
            rawMethod = Activator.CreateInstance(GetComponentMethodType(componentType)) as IRawComponentMethods;
            cache.Add(componentType, rawMethod);
            return rawMethod;
        }

        private static void ClearStaticCache() => s_cache.Clear();
    }
}