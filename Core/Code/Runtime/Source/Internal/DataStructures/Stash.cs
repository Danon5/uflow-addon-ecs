namespace UFlow.Core.Runtime.DataStructures {
#if IL2CPP_ENABLED
    using Ecs;
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    internal sealed class Stash<T> {
        private readonly SparseSet<T> m_components;

        public int Count => m_components.Count;
        
        public Stash(int initialCapacity = 1) {
            m_components = new SparseSet<T>(initialCapacity);
        }

        public void Set(int entityId, in T component) {
            m_components.Set(entityId, component);
        }

        public ref T Get(int entityId) {
            return ref m_components.Get(entityId);
        }
        
        public bool Has(int entityId) {
            return m_components.Has(entityId);
        }

        public void Remove(int entityId) {
            m_components.Remove(entityId);
        }

        internal void Clear() {
            m_components.Clear();
        }
    }
}