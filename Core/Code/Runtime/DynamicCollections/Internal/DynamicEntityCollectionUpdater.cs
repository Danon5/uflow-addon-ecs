using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace UFlow.Addon.Ecs.Core.Runtime {
#if IL2CPP_ENABLED
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    internal sealed class DynamicEntityCollectionUpdater : IDisposable {
        private readonly IInternalDynamicEntityCollection m_collection;
        private readonly World m_world;
        private readonly Predicate<Bitset> m_filter;
        private readonly IDisposable m_subscriptions;

        public DynamicEntityCollectionUpdater(in IInternalDynamicEntityCollection collection, 
                                              World world, 
                                              in Predicate<Bitset> filter, 
                                              in List<Func<World, DynamicEntityCollectionUpdater, IDisposable>> actions) {
            m_collection = collection;
            m_world = world;
            m_filter = filter;
            m_subscriptions = actions.Select(action => action(world, this)).MergeIntoGroup();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CheckedAdd(in Entity entity) {
            if (!m_filter(entity.Bitset)) return;
            m_collection.Add(entity);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CheckedRemove(in Entity entity) {
            if (m_filter(entity.Bitset)) return;
            m_collection.Remove(entity);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CheckedAddOrRemove(in Entity entity) {
            var filterResult = m_filter(entity.Bitset);
            if (filterResult)
                m_collection.Add(entity);
            else
                m_collection.Remove(entity);
        }

        public void AddInitialEntities() {
            foreach (var entity in m_world.GetEntitiesEnumerable())
                CheckedAdd(entity);
        }

        public void Dispose() {
            m_subscriptions?.Dispose();
        }
    }
}