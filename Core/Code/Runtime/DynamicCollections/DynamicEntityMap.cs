using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UFlow.Addon.ECS.Core.Runtime {
    public sealed class DynamicEntityMap<TKey> : IDynamicEntityCollection, IInternalDynamicEntityCollection, IDisposable 
        where TKey : IEcsComponent {
        private readonly DynamicEntityCollectionUpdater m_updater;
        private readonly Dictionary<TKey, Entity> m_entities;

        public int EntityCount => m_entities.Count;

        internal DynamicEntityMap(in World world,
                                  in Predicate<Bitset> filter,
                                  in List<Func<World, DynamicEntityCollectionUpdater, IDisposable>> actions,
                                  in List<Predicate<Entity>> initialAddPredicates,
                                  bool excludeInitialEntities) {
            m_updater = new DynamicEntityCollectionUpdater(this, world, filter, actions, initialAddPredicates);
            m_entities = new Dictionary<TKey, Entity>();
            if (excludeInitialEntities) return;
            m_updater.AddInitialEntities();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ResetCache() {
            if (m_entities.Count == 0) return;
            m_entities.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnsureAdded(in Entity entity) {
            if (!entity.TryGet(out TKey component)) return;
            if (m_entities.ContainsKey(component)) return;
            m_entities.Add(component, entity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnsureRemoved(in Entity entity) {
            if (!entity.TryGet(out TKey component)) return;
            if (!m_entities.ContainsKey(component)) return;
            m_entities.Remove(component);
        }

        public void Dispose() => m_updater?.Dispose();

        public Entity GetEntity(in TKey key) => m_entities[key];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Dictionary<TKey, Entity>.Enumerator GetEnumerator() => m_entities.GetEnumerator();
    }
}