using System.Runtime.CompilerServices;

namespace UFlow.Addon.ECS.Core.Runtime {
    internal sealed class FieldSerializer<TObject, TField> : IFieldSerializer<TObject> {
        private readonly int m_offset;

        public FieldSerializer(int offset) => m_offset = offset;

        public unsafe void Serialize(in ByteBuffer buffer, ref TObject value) {
            if (!SerializerCache<TField>.TryGetWithThrowOnFailure(out var serializer)) return;
            var objPtr = Unsafe.AsPointer(ref value);
            var fieldPtr = (void*)((byte*)objPtr + m_offset);
            var fieldValue = Unsafe.Read<TField>(fieldPtr);
            serializer.Serialize(buffer, ref fieldValue);
        }
        
        public unsafe void Deserialize(in ByteBuffer buffer, ref TObject value) {
            if (!SerializerCache<TField>.TryGetWithThrowOnFailure(out var serializer)) return;
            var objPtr = Unsafe.AsPointer(ref value);
            var fieldPtr = (void*)((byte*)objPtr + m_offset);
            var fieldValue = Unsafe.Read<TField>(fieldPtr);
            serializer.Deserialize(buffer, ref fieldValue);
        }
    }
}