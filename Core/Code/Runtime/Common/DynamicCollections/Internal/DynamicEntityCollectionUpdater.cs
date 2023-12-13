using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UFlow.Core.Runtime;

namespace UFlow.Addon.ECS.Core.Runtime {
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
        private readonly QueryEnabledFlags m_enabledFlags;
        private readonly List<Predicate<Entity>> m_initialAddPredicates;

        public DynamicEntityCollectionUpdater(in IInternalDynamicEntityCollection collection,
                                              World world,
                                              in Predicate<Bitset> filter,
                                              in List<Func<World, DynamicEntityCollectionUpdater, IDisposable>> actions,
                                              in List<Predicate<Entity>> predicates) {
            m_collection = collection;
            m_world = world;
            m_filter = filter;
            m_subscriptions = actions.Select(action => action(world, this)).MergeIntoGroup();
            m_initialAddPredicates = predicates;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CheckedAdd(in Entity entity) {
            if (!m_filter(entity.Bitset)) return;
            m_collection.EnsureAdded(entity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CheckedRemove(in Entity entity) {
            if (m_filter(entity.Bitset)) return;
            m_collection.EnsureRemoved(entity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CheckedAddOrRemove(in Entity entity) {
            var filterResult = m_filter(entity.Bitset);
            if (filterResult)
                m_collection.EnsureAdded(entity);
            else
                m_collection.EnsureRemoved(entity);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnsureAdded(in Entity entity) => m_collection.EnsureAdded(entity);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnsureRemoved(in Entity entity) => m_collection.EnsureRemoved(entity);

        public void AddInitialEntities() {
            foreach (var entity in m_world.GetEntitiesEnumerable()) {
                var anyPredicateFailed = m_initialAddPredicates.Any(predicate => !predicate(entity));
                if (anyPredicateFailed) continue;
                CheckedAdd(entity);
            }
        }

        public void Dispose() {
            m_subscriptions?.Dispose();
        }
    }
}