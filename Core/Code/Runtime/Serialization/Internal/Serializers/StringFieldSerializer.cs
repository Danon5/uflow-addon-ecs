using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UFlow.Addon.ECS.Core.Runtime {
    internal sealed class StringFieldSerializer<TObject> : ISerializer<TObject> {
        private readonly int m_offset;

        public StringFieldSerializer(in FieldInfo fieldInfo) => m_offset = (int)Marshal.OffsetOf<string>(fieldInfo.Name);
        public unsafe void Serialize(in ByteBuffer buffer, ref TObject value) {
            var objPtr = Unsafe.AsPointer(ref value);
            var fieldPtr = (void*)((byte*)objPtr + m_offset);
            var fieldValue = Unsafe.Read<string>(fieldPtr);
            buffer.Write(fieldValue);
        }
        public unsafe void Deserialize(in ByteBuffer buffer, ref TObject value) {
            var objPtr = Unsafe.AsPointer(ref value);
            var fieldPtr = (void*)((byte*)objPtr + m_offset);
            Unsafe.Write(fieldPtr, buffer.ReadString());
        }
    }
}