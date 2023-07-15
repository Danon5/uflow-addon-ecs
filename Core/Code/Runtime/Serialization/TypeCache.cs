using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UFlow.Core.Runtime;

namespace UFlow.Addon.Ecs.Core.Runtime {
    internal sealed class TypeCache {
        private readonly Dictionary<ushort, Type> m_idToTypeMap = new();
        private readonly Dictionary<Type, ushort> m_typeToIdMap = new();

        public void RegisterTypes() {
            foreach (var type in UFlowUtils.Reflection.GetAllTypesWithAttribute<UFlowSerializableAttribute>()) {
                var id = type.GetCustomAttribute<UFlowSerializableAttribute>().id;
                m_idToTypeMap.Add(id, type);
                m_typeToIdMap.Add(type, id);
            }
        }
        
        public void RegisterTypesWithFieldAttribute<T>() where T : Attribute {
            foreach (var type in UFlowUtils.Reflection.GetAllTypesWithAttribute<UFlowSerializableAttribute>()) {
                if (!UFlowUtils.Reflection.HasFieldAttributeInType<T>(type)) continue;
                var id = type.GetCustomAttribute<UFlowSerializableAttribute>().id;
                m_idToTypeMap.Add(id, type);
                m_typeToIdMap.Add(type, id);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Type GetTypeFromId(ushort id) => m_idToTypeMap[id];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort GetIdFromType(in Type type) => m_typeToIdMap[type];
    }
}