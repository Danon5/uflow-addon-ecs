using System;
using System.Runtime.CompilerServices;

namespace UFlow.Addon.Ecs.Core.Runtime {
    public static partial class EcsUtils {
        internal static class Internal {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void EnsureLength<T>(ref T[] array, int length) {
                if (array.Length >= length) return;
                var oldCapacity = array.Length;
                var newCapacity = Math.Max(oldCapacity, 1);
                while (newCapacity <= length)
                    newCapacity <<= 1;
                Array.Resize(ref array, newCapacity);
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void EnsureLength<T>(ref T[] array, int length, in T defaultValue) {
                if (array.Length >= length) return;
                var oldCapacity = array.Length;
                var newCapacity = Math.Max(oldCapacity, 1);
                while (newCapacity <= length)
                    newCapacity <<= 1;
                Array.Resize(ref array, newCapacity);
                for (var i = oldCapacity; i < newCapacity; i++)
                    array[i] = defaultValue;
            }
            
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void EnsureIndex<T>(ref T[] array, int index) => EnsureLength(ref array, index + 1);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void EnsureIndex<T>(ref T[] array, int index, in T defaultValue) =>
                EnsureLength(ref array, index + 1, defaultValue);
        }
    }
}