using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UFlow.Core.Runtime;

namespace UFlow.Addon.Ecs.Core.Runtime {
#if IL2CPP_ENABLED
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    public sealed class SparseArray<T> : IDisposable {
        private T[] m_dense;
        private int[] m_sparse;

        public int Count { get; private set; }

        public SparseArray() => Clear();

        public SparseArray(int initialCapacity) {
            initialCapacity = Math.Max(initialCapacity, 1);
            m_dense = new T[initialCapacity];
            m_sparse = new int[initialCapacity];
            for (var i = 0; i < initialCapacity; i++)
                m_sparse[i] = -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator GetEnumerator() => new Enumerator(this);

        public void Dispose() => Clear();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Set(int id, in T value) {
            UFlowUtils.Collections.EnsureIndex(ref m_sparse, id, -1);
            if (m_sparse[id] == -1) Count++;
            UFlowUtils.Collections.EnsureIndex(ref m_dense, Count);
            m_dense[Count] = value;
            m_sparse[id] = Count;
            return ref m_dense[Count];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T SetBufferValue(in T value) {
            m_dense[0] = value;
            m_sparse[0] = 0;
            return ref m_dense[0];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(int id) {
            var idx = m_sparse[id];
            var lastIdx = Count--;
            if (idx != lastIdx) {
                m_sparse[lastIdx] = idx;
                m_dense[idx] = m_dense[lastIdx];
            }

            m_sparse[id] = -1;
            m_dense[idx] = default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveBufferValue() {
            m_dense[0] = default;
            m_sparse[0] = -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Get(int id) => ref m_dense[m_sparse[id]];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetBufferValue() => ref m_dense[0];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has(int id) => id < m_sparse.Length && m_sparse[id] != -1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasBufferValue() => m_sparse[0] != -1;

        public void Clear() {
            m_dense = new T[1];
            m_sparse = new int[1];
            m_sparse[0] = -1;
            Count = 0;
        }

#if IL2CPP_ENABLED
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
        public ref struct Enumerator {
            private readonly ReadOnlySpan<T> m_values;
            private readonly int m_count;
            private int m_index;

            internal Enumerator(in SparseArray<T> sparseArray) {
                m_values = new ReadOnlySpan<T>(sparseArray.m_dense);
                m_count = sparseArray.Count;
                m_index = 0;
            }

            public T Current {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => m_values[m_index];
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext() => ++m_index <= m_count;
        }
    }
}