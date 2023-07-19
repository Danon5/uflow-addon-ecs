using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UFlow.Core.Runtime;

[assembly: InternalsVisibleTo("UFlow.Addon.Serialization.Core.Runtime")]
namespace UFlow.Addon.Ecs.Core.Runtime {
#if IL2CPP_ENABLED
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    public sealed class World {
        internal readonly short id;
        private readonly IdStack m_entityIdStack;
        private readonly List<Type> m_componentTypes;
        private EntityInfo[] m_entityInfos;
        private Bitset m_bitset;

        public int EntityCount { get; private set; }
        public bool IsDeserializing { get; private set; }
        internal int NextEntityId => m_entityIdStack.NextId;
        internal List<Type> ComponentTypes => GetWorldComponentTypes();
        internal int ComponentCount => ComponentTypes.Count;

        public World() {
            id = Worlds.GetNextId();
            m_entityIdStack = new IdStack(1);
            m_componentTypes = new List<Type>();
            m_entityInfos = Array.Empty<EntityInfo>();
            m_bitset[Bits.IsAlive] = true;
            Worlds.AddWorld(this);
        }

        public static World CreateDefault() => EcsUtils.Worlds.CreateWorldFromType<DefaultWorld>();

        public void Destroy() {
            Worlds.DestroyWorld(this);
            m_bitset.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsAlive() => m_bitset[Bits.IsAlive];

        public IDisposable When<T>(GenericHandler<T> action) => Publishers<T>.WorldInstance.Subscribe(action, id);

        public IDisposable WhenWorldDestroying(WorldDestroyingHandler action) => 
            Publishers<WorldDestroyingEvent>.WorldInstance.Subscribe(
                (in WorldDestroyingEvent _) => action(this), id);

        public IDisposable WhenWorldDestroyed(WorldDestroyedHandler action) => 
            Publishers<WorldDestroyedEvent>.WorldInstance.Subscribe(
                (in WorldDestroyedEvent _) => action(), id);

        public IDisposable WhenWorldComponentAdded<T>(WorldComponentAddedHandler<T> action) where T : IEcsComponent =>
            Publishers<WorldComponentAddedEvent<T>>.WorldInstance.Subscribe(
                (in WorldComponentAddedEvent<T> _) => action(ref Get<T>()), id);

        public IDisposable WhenWorldComponentEnabled<T>(WorldComponentEnabledHandler<T> action) where T : IEcsComponent =>
            Publishers<WorldComponentEnabledEvent<T>>.WorldInstance.Subscribe(
                (in WorldComponentEnabledEvent<T> _) => action(ref Get<T>()), id);

        public IDisposable WhenWorldComponentChanged<T>(WorldComponentChangedHandler<T> action) where T : IEcsComponent =>
            Publishers<WorldComponentChangedEvent<T>>.WorldInstance.Subscribe(
                (in WorldComponentChangedEvent<T> _) => action(GetPrevious<T>(), ref Get<T>()), id);

        public IDisposable WhenWorldComponentDisabling<T>(WorldComponentDisablingHandler<T> action) where T : IEcsComponent =>
            Publishers<WorldComponentDisablingEvent<T>>.WorldInstance.Subscribe(
                (in WorldComponentDisablingEvent<T> _) => action(ref Get<T>()), id);

        public IDisposable WhenWorldComponentDisabled<T>(WorldComponentDisabledHandler<T> action) where T : IEcsComponent =>
            Publishers<WorldComponentDisabledEvent<T>>.WorldInstance.Subscribe(
                (in WorldComponentDisabledEvent<T> _) => action(ref Get<T>()), id);

        public IDisposable WhenWorldComponentRemoving<T>(WorldComponentRemovingHandler<T> action) where T : IEcsComponent =>
            Publishers<WorldComponentRemovingEvent<T>>.WorldInstance.Subscribe(
                (in WorldComponentRemovingEvent<T> _) => action(ref Get<T>()), id);

        public IDisposable WhenWorldComponentRemoved<T>(WorldComponentRemovedHandler<T> action) where T : IEcsComponent =>
            Publishers<WorldComponentRemovedEvent<T>>.WorldInstance.Subscribe(
                (in WorldComponentRemovedEvent<T> @event) => action(@event.component), id);

        public IDisposable WhenEntityCreated(EntityCreatedHandler action) =>
            Publishers<EntityCreatedEvent>.WorldInstance.Subscribe(
                (in EntityCreatedEvent @event) => action(@event.entity), id);

        public IDisposable WhenEntityEnabled(EntityEnabledHandler action) =>
            Publishers<EntityEnabledEvent>.WorldInstance.Subscribe(
                (in EntityEnabledEvent @event) => action(@event.entity), id);

        public IDisposable WhenEntityDisabling(EntityDisablingHandler action) =>
            Publishers<EntityDisablingEvent>.WorldInstance.Subscribe(
                (in EntityDisablingEvent @event) => action(@event.entity), id);

        public IDisposable WhenEntityDisabled(EntityDisabledHandler action) =>
            Publishers<EntityDisabledEvent>.WorldInstance.Subscribe(
                (in EntityDisabledEvent @event) => action(@event.entity), id);

        public IDisposable WhenEntityDestroying(EntityDestroyingHandler action) =>
            Publishers<EntityDestroyingEvent>.WorldInstance.Subscribe(
                (in EntityDestroyingEvent @event) => action(@event.entity), id);

        public IDisposable WhenEntityDestroyed(EntityDestroyedHandler action) =>
            Publishers<EntityDestroyedEvent>.WorldInstance.Subscribe(
                (in EntityDestroyedEvent @event) => action(@event.entity), id);

        public IDisposable WhenEntityComponentAdded<T>(EntityComponentAddedHandler<T> action) where T : IEcsComponent =>
            Publishers<EntityComponentAddedEvent<T>>.WorldInstance.Subscribe(
                (in EntityComponentAddedEvent<T> @event) => action(@event.entity, ref @event.entity.Get<T>()), id);

        public IDisposable WhenEntityComponentEnabled<T>(EntityComponentEnabledHandler<T> action) where T : IEcsComponent =>
            Publishers<EntityComponentEnabledEvent<T>>.WorldInstance.Subscribe(
                (in EntityComponentEnabledEvent<T> @event) => action(@event.entity, ref @event.entity.Get<T>()), id);

        public IDisposable WhenEntityComponentChanged<T>(EntityComponentChangedHandler<T> action) where T : IEcsComponent =>
            Publishers<EntityComponentChangedEvent<T>>.WorldInstance.Subscribe(
                (in EntityComponentChangedEvent<T> @event) => 
                    action(@event.entity, @event.entity.GetPrevious<T>(), ref @event.entity.Get<T>()), id);

        public IDisposable WhenEntityComponentDisabling<T>(EntityComponentDisablingHandler<T> action) where T : IEcsComponent =>
            Publishers<EntityComponentDisablingEvent<T>>.WorldInstance.Subscribe(
                (in EntityComponentDisablingEvent<T> @event) => action(@event.entity, ref @event.entity.Get<T>()), id);

        public IDisposable WhenEntityComponentDisabled<T>(EntityComponentDisabledHandler<T> action) where T : IEcsComponent =>
            Publishers<EntityComponentDisabledEvent<T>>.WorldInstance.Subscribe(
                (in EntityComponentDisabledEvent<T> @event) => action(@event.entity, ref @event.entity.Get<T>()), id);

        public IDisposable WhenEntityComponentRemoving<T>(EntityComponentRemovingHandler<T> action) where T : IEcsComponent =>
            Publishers<EntityComponentRemovingEvent<T>>.WorldInstance.Subscribe(
                (in EntityComponentRemovingEvent<T> @event) => action(@event.entity, ref @event.entity.Get<T>()), id);

        public IDisposable WhenEntityComponentRemoved<T>(EntityComponentRemovedHandler<T> action) where T : IEcsComponent =>
            Publishers<EntityComponentRemovedEvent<T>>.WorldInstance.Subscribe(
                (in EntityComponentRemovedEvent<T> @event) => action(@event.entity, @event.component), id);

        internal IDisposable WhenEntityDisableComponents(EntityDisableComponentsHandler action) =>
            Publishers<EntityDisableComponentsEvent>.WorldInstance.Subscribe(
                (in EntityDisableComponentsEvent @event) => action(@event.entity), id);

        internal IDisposable WhenEntityRemoveComponents(EntityRemoveComponentsHandler action) =>
            Publishers<EntityRemoveComponentsEvent>.WorldInstance.Subscribe(
                (in EntityRemoveComponentsEvent @event) => action(@event.entity), id);
        
        internal IDisposable WhenEntityComponentParentEnabled<T>(EntityComponentParentEnabledHandler<T> action) where T : IEcsComponent =>
            Publishers<EntityComponentParentEnabledEvent<T>>.WorldInstance.Subscribe(
                (in EntityComponentParentEnabledEvent<T> @event) => action(@event.entity, ref @event.entity.Get<T>()), id);
        
        internal IDisposable WhenEntityComponentParentDisabled<T>(EntityComponentParentDisabledHandler<T> action) where T : IEcsComponent =>
            Publishers<EntityComponentParentDisabledEvent<T>>.WorldInstance.Subscribe(
                (in EntityComponentParentDisabledEvent<T> @event) => action(@event.entity, ref @event.entity.Get<T>()), id);

        internal IDisposable WhenReset(WorldResetHandler action) =>
            Publishers<WorldResetEvent>.WorldInstance.Subscribe(
                (in WorldResetEvent _) => action(), id);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Publish<T>(in T @event) => Publishers<T>.WorldInstance.Publish(@event, id);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Set<T>(in T component = default, bool enableIfAdded = true) where T : IEcsComponent {
            var stash = Stashes<T>.GetOrCreate(id);
            var alreadyHas = stash.WorldHas();
            if (alreadyHas) {
                var previousStash = Stashes<T>.GetOrCreatePrevious(id);
                previousStash.WorldSet(Get<T>());
            }
            ref var compRef = ref stash.WorldSet(component);
            if (!alreadyHas) {
                m_bitset[Stashes<T>.Bit] = enableIfAdded;
                Publishers<WorldComponentAddedEvent<T>>.WorldInstance.Publish(new WorldComponentAddedEvent<T>(), id);
                if (enableIfAdded)
                    Publishers<WorldComponentEnabledEvent<T>>.WorldInstance.Publish(new WorldComponentEnabledEvent<T>(), id);
                else
                    Publishers<WorldComponentDisabledEvent<T>>.WorldInstance.Publish(new WorldComponentDisabledEvent<T>(), id); 
                var previousStash = Stashes<T>.GetOrCreatePrevious(id);
                previousStash.WorldSet(Get<T>());
                m_componentTypes.Add(typeof(T));
            }
            return ref compRef;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Add<T>(in T component = default) where T : IEcsComponent {
            if (Stashes<T>.TryGet(id, out var stash) && stash.WorldHas())
                throw new Exception($"World already has component of type {typeof(T)}");
            return ref Set(component);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryAdd<T>(in T component = default) where T : IEcsComponent {
            if (Stashes<T>.TryGet(id, out var stash) && stash.WorldHas())
                return false;
            Set(component);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void NotifyChanged<T>() where T : IEcsComponent {
            Publishers<WorldComponentChangedEvent<T>>.WorldInstance.Publish(new WorldComponentChangedEvent<T>(), id);
            var previousStash = Stashes<T>.GetOrCreatePrevious(id);
            previousStash.WorldSet(Get<T>());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T SetWithNotify<T>(in T component) where T : IEcsComponent {
            ref var compRef = ref Set(component);
            NotifyChanged<T>();
            return ref compRef;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Get<T>() where T : IEcsComponent => ref Stashes<T>.Get(id).WorldGet();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGet<T>(out T component) where T : IEcsComponent {
            if (!Stashes<T>.TryGet(id, out var stash) || !stash.WorldHas()) {
                component = default;
                return false;
            }
            
            component = stash.WorldGet();
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetPrevious<T>() where T : IEcsComponent => ref Stashes<T>.GetPrevious(id).WorldGet();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetPrevious<T>(out T component) where T : IEcsComponent {
            if (!Stashes<T>.TryGetPrevious(id, out var stash) || !stash.WorldHas()) {
                component = default;
                return false;
            }
            component = stash.WorldGet();
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has<T>() where T : IEcsComponent => Stashes<T>.TryGet(id, out var stash) && stash.WorldHas();

        public void Remove<T>() where T : IEcsComponent {
            if (!Stashes<T>.TryGet(id, out var stash) || !stash.WorldHas())
                throw new Exception($"World does not have component of type {typeof(T)}");
            var comp = stash.WorldGet();
            Disable<T>();
            Publishers<WorldComponentRemovingEvent<T>>.WorldInstance.Publish(new WorldComponentRemovingEvent<T>(), id);
            stash.WorldRemove();
            m_bitset[Stashes<T>.Bit] = false;
            Publishers<WorldComponentRemovedEvent<T>>.WorldInstance.Publish(new WorldComponentRemovedEvent<T>(comp), id);
            m_componentTypes.Remove(typeof(T));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryRemove<T>() where T : IEcsComponent {
            if (!Stashes<T>.TryGet(id, out var stash) || !stash.WorldHas())
                return false;
            Remove<T>();
            return true;
        }

        public void SetEnabled<T>(bool enabled) where T : IEcsComponent {
            if (!Stashes<T>.TryGet(id, out var stash) || !stash.WorldHas())
                throw new Exception($"World does not have component of type {typeof(T)}");
            var wasEnabled = m_bitset[Stashes<T>.Bit];
            if (enabled == wasEnabled) return;
            if (!enabled)
                Publishers<WorldComponentDisablingEvent<T>>.WorldInstance.Publish(new WorldComponentDisablingEvent<T>(), id);
            m_bitset[Stashes<T>.Bit] = enabled;
            if (enabled)
                Publishers<WorldComponentEnabledEvent<T>>.WorldInstance.Publish(new WorldComponentEnabledEvent<T>(), id);
            else
                Publishers<WorldComponentDisabledEvent<T>>.WorldInstance.Publish(new WorldComponentDisabledEvent<T>(), id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Enable<T>() where T : IEcsComponent => SetEnabled<T>(true);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Disable<T>() where T : IEcsComponent => SetEnabled<T>(false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEnabled<T>() where T : IEcsComponent => m_bitset[Stashes<T>.Bit];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public QueryBuilder BuildQuery() => new QueryBuilder(this, QueryEnabledFlags.Enabled);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetOrCreateSystemGroup<T>() where T : BaseSystemGroup => Systems.GetOrCreateGroup<T>(id);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BaseSystemGroup GetOrCreateSystemGroup(in Type type) => Systems.GetOrCreateGroup(id, type);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetupSystemGroup<T>() => Systems.SetupGroup<T>(id);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RunSystemGroup<T>() => Systems.RunGroup<T>(id);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CleanupSystemGroup<T>() => Systems.CleanupGroup<T>(id);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetupSystemGroups() => Systems.SetupGroups(id);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CleanupSystemGroups() => Systems.CleanupGroups(id);

        public Entity CreateEntity(bool enable = true) {
            var entityId = m_entityIdStack.GetNextId();
            UFlowUtils.Collections.EnsureIndex(ref m_entityInfos, entityId);
            ref var info = ref m_entityInfos[entityId];
            info.bitset[Bits.IsAlive] = true;
            info.bitset[Bits.IsEnabled] = enable;
            info.componentTypes = new List<Type>();
            var entity = new Entity(entityId, info.gen, id);
            EntityCount++;
            Publish(new EntityCreatedEvent(entity));
            if (enable)
                Publish(new EntityEnabledEvent(entity));
            else
                Publish(new EntityDisabledEvent(entity));
            return entity;
        }

        internal void DestroyEntity(in Entity entity) {
            if (entity.IsEnabled()) {
                Publish(new EntityDisableComponentsEvent(entity));
                Publish(new EntityDisabledEvent(entity));
            }
            Publish(new EntityRemoveComponentsEvent(entity));
            m_entityIdStack.RecycleId(entity.id);
            ref var info = ref m_entityInfos[entity.id];
            info.gen++;
            info.bitset.Clear();
            EntityCount--;
            Publish(new EntityDestroyedEvent(entity));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool IsEntityAlive(in Entity entity) {
            if (!IsAlive() || entity.id >= m_entityInfos.Length) return false;
            ref var info = ref m_entityInfos[entity.id];
            return info.gen == entity.gen && info.bitset[Bits.IsAlive];
        }

        internal void SetEntityEnabled(in Entity entity, bool enabled) {
            ref var info = ref m_entityInfos[entity.id];
            var wasEnabled = info.bitset[Bits.IsEnabled];
            if (enabled == wasEnabled) return;
            if (!enabled)
                Publish(new EntityDisablingEvent(entity));
            info.bitset[Bits.IsEnabled] = enabled;
            if (enabled)
                Publish(new EntityEnabledEvent(entity));
            else
                Publish(new EntityDisabledEvent(entity));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool IsEntityEnabled(in Entity entity) => m_entityInfos[entity.id].bitset[Bits.IsEnabled];

        internal void SetEntityComponentEnabled<T>(in Entity entity, bool enabled) where T : IEcsComponent {
            if (!Stashes<T>.TryGet(id, out var stash) || !stash.Has(entity.id))
                throw new Exception($"Entity does not have component of type {typeof(T)}");
            ref var info = ref m_entityInfos[entity.id];
            var wasEnabled = info.bitset[Stashes<T>.Bit];
            if (enabled == wasEnabled) return;
            if (!enabled)
                Publishers<EntityComponentDisablingEvent<T>>.WorldInstance.Publish(new EntityComponentDisablingEvent<T>(entity), id);
            info.bitset[Stashes<T>.Bit] = enabled;
            if (enabled)
                Publishers<EntityComponentEnabledEvent<T>>.WorldInstance.Publish(new EntityComponentEnabledEvent<T>(entity), id);
            else
                Publishers<EntityComponentDisabledEvent<T>>.WorldInstance.Publish(new EntityComponentDisabledEvent<T>(entity), id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool IsEntityComponentEnabled<T>(in Entity entity) where T : IEcsComponent => 
            m_entityInfos[entity.id].bitset[Stashes<T>.Bit];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetComponentBit<T>(in Entity entity, bool value) where T : IEcsComponent =>
            m_entityInfos[entity.id].bitset[Stashes<T>.Bit] = value;

        internal void AddEntityComponentType(in Entity entity, in Type type) => m_entityInfos[entity.id].componentTypes.Add(type);
        
        internal void RemoveEntityComponentType(in Entity entity, in Type type) => m_entityInfos[entity.id].componentTypes.Remove(type);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Entity GetEntity(int entityId) {
            ref var info = ref m_entityInfos[entityId];
            return new Entity(entityId, info.gen, id);
        }

        internal IEnumerable<Entity> GetEntitiesEnumerable() {
            for (var i = 1; i < NextEntityId; i++) {
                var info = m_entityInfos[i];
                if (!info.bitset[Bits.IsAlive]) continue;
                yield return new Entity(i, info.gen, id);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Bitset GetEntityComponentBitset(int entityId) => m_entityInfos[entityId].bitset;
        
        internal List<Type> GetWorldComponentTypes() => m_componentTypes;
        
        internal List<Type> GetEntityComponentTypes(in Entity entity) => m_entityInfos[entity.id].componentTypes;

        internal void ResetForDeserialization() {
            IsDeserializing = true;
            Publish(new WorldResetEvent());
            m_entityIdStack.Reset();
            m_bitset.Clear();
            m_componentTypes.Clear();
            Array.Resize(ref m_entityInfos, 0);
            IsDeserializing = false;
        }
    }
}