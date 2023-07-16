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
        public void Write(byte value) => WriteInternal(value);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(sbyte value) => WriteInternal((byte)value);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(short value) => WriteInternal(value);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ushort value) => WriteInternal((short)value);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(int value) => WriteInternal(value);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(uint value) => WriteInternal((int)value);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(long value) => WriteInternal(value);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ulong value) => WriteInternal((long)value);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(in Span<byte> values) {
            EnsureLength(ref m_buffer, Cursor + values.Length);
            foreach (var value in values)
                Write(value);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(in Span<sbyte> values) {
            EnsureLength(ref m_buffer, Cursor + values.Length);
            foreach (var value in values)
                Write(value);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(in Span<short> values) {
            EnsureLength(ref m_buffer, Cursor + values.Length);
            foreach (var value in values)
                Write(value);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(in Span<ushort> values) {
            EnsureLength(ref m_buffer, Cursor + values.Length);
            foreach (var value in values)
                Write(value);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(in Span<int> values) {
            EnsureLength(ref m_buffer, Cursor + values.Length);
            foreach (var value in values)
                Write(value);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(in Span<uint> values) {
            EnsureLength(ref m_buffer, Cursor + values.Length);
            foreach (var value in values)
                Write(value);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(in Span<long> values) {
            EnsureLength(ref m_buffer, Cursor + values.Length);
            foreach (var value in values)
                Write(value);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(in Span<ulong> values) {
            EnsureLength(ref m_buffer, Cursor + values.Length);
            foreach (var value in values)
                Write(value);
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
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteInternal(byte value) {
            EnsureLength(ref m_buffer, Cursor + 1);
            m_buffer[Cursor] = value;
            Cursor += 1;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteInternal(short value) {
            EnsureLength(ref m_buffer, Cursor + 2);
            if (m_littleEndian) {
                m_buffer[Cursor] = (byte)value;
                m_buffer[Cursor + 1] = (byte)(value >> 8);
            }
            else {
                m_buffer[Cursor + 1] = (byte)value;
                m_buffer[Cursor] = (byte)(value >> 8);
            }
            Cursor += 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteInternal(int value) {
            EnsureLength(ref m_buffer, Cursor + 4);
            if (m_littleEndian) {
                m_buffer[Cursor] = (byte)value;
                m_buffer[Cursor + 1] = (byte)(value >> 8);
                m_buffer[Cursor + 2] = (byte)(value >> 16);
                m_buffer[Cursor + 3] = (byte)(value >> 24);
            }
            else {
                m_buffer[Cursor + 3] = (byte)value;
                m_buffer[Cursor + 2] = (byte)(value >> 8);
                m_buffer[Cursor + 1] = (byte)(value >> 16);
                m_buffer[Cursor] = (byte)(value >> 24);
            }
            Cursor += 4;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteInternal(long value) {
            EnsureLength(ref m_buffer, Cursor + 8);
            if (m_littleEndian) {
                m_buffer[Cursor] = (byte)value;
                m_buffer[Cursor + 1] = (byte)(value >> 8);
                m_buffer[Cursor + 2] = (byte)(value >> 16);
                m_buffer[Cursor + 3] = (byte)(value >> 24);
                m_buffer[Cursor + 4] = (byte)(value >> 32);
                m_buffer[Cursor + 5] = (byte)(value >> 40);
                m_buffer[Cursor + 6] = (byte)(value >> 48);
                m_buffer[Cursor + 7] = (byte)(value >> 56);
            }
            else {
                m_buffer[Cursor + 7] = (byte)value;
                m_buffer[Cursor + 6] = (byte)(value >> 8);
                m_buffer[Cursor + 5] = (byte)(value >> 16);
                m_buffer[Cursor + 4] = (byte)(value >> 24);
                m_buffer[Cursor + 3] = (byte)(value >> 32);
                m_buffer[Cursor + 2] = (byte)(value >> 40);
                m_buffer[Cursor + 1] = (byte)(value >> 48);
                m_buffer[Cursor] = (byte)(value >> 56);
            }
            Cursor += 8;
        }
    }
}