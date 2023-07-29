using System;
using System.Linq;
using UFlow.Core.Runtime;

namespace UFlow.Addon.ECS.Core.Runtime {
    internal static class SerializerCache<T> {
        private static readonly ISerializer<T> s_serializer;

        static SerializerCache() {
            var type = UFlowUtils.Reflection.GetInheritors<ISerializer<T>>().FirstOrDefault();
            if (type == null) return;
            s_serializer = Activator.CreateInstance(type) as ISerializer<T>;
        }

        public static bool TryGet(out ISerializer<T> serializer) {
            if (s_serializer == null) {
                serializer = default;
                return false;
            }
            serializer = s_serializer;
            return true;
        }
        
        public static bool TryGetWithThrowOnFailure(out ISerializer<T> serializer) {
            if (s_serializer == null) {
                serializer = default;
                throw new Exception($"No serializer exists for type {typeof(T)}");
            }
            serializer = s_serializer;
            return true;
        }
    }
}