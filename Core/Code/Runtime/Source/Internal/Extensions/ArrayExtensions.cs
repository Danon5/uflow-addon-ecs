using System;
using System.Runtime.CompilerServices;

namespace UFlow.Core.Runtime.Extensions {
    internal static class ArrayExtensions {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void EnsureLength<T>(ref T[] array, int length) {
            if (array.Length >= length)
                return;

            var oldCapacity = array.Length;
            var newCapacity = Math.Max(oldCapacity, 1);
            while (newCapacity <= length)
                newCapacity <<= 1;

            Array.Resize(ref array, newCapacity);
        }
    }
}