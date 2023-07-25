using System;
using System.Runtime.CompilerServices;

namespace UFlow.Addon.ECS.Core.Runtime {
#if IL2CPP_ENABLED
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    internal sealed class Stash<T> {
        private readonly SparseArray<T> m_components;

        public int Count => m_components.Count;

        public Stash(int initialCapacity = 1) {
            m_components = new SparseArray<T>(initialCapacity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Set(int entityId, in T component) {
            return ref m_components.Set(entityId, component);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T WorldSet(in T component) {
            return ref m_components.SetBufferValue(component);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Get(int entityId) {
            return ref m_components.Get(entityId);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T WorldGet() {
            return ref m_components.GetBufferValue();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has(int entityId) {
            return m_components.Has(entityId);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool WorldHas() {
            return m_components.HasBufferValue();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(int entityId) {
            if (m_components.Get(entityId) is IDisposable disposable)
                disposable.Dispose();
            m_components.Remove(entityId);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WorldRemove() {
            if (m_components.GetBufferValue() is IDisposable disposable)
                disposable.Dispose();
            m_components.RemoveBufferValue();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear() {
            m_components.Clear();
        }
    }
}