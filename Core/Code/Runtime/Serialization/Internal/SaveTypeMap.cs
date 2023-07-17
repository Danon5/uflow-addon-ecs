using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UFlow.Core.Runtime;

namespace UFlow.Addon.Ecs.Core.Runtime {
    internal sealed class SaveTypeMap {
        private readonly Dictionary<Type, int> m_typeToHash = new();
        private readonly Dictionary<int, Type> m_hashToType = new();

        public void RegisterTypes() {
            foreach (var type in UFlowUtils.Reflection.GetAllTypesWithAttribute<EcsSerializableAttribute>()) {
                var hash = type.GetCustomAttribute<EcsSerializableAttribute>().id.GetHashCode();
                m_typeToHash[type] = hash;
                m_hashToType[hash] = type;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetHash(in Type type) => m_typeToHash[type];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Type GetType(int hash) => m_hashToType[hash];
    }
}