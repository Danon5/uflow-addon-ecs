using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UFlow.Addon.Ecs.Core.Runtime {
    public sealed class DynamicEntityMap<TKey> : IDynamicEntityCollection, IInternalDynamicEntityCollection, IDisposable {
        private readonly DynamicEntityCollectionUpdater m_updater;
        private readonly Dictionary<TKey, Entity> m_entities;
        private readonly TryGetKeyDelegate m_tryGetKey;

        public int EntityCount => m_entities.Count;

        internal DynamicEntityMap(in World world,
                                  in Predicate<Bitset> filter,
                                  in List<Func<World, DynamicEntityCollectionUpdater, IDisposable>> actions,
                                  in List<Predicate<Entity>> initialAddPredicates,
                                  bool excludeInitialEntities,
                                  in TryGetKeyDelegate tryGetKey) {
            m_updater = new DynamicEntityCollectionUpdater(this, world, filter, actions, initialAddPredicates);
            m_entities = new Dictionary<TKey, Entity>();
            m_tryGetKey = tryGetKey;
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
            if (!m_tryGetKey(entity, out var key)) return;
            if (m_entities.ContainsKey(key)) return;
            m_entities.Add(key, entity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnsureRemoved(in Entity entity) {
            if (!m_tryGetKey(entity, out var key)) return;
            if (!m_entities.ContainsKey(key)) return;
            m_entities.Remove(key);
        }

        public void Dispose() => m_updater?.Dispose();

        public Entity GetEntity(in TKey key) => m_entities[key];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Dictionary<TKey, Entity>.Enumerator GetEnumerator() => m_entities.GetEnumerator();

        public delegate bool TryGetKeyDelegate(in Entity entity, out TKey key);
    }
}