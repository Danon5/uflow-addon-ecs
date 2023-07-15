using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UFlow.Addon.Ecs.Core.Runtime {
#if IL2CPP_ENABLED
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    public sealed class DynamicEntitySet : IDynamicEntityCollection, IInternalDynamicEntityCollection, IDisposable {
        private readonly DynamicEntityCollectionUpdater m_updater;
        private readonly SparseArray<Entity> m_entities;

        public int EntityCount => m_entities.Count;

        internal DynamicEntitySet(in World world, 
                                  in Predicate<Bitset> filter, 
                                  in List<Func<World, DynamicEntityCollectionUpdater, IDisposable>> actions,
                                  bool excludeInitialEntities) {
            m_updater = new DynamicEntityCollectionUpdater(this, world, filter, actions);
            m_entities = new SparseArray<Entity>();
            if (excludeInitialEntities) return;
            m_updater.AddInitialEntities();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(in Entity entity) {
            if (m_entities.Has(entity.id)) return;
            m_entities.Set(entity.id, entity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(in Entity entity) {
            if (!m_entities.Has(entity.id)) return;
            m_entities.Remove(entity.id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ResetCache() {
            if (m_entities.Count == 0) return;
            m_entities.Clear();
        }

        public void Dispose() {
            m_updater?.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SparseArray<Entity>.Enumerator GetEnumerator() => m_entities.GetEnumerator();
    }
}