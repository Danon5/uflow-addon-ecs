using System;
using System.Collections;
using System.Collections.Generic;
using UFlow.Core.Runtime;

namespace UFlow.Addon.Ecs.Core.Runtime {
    public sealed class DynamicEntitySet : IEnumerable<Entity>, IDisposable {
        private readonly World m_world;
        private readonly Predicate<ComponentBitset> m_filter;
        private readonly IDisposable m_subscriptions;
        private readonly SparseSet<Entity> m_entities;

        internal DynamicEntitySet(in World world, Predicate<ComponentBitset> filter) {
            m_world = world;
            m_filter = filter;
            m_subscriptions = new List<IDisposable> {
                world.SubscribeEntityComponentAdded(CheckedAdd),
                world.SubscribeEntityComponentRemoved(CheckedRemove),
                world.SubscribeEntityDestroyed(Remove),
                world.SubscribeWorldDestroyed(Dispose)
            }.MergeIntoGroup();
            m_entities = new SparseSet<Entity>();

            foreach (var entity in m_world.GetEntitiesEnumerable())
                CheckedAdd(entity);
        }

        public IEnumerator<Entity> GetEnumerator() {
            foreach (var entity in m_entities)
                yield return entity;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public void Dispose() {
            m_entities.Dispose();
            m_subscriptions.Dispose();
        }

        internal void CheckedAdd(in Entity entity) {
            if (m_entities.Has(entity.id)) return;
            if (!m_filter(entity.ComponentBitset)) return;
            m_entities.Set(entity.id, entity);
        }
        
        internal void CheckedRemove(in Entity entity) {
            if (!m_entities.Has(entity.id)) return;
            if (m_filter(entity.ComponentBitset)) return;
            m_entities.Remove(entity.id);
        }
        
        internal void Remove(in Entity entity) {
            if (!m_entities.Has(entity.id)) return;
            m_entities.Remove(entity.id);
        }
    }
}