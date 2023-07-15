using System;
using System.Runtime.CompilerServices;

namespace UFlow.Addon.Ecs.Core.Runtime {
    public sealed class ByteBuffer {
        private const int c_default_capacity = 4096;
        private readonly bool m_autoResize;
        private readonly bool m_littleEndian;
        private byte[] m_buffer;
        
        public int Cursor { get; private set; }
        public int Capacity { get; private set; }

        public ByteBuffer() {
            m_autoResize = false;
            Capacity = c_default_capacity;
            m_buffer = new byte[Capacity];
            m_littleEndian = BitConverter.IsLittleEndian;
        }

        public ByteBuffer(bool autoResize) {
            m_autoResize = autoResize;
            Capacity = c_default_capacity;
            m_buffer = new byte[Capacity];
            m_littleEndian = BitConverter.IsLittleEndian;
        }
        
        public ByteBuffer(int capacity) {
            m_autoResize = false;
            Capacity = capacity;
            m_buffer = new byte[Capacity];
            m_littleEndian = BitConverter.IsLittleEndian;
        }
        
        public ByteBuffer(bool autoResize, int initialCapacity) {
            m_autoResize = autoResize;
            Capacity = initialCapacity;
            m_buffer = new byte[Capacity];
            m_littleEndian = BitConverter.IsLittleEndian;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write<T>(in T value) where T : unmanaged {
            var size = sizeof(T);
            if (m_autoResize)
                Capacity = EnsureLength(ref m_buffer, Cursor + size);
            if (!m_littleEndian) {
                fixed (T* valuePtr = &value) {
                    var bytePtr = (byte*)valuePtr;
                    for (var i = 0; i < size; i++)
                        m_buffer[Cursor + i] = bytePtr[size - 1 - i];
                }
            }
            else {
                fixed (byte* bytePtr = &m_buffer[Cursor])
                    *(T*)bytePtr = value;
            }
            Cursor += size;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe T Read<T>() where T : unmanaged {
            var size = sizeof(T);
            T value;
            fixed (byte* bytePtr = &m_buffer[Cursor]) {
                if (!m_littleEndian) {
                    for (var i = 0; i < size / 2; i++)
                        (bytePtr[i], bytePtr[size - 1 - i]) = (bytePtr[size - 1 - i], bytePtr[i]);
                }
                
                value = *(T*)bytePtr;
            }
            Cursor += size;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Complete() => Cursor = 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> GetBytesToCursor() => new(m_buffer, 0, Cursor + 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int EnsureLength<T>(ref T[] array, int length, int maxLength = int.MaxValue) {
            if (array.Length >= length)
                return array.Length;

            var oldCapacity = array.Length;
            var newCapacity = Math.Max(oldCapacity, 1);
            while (newCapacity <= length && newCapacity <= maxLength)
                newCapacity <<= 1;
            newCapacity = Math.Min(newCapacity, maxLength);

            Array.Resize(ref array, newCapacity);
            return newCapacity;
        }
    }
}