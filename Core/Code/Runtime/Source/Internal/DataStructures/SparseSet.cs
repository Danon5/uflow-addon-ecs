using System;
using System.Runtime.CompilerServices;
using UFlow.Core.Runtime.Extensions;

namespace UFlow.Core.Runtime.DataStructures {
#if IL2CPP_ENABLED
    using Ecs;
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    internal sealed class SparseSet<T> {
        private T[] m_dense;
        private int[] m_sparse;
        private int[] m_recycledDenseIndices;
        private int m_recycledDenseIndexCount;

        public int Count { get; private set; }

        public SparseSet() {
            m_dense = new T[1];
            m_sparse = new int[1];
            m_recycledDenseIndices = new int[1];
        }

        public SparseSet(int initialCapacity) {
            m_dense = new T[initialCapacity];
            m_sparse = new int[initialCapacity];
            m_recycledDenseIndices = Array.Empty<int>();
        }

        /// <summary>
        /// Gets the data at a given index by reference.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Get(int id) {
            return ref m_dense[m_sparse[id]];
        }

        /// <summary>
        /// Sets the data at a given index.
        /// </summary>
        public void Set(int id, in T data) {
            if (id < Count) {
                var denseIndex = m_sparse[id];
                if (denseIndex > 0)
                    m_dense[denseIndex] = data;
                else
                    Add(id, data);
            }
            else
                Add(id, data);
        }

        /// <summary>
        /// Returns whether a value exists for a given index.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has(int id) {
            return id <= Count && m_sparse[id] > 0;
        }

        /// <summary>
        /// Removes an element at a given index. Recycles the index for later use.
        /// </summary>
        public void Remove(int id) {
            var denseIndex = m_sparse[id];

            // remove from dense and sparse
            m_dense[denseIndex] = default;
            m_sparse[id] = 0;

            // add to recycled
            ArrayExtensions.EnsureLength(ref m_recycledDenseIndices, m_recycledDenseIndexCount + 1);
            m_recycledDenseIndices[m_recycledDenseIndexCount++] = denseIndex;
            
            Count--;
        }

        /// <summary>
        /// Clears the sparse set. Recycles all used indices.
        /// </summary>
        public void Clear() {
            ArrayExtensions.EnsureLength(ref m_recycledDenseIndices, Count);

            for (var i = 0; i < Count; i++) {
                m_dense[i] = default;
                m_recycledDenseIndices[i] = i;
            }

            for (var i = 0; i < m_sparse.Length; i++)
                m_sparse[i] = 0;

            m_recycledDenseIndexCount = Count;
            Count = 0;
        }

        private void Add(int id, in T data) {
            // get dense index from either recycled indices or the last occupied dense index + 1 (denseCount)
            var denseIndex = m_recycledDenseIndexCount > 0 ? m_recycledDenseIndices[--m_recycledDenseIndexCount] : Count + 1;

            ArrayExtensions.EnsureLength(ref m_sparse, id + 1);
            m_sparse[id] = denseIndex;

            ArrayExtensions.EnsureLength(ref m_dense, denseIndex + 1);
            m_dense[denseIndex] = data;

            Count++;
        }
    }
}