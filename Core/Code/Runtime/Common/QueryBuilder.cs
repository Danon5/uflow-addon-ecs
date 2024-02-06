using System;
using System.Collections.Generic;

namespace UFlow.Addon.Entities.Core.Runtime {
#if IL2CPP_ENABLED
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    public sealed class QueryBuilder {
        private readonly World m_world;
        private readonly QueryEnabledFlags m_enabledFlags;
        private readonly List<Bitset> m_withEitherSets;
        private readonly List<Bitset> m_withoutEitherSets;
        private readonly List<Func<World, DynamicEntityCollectionUpdater, IDisposable>> m_actions;
        private readonly List<Func<World, DynamicEntityCollectionUpdater, IDisposable>> m_whenActions;
        private readonly List<Predicate<Entity>> m_initialAddPredicates;
        private Bitset m_withSet;
        private Bitset m_withoutSet;
        private Bitset m_whenAddedSet;
        private Bitset m_whenEnabledSet;
        private Bitset m_whenChangedSet;
        private Bitset m_whenDisabledSet;
        private Bitset m_whenRemovedSet;
        private bool m_excludeInitialEntities;
        private bool m_hasWhenFilter;

        internal QueryBuilder(in World world, QueryEnabledFlags flags) {
            m_world = world;
            m_enabledFlags = flags;
            m_withEitherSets = new List<Bitset>();
            m_withoutEitherSets = new List<Bitset>();
            m_actions = new List<Func<World, DynamicEntityCollectionUpdater, IDisposable>>();
            m_whenActions = new List<Func<World, DynamicEntityCollectionUpdater, IDisposable>>();
            m_initialAddPredicates = new List<Predicate<Entity>>();
        }

        public QueryBuilder With<T>() where T : IEcsComponentData {
            if (m_withSet[Stashes<T>.Bit]) return this;
            m_withSet[Stashes<T>.Bit] = true;
            var enabled = m_enabledFlags.HasFlag(QueryEnabledFlags.Enabled);
            var disabled = m_enabledFlags.HasFlag(QueryEnabledFlags.Disabled);
            if (enabled == disabled) {
                if (!enabled) return this;
                m_initialAddPredicates.Add(entity => entity.Has<T>());
                m_actions.Add((world, updater) => world.SubscribeEntityComponentAdded((in Entity entity, ref T _) => 
                    updater.CheckedAdd(entity)));
                m_actions.Add((world, updater) => world.SubscribeEntityComponentRemoved((in Entity entity, in T _) => 
                    updater.CheckedRemove(entity)));
                return this;
            }
            m_initialAddPredicates.Add(entity => entity.IsEnabled() == enabled && entity.Has<T>() && entity.IsEnabled<T>());
            m_actions.Add((world, updater) => world.SubscribeEntityComponentRemoved((in Entity entity, in T _) => {
                updater.CheckedRemove(entity);
            }));
            AddEnabledActions<T>(false);
            return this;
        }

        public QueryBuilder Without<T>() where T : IEcsComponentData {
            if (m_withoutSet[Stashes<T>.Bit]) return this;
            m_withoutSet[Stashes<T>.Bit] = true;
            var enabled = m_enabledFlags.HasFlag(QueryEnabledFlags.Enabled);
            var disabled = m_enabledFlags.HasFlag(QueryEnabledFlags.Disabled);
            if (enabled == disabled) {
                if (!enabled) return this;
                m_initialAddPredicates.Add(entity => !entity.Has<T>());
                m_actions.Add((world, updater) => world.SubscribeEntityComponentAdded((in Entity entity, ref T _) => 
                    updater.CheckedRemove(entity)));
                m_actions.Add((world, updater) => world.SubscribeEntityComponentRemoved((in Entity entity, in T _) => 
                    updater.CheckedAdd(entity)));
                return this;
            }
            m_initialAddPredicates.Add(entity => entity.IsEnabled() == enabled && (!entity.Has<T>() || !entity.IsEnabled<T>()));
            m_actions.Add((world, updater) => world.SubscribeEntityComponentRemoved((in Entity entity, in T _) => 
                updater.CheckedAdd(entity)));
            AddEnabledActions<T>(true);
            return this;
        }

        public QueryBuilder WhenAdded<T>() where T : IEcsComponentData {
            if (m_whenAddedSet[Stashes<T>.Bit]) return this;
            m_whenAddedSet[Stashes<T>.Bit] = true;
            m_whenActions.Add((world, updater) => world.SubscribeEntityComponentAdded((in Entity entity, ref T _) => 
                updater.CheckedAdd(entity)));
            m_hasWhenFilter = true;
            return this;
        }

        public QueryBuilder WhenEnabled<T>() where T : IEcsComponentData {
            if (m_whenEnabledSet[Stashes<T>.Bit]) return this;
            m_whenEnabledSet[Stashes<T>.Bit] = true;
            m_whenActions.Add((world, updater) => world.SubscribeEntityComponentEnabled((in Entity entity, ref T _) => 
                updater.CheckedAdd(entity)));
            m_hasWhenFilter = true;
            return this;
        }

        public QueryBuilder WhenChanged<T>() where T : IEcsComponentData {
            if (m_whenChangedSet[Stashes<T>.Bit]) return this;
            m_whenChangedSet[Stashes<T>.Bit] = true;
            m_whenActions.Add((world, updater) => world.SubscribeEntityComponentChanged((in Entity entity, in T _, ref T _) => 
                updater.CheckedAdd(entity)));
            m_hasWhenFilter = true;
            return this;
        }

        public QueryBuilder WhenDisabled<T>() where T : IEcsComponentData {
            if (m_whenDisabledSet[Stashes<T>.Bit]) return this;
            m_whenDisabledSet[Stashes<T>.Bit] = true;
            m_whenActions.Add((world, updater) => world.SubscribeEntityComponentDisabled((in Entity entity, ref T _) => 
                updater.CheckedAdd(entity)));
            m_hasWhenFilter = true;
            return this;
        }

        public QueryBuilder WhenRemoved<T>() where T : IEcsComponentData {
            if (m_whenRemovedSet[Stashes<T>.Bit]) return this;
            m_whenRemovedSet[Stashes<T>.Bit] = true;
            m_whenActions.Add((world, updater) => world.SubscribeEntityComponentRemoved((in Entity entity, in T _) => 
                updater.CheckedAdd(entity)));
            m_hasWhenFilter = true;
            return this;
        }

        public EitherBuilder WithEither<T>() where T : IEcsComponentData {
            return new EitherBuilder(this, EitherBuilder.EitherType.With).Or<T>();
        }

        public EitherBuilder WithoutEither<T>() where T : IEcsComponentData {
            return new EitherBuilder(this, EitherBuilder.EitherType.Without).Or<T>();
        }

        public EitherBuilder WhenEitherAdded<T>() where T : IEcsComponentData {
            m_hasWhenFilter = true;
            return new EitherBuilder(this, EitherBuilder.EitherType.WhenAdded).Or<T>();
        }

        public EitherBuilder WhenEitherEnabled<T>() where T : IEcsComponentData {
            m_hasWhenFilter = true;
            return new EitherBuilder(this, EitherBuilder.EitherType.WhenEnabled).Or<T>();
        }

        public EitherBuilder WhenEitherChanged<T>() where T : IEcsComponentData {
            m_hasWhenFilter = true;
            return new EitherBuilder(this, EitherBuilder.EitherType.WhenChanged).Or<T>();
        }

        public EitherBuilder WhenEitherDisabled<T>() where T : IEcsComponentData {
            m_hasWhenFilter = true;
            return new EitherBuilder(this, EitherBuilder.EitherType.WhenDisabled).Or<T>();
        }

        public EitherBuilder WhenEitherRemoved<T>() where T : IEcsComponentData {
            m_hasWhenFilter = true;
            return new EitherBuilder(this, EitherBuilder.EitherType.WhenRemoved).Or<T>();
        }

        public QueryBuilder WithoutInitialEntities() {
            m_excludeInitialEntities = true;
            return this;
        }

        public DynamicEntitySet AsSet() => 
            new(m_world, GetFilter(), GetSubscriptions(), m_initialAddPredicates, m_excludeInitialEntities);
        public DynamicEntityMap<TKey> AsMap<TKey>() where TKey : IEcsComponentData => 
            new(m_world, GetFilter(), GetSubscriptions(), m_initialAddPredicates, m_excludeInitialEntities);

        private Predicate<Bitset> GetFilter() => Queries.GetFilter(m_withSet, m_withoutSet, m_withEitherSets, m_withoutEitherSets);

        private List<Func<World, DynamicEntityCollectionUpdater, IDisposable>> GetSubscriptions() =>
            m_hasWhenFilter ? m_whenActions : m_actions;

        private void AddEnabledActions<T>(bool inverse) where T : IEcsComponentData {
            switch (m_enabledFlags) {
                case QueryEnabledFlags.Enabled:
                    if (inverse) { // without
                        m_actions.Add((world, updater) => world.SubscribeEntityComponentEnabled((in Entity entity, ref T _) =>
                            updater.CheckedRemove(entity)));
                        m_actions.Add((world, updater) => world.SubscribeEntityComponentDisabled((in Entity entity, ref T _) => {
                            if (entity.IsEnabled())
                                updater.CheckedAdd(entity);
                        }));
                        m_actions.Add((world, updater) => world.SubscribeEntityComponentParentEnabled((in Entity entity, ref T _) => {
                            updater.CheckedRemove(entity);
                        }));
                        m_actions.Add((world, updater) => world.SubscribeEntityComponentParentDisabled((in Entity entity, ref T _) => {
                            if (entity.IsEnabled<T>())
                                updater.CheckedAdd(entity);
                        }));
                    }
                    else { // with
                        m_actions.Add((world, updater) => world.SubscribeEntityComponentEnabled((in Entity entity, ref T _) => {
                            if (entity.IsEnabled())
                                updater.CheckedAdd(entity);
                        }));
                        m_actions.Add((world, updater) => world.SubscribeEntityComponentDisabled((in Entity entity, ref T _) => 
                            updater.CheckedRemove(entity)));
                        m_actions.Add((world, updater) => world.SubscribeEntityComponentParentEnabled((in Entity entity, ref T _) => {
                            if (entity.IsEnabled<T>())
                                updater.CheckedAdd(entity);
                        }));
                        m_actions.Add((world, updater) => world.SubscribeEntityComponentParentDisabled((in Entity entity, ref T _) => 
                            updater.EnsureRemoved(entity)));
                    }
                    break;
                case QueryEnabledFlags.Disabled:
                    if (inverse) { // without
                        m_actions.Add((world, updater) => world.SubscribeEntityComponentEnabled((in Entity entity, ref T _) => 
                            updater.CheckedAdd(entity)));
                        m_actions.Add((world, updater) => world.SubscribeEntityComponentDisabled((in Entity entity, ref T _) => 
                            updater.CheckedRemove(entity)));
                        m_actions.Add((world, updater) => world.SubscribeEntityComponentParentEnabled((in Entity entity, ref T _) => 
                            updater.CheckedAdd(entity)));
                        m_actions.Add((world, updater) => world.SubscribeEntityComponentParentDisabled((in Entity entity, ref T _) => 
                            updater.EnsureRemoved(entity)));
                    }
                    else { // with
                        m_actions.Add((world, updater) => world.SubscribeEntityComponentEnabled((in Entity entity, ref T _) => 
                            updater.CheckedRemove(entity)));
                        m_actions.Add((world, updater) => world.SubscribeEntityComponentDisabled((in Entity entity, ref T _) => 
                            updater.CheckedAdd(entity)));
                        m_actions.Add((world, updater) => world.SubscribeEntityComponentParentEnabled((in Entity entity, ref T _) => 
                            updater.EnsureRemoved(entity)));
                        m_actions.Add((world, updater) => world.SubscribeEntityComponentParentDisabled((in Entity entity, ref T _) => 
                            updater.CheckedAdd(entity)));
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public sealed class EitherBuilder {
            private readonly QueryBuilder m_builder;
            private readonly EitherType m_type;
            private Bitset m_filter;

            internal EitherBuilder(in QueryBuilder builder, EitherType type) {
                m_builder = builder;
                m_type = type;
                m_filter = default;
            }

            public EitherBuilder Or<T>() where T : IEcsComponentData {
                if (m_filter[Stashes<T>.Bit]) return this;
                m_filter[Stashes<T>.Bit] = true;
                switch (m_type) {
                    case EitherType.With:
                        m_builder.m_actions.Add((world, updater) => world.SubscribeEntityComponentAdded((in Entity entity, ref T _) => 
                            updater.CheckedAdd(entity)));
                        m_builder.m_actions.Add((world, updater) => world.SubscribeEntityComponentRemoved((in Entity entity, in T _) => 
                            updater.CheckedRemove(entity)));
                        break;
                    case EitherType.Without:
                        m_builder.m_actions.Add((world, updater) => world.SubscribeEntityComponentAdded((in Entity entity, ref T _) => 
                            updater.CheckedRemove(entity)));
                        m_builder.m_actions.Add((world, updater) => world.SubscribeEntityComponentRemoved((in Entity entity, in T _) => 
                            updater.CheckedAdd(entity)));
                        break;
                    case EitherType.WhenAdded:
                        m_builder.m_whenActions.Add((world, updater) => world.SubscribeEntityComponentAdded((in Entity entity, ref T _) => 
                            updater.CheckedAdd(entity)));
                        break;
                    case EitherType.WhenEnabled:
                        m_builder.m_whenActions.Add((world, updater) => world.SubscribeEntityComponentEnabled((in Entity entity, ref T _) => 
                            updater.CheckedAdd(entity)));
                        break;
                    case EitherType.WhenChanged:
                        m_builder.m_whenActions.Add((world, updater) => world.SubscribeEntityComponentChanged((in Entity entity, in T _, ref T _) => 
                            updater.CheckedAdd(entity)));
                        break;
                    case EitherType.WhenDisabled:
                        m_builder.m_whenActions.Add((world, updater) => world.SubscribeEntityComponentDisabled((in Entity entity, ref T _) => 
                            updater.CheckedAdd(entity)));
                        break;
                    case EitherType.WhenRemoved:
                        m_builder.m_whenActions.Add((world, updater) => world.SubscribeEntityComponentRemoved((in Entity entity, in T _) => 
                            updater.CheckedAdd(entity)));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                var enabled = m_builder.m_enabledFlags.HasFlag(QueryEnabledFlags.Enabled);
                var disabled = m_builder.m_enabledFlags.HasFlag(QueryEnabledFlags.Disabled);
                if (enabled == disabled) return this;
                m_builder.AddEnabledActions<T>(m_type is EitherType.Without);
                return this;
            }

            public QueryBuilder EndEither() {
                switch (m_type) {
                    case EitherType.With:
                    case EitherType.WhenAdded:
                    case EitherType.WhenEnabled:
                    case EitherType.WhenChanged:
                        m_builder.m_withEitherSets.Add(m_filter);
                        break;
                    case EitherType.Without:
                    case EitherType.WhenDisabled:
                    case EitherType.WhenRemoved:
                        m_builder.m_withoutEitherSets.Add(m_filter);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return m_builder;
            }

            public DynamicEntitySet AsSet() => EndEither().AsSet();
            public DynamicEntityMap<TKey> AsMap<TKey>() where TKey : IEcsComponentData => EndEither().AsMap<TKey>();

            internal enum EitherType : byte {
                With,
                Without,
                WhenAdded,
                WhenEnabled,
                WhenChanged,
                WhenDisabled,
                WhenRemoved
            }
        }
    }
}