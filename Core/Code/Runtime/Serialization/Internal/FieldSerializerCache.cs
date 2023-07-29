using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace UFlow.Addon.ECS.Core.Runtime {
    internal static class FieldSerializerCache<T> {
        private static readonly IFieldSerializer<T>[] s_serializers;

        static FieldSerializerCache() {
            var objType = typeof(T);
            var fieldSerializerType = typeof(FieldSerializer<,>);
            var fieldSerializers = new List<IFieldSerializer<T>>();
            foreach (var field in objType.GetFields()) {
                var fieldSerializerGenericType = fieldSerializerType.MakeGenericType(objType, field.FieldType);
                var fieldSerializer = Activator.CreateInstance(fieldSerializerGenericType, Marshal.OffsetOf<T>(field.Name));
                fieldSerializers.Add(fieldSerializer as IFieldSerializer<T>);
            }
            s_serializers = fieldSerializers.ToArray();
        }
        
        
    }
}