using System;
using System.Collections.Generic;
using UFlow.Core.Runtime.DataStructures;

namespace UFlow.Core.Runtime {
    public sealed class Query {
        private readonly World m_world;
        private ComponentBitset m_withSet;
        private ComponentBitset m_withoutSet;

        internal Query(in World world, bool getEnabled) {
            m_world = world;
            m_withSet[World.IsAliveBit] = true;
            if (getEnabled)
                m_withSet[World.IsEnabledBit] = true;
            else
                m_withoutSet[World.IsEnabledBit] = true;
        }

        public Query With<T>() {
            m_withSet[StashManager<T>.Bit] = true;
            return this;
        }

        public Query Without<T>() {
            m_withoutSet[StashManager<T>.Bit] = true;
            return this;
        }

        public int GetEntityCount() {
            var count = 0;
            foreach (var entity in AsEnumerable()) {
                count++;
            }

            return count;
        }

        public IEnumerable<Entity> AsEnumerable() => AsEnumerable(m_world, GetFilter());

        private static IEnumerable<Entity> AsEnumerable(World world, Predicate<ComponentBitset> filter) {
            for (var i = 1; i < Math.Min(world.entityInfos.Length, world.NextEntityId); i++) {
                var info = world.entityInfos[i];
                if (filter.Invoke(info.componentBitset))
                    yield return new Entity(i, info.gen, world.id);
            }
        }

        private Predicate<ComponentBitset> GetFilter() => QueryManager.GetQuery(m_withSet, m_withoutSet);
    }
}