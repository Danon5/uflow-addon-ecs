using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
// ReSharper disable StaticMemberInGenericType

namespace UFlow.Addon.ECS.Core.Runtime {
    internal static class FieldSerializerCache<T> {
        private static readonly SerializerEntry[] s_entries;
        private static readonly Dictionary<Type, int[]> s_attributeIndexMap = new();

        static FieldSerializerCache() {
            var objType = typeof(T);
            var fieldSerializerType = typeof(FieldSerializer<,>);
            var fieldSerializers = new List<SerializerEntry>();
            foreach (var fieldInfo in objType.GetFields()) {
                var fieldSerializerGenericType = fieldSerializerType.MakeGenericType(objType, fieldInfo.FieldType);
                var fieldSerializer = Activator.CreateInstance(fieldSerializerGenericType, Marshal.OffsetOf<T>(fieldInfo.Name));
                fieldSerializers.Add(new SerializerEntry {
                    fieldInfo = fieldInfo,
                    serializer = fieldSerializer as IFieldSerializer<T>
                });
            }
            s_entries = fieldSerializers.ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerator<IFieldSerializer<T>> GetEnumerator() {
            foreach (var entry in s_entries)
                yield return entry.serializer;
        }
        
        public static IEnumerator<IFieldSerializer<T>> GetWithAttributeEnumerator<TAttribute>() where TAttribute : Attribute {
            var attributeType = typeof(TAttribute);
            if (!s_attributeIndexMap.TryGetValue(attributeType, out var indexMap)) {
                var indices = new List<int>();
                for (var i = 0; i < s_entries.Length; i++) {
                    var entry = s_entries[i];
                    if (entry.fieldInfo.GetCustomAttribute<TAttribute>() == null) continue;
                    indices.Add(i);
                }
                indexMap = indices.ToArray();
                s_attributeIndexMap.Add(attributeType, indexMap);
            }
            foreach (var index in indexMap)
                yield return s_entries[index].serializer;
        }

        private struct SerializerEntry {
            public FieldInfo fieldInfo;
            public IFieldSerializer<T> serializer;
        }
    }
}