using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UFlow.Addon.Ecs.Core.Runtime {
    internal sealed class ArrayFieldSerializer<TObject, TField> : ISerializer<TObject> where TField : unmanaged {
        private readonly int m_offset;

        public ArrayFieldSerializer(in FieldInfo fieldInfo) => m_offset = (int)Marshal.OffsetOf<TObject>(fieldInfo.Name);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Serialize(in ByteBuffer buffer, ref TObject value) {
            var statePtr = Unsafe.AsPointer(ref value);
            var fieldPtr = (void*)((byte*)statePtr + m_offset);
            var array = Unsafe.Read<TField[]>(fieldPtr);
            buffer.Write(array.Length);
            foreach (var element in array)
                buffer.WriteUnsafe(element);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Deserialize(in ByteBuffer buffer, ref TObject value) {
            var statePtr = Unsafe.AsPointer(ref value);
            var fieldPtr = (void*)((byte*)statePtr + m_offset);
            var length = buffer.ReadInt();
            var array = new TField[length];
            for (var i = 0; i < length; i++)
                array[i] = buffer.ReadUnsafe<TField>();
            Unsafe.Write(fieldPtr, array);
        }
    }
}