using System;
using System.Runtime.CompilerServices;
using UFlow.Core.Shared;

namespace UFlow.Addon.Entities.Core.Runtime {
#if UFLOW_IL2CPP_ENABLED
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    internal sealed class SparseList<T> {
        private T[] m_dense;
        private int[] m_sparse;
        private int[] m_denseToSparse;

        public int Count { get; private set; }

        public SparseList(int initialCapacity = 0) {
            initialCapacity = Math.Max(initialCapacity, 0);
            m_dense = new T[initialCapacity];
            m_sparse = new int[initialCapacity];
            m_denseToSparse = new int[initialCapacity];
            Count = 0;
            for (var i = 0; i < initialCapacity; i++)
                m_sparse[i] = -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator GetEnumerator() => new(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Set(int id, in T value) {
            UFlowUtils.Collections.EnsureIndex(ref m_sparse, id, -1);
            if (m_sparse[id] == -1)
                Count++;
            UFlowUtils.Collections.EnsureIndex(ref m_dense, Count);
            UFlowUtils.Collections.EnsureIndex(ref m_denseToSparse, Count, -1);
            m_dense[Count] = value;
            m_denseToSparse[Count] = id;
            m_sparse[id] = Count;
            return ref m_dense[Count];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(int id) {
            var denseIdx = m_sparse[id];
            var lastDenseIdx = Count--;
            if (denseIdx == lastDenseIdx) {
                m_dense[denseIdx] = default;
                m_sparse[id] = -1;
                m_denseToSparse[denseIdx] = -1;
            }
            else {
                var lastSparseIndex = m_denseToSparse[lastDenseIdx];
                m_dense[denseIdx] = m_dense[lastDenseIdx];
                m_sparse[id] = -1;
                m_denseToSparse[denseIdx] = lastSparseIndex;
                m_dense[lastDenseIdx] = default;
                m_sparse[lastSparseIndex] = denseIdx;
                m_denseToSparse[lastDenseIdx] = -1;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Get(int id) => ref m_dense[m_sparse[id]];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has(int id) => id < m_sparse.Length && m_sparse[id] != -1;

        public void Clear() {
            m_dense = Array.Empty<T>();
            m_sparse = Array.Empty<int>();
            m_denseToSparse = Array.Empty<int>();
            m_sparse[0] = -1;
            m_denseToSparse[0] = -1;
            Count = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T First() => ref m_dense[0];

#if UFLOW_IL2CPP_ENABLED
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
        public ref struct Enumerator {
            private readonly ReadOnlySpan<T> m_values;
            private readonly int m_count;
            private int m_index;

            internal Enumerator(in SparseList<T> sparseArray) {
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