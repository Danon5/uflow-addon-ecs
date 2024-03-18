using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UFlow.Addon.Entities.Core.Runtime {
#if IL2CPP_ENABLED
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    public sealed class DynamicEntitySet : IDynamicEntityCollection, IInternalDynamicEntityCollection, IDisposable {
        public event Action<Entity> OnEntityAdded;
        public event Action<Entity> OnEntityRemoved;
        private readonly DynamicEntityCollectionUpdater m_updater;
        private readonly SparseArray<Entity> m_entities;

        public int EntityCount => m_entities.Count;

        internal DynamicEntitySet(in World world, 
                                  in Predicate<Bitset> filter, 
                                  in List<Func<World, DynamicEntityCollectionUpdater, IDisposable>> actions,
                                  in List<Predicate<Entity>> initialAddPredicates,
                                  bool excludeInitialEntities) {
            m_updater = new DynamicEntityCollectionUpdater(this, world, filter, actions, initialAddPredicates);
            m_entities = new SparseArray<Entity>();
            if (excludeInitialEntities) return;
            m_updater.AddInitialEntities();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void IInternalDynamicEntityCollection.EnsureAdded(in Entity entity) {
            if (m_entities.Has(entity.id)) return;
            m_entities.Set(entity.id, entity);
            OnEntityAdded?.Invoke(entity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void IInternalDynamicEntityCollection.EnsureRemoved(in Entity entity) {
            if (!m_entities.Has(entity.id)) return;
            OnEntityRemoved?.Invoke(entity);
            m_entities.Remove(entity.id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool WorldIsAlive() => m_updater.WorldIsAlive();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ResetCache() {
            if (m_entities.Count == 0) return;
            m_entities.Clear();
            foreach (var entity in m_entities)
                OnEntityRemoved?.Invoke(entity);
        }

        public void Dispose() => m_updater?.Dispose();

        public Entity First() => m_entities.Count > 0 ? m_entities.First() : default;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SparseArray<Entity>.Enumerator GetEnumerator() => m_entities.GetEnumerator();

        public bool Contains(in Entity entity) => m_entities.Has(entity.id);
    }
}