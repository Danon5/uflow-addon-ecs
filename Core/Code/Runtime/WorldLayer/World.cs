using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UFlow.Core.Runtime;
using UFlow.Core.Shared;

namespace UFlow.Addon.ECS.Core.Runtime {
#if IL2CPP_ENABLED
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    public sealed class World {
        internal readonly short id;
        private readonly EcsIdStack m_entityIdStack;
        private readonly List<Type> m_componentTypes;
        private EntityInfo[] m_entityInfos;
        private Bitset m_bitset;

        public int EntityCount { get; private set; }
        public bool IsDeserializing { get; internal set; }
        internal int NextEntityId => m_entityIdStack.NextId;
        internal List<Type> ComponentTypes => GetWorldComponentTypes();
        internal int ComponentCount => ComponentTypes.Count;

        public World() {
            id = Worlds.GetNextId();
            m_entityIdStack = new EcsIdStack(1);
            m_componentTypes = new List<Type>();
            m_entityInfos = Array.Empty<EntityInfo>();
            m_bitset[Bits.IsAlive] = true;
            Worlds.AddWorld(this);
        }

        public static World CreateDefault() => EcsUtils.Worlds.CreateWorldFromType<DefaultWorld>();

        public void Destroy() {
            LogicHook<WorldDestroyedHook>.Execute(new WorldDestroyedHook(id));
            CleanupSystemGroups();
            Worlds.DestroyWorld(this);
            m_bitset.Clear();
        }

        public void Reset() {
            DestroyAllEntities();
            Publish(new WorldResetEvent());
            EntityCount = 0;
            m_entityIdStack.Reset();
            m_bitset.Clear();
            m_bitset[Bits.IsAlive] = true;
            m_componentTypes.Clear();
            Array.Resize(ref m_entityInfos, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsAlive() => m_bitset[Bits.IsAlive];

        public IDisposable Subscribe<T>(GenericHandler<T> action) => Publishers<T>.WorldInstance.Subscribe(action, id);

        public IDisposable SubscribeWorldDestroying(WorldDestroyingHandler action) => 
            Publishers<WorldDestroyingEvent>.WorldInstance.Subscribe(
                (in WorldDestroyingEvent _) => action(this), id);

        public IDisposable SubscribeWorldDestroyed(WorldDestroyedHandler action) => 
            Publishers<WorldDestroyedEvent>.WorldInstance.Subscribe(
                (in WorldDestroyedEvent _) => action(), id);

        public IDisposable SubscribeWorldComponentAdded<T>(WorldComponentAddedHandler<T> action) where T : IEcsComponentData =>
            Publishers<WorldComponentAddedEvent<T>>.WorldInstance.Subscribe(
                (in WorldComponentAddedEvent<T> _) => action(ref Get<T>()), id);

        public IDisposable SubscribeWorldComponentEnabled<T>(WorldComponentEnabledHandler<T> action) where T : IEcsComponentData =>
            Publishers<WorldComponentEnabledEvent<T>>.WorldInstance.Subscribe(
                (in WorldComponentEnabledEvent<T> _) => action(ref Get<T>()), id);

        public IDisposable SubscribeWorldComponentChanged<T>(WorldComponentChangedHandler<T> action) where T : IEcsComponentData =>
            Publishers<WorldComponentChangedEvent<T>>.WorldInstance.Subscribe(
                (in WorldComponentChangedEvent<T> _) => action(GetPrevious<T>(), ref Get<T>()), id);

        public IDisposable SubscribeWorldComponentDisabling<T>(WorldComponentDisablingHandler<T> action) where T : IEcsComponentData =>
            Publishers<WorldComponentDisablingEvent<T>>.WorldInstance.Subscribe(
                (in WorldComponentDisablingEvent<T> _) => action(ref Get<T>()), id);

        public IDisposable SubscribeWorldComponentDisabled<T>(WorldComponentDisabledHandler<T> action) where T : IEcsComponentData =>
            Publishers<WorldComponentDisabledEvent<T>>.WorldInstance.Subscribe(
                (in WorldComponentDisabledEvent<T> _) => action(ref Get<T>()), id);

        public IDisposable SubscribeWorldComponentRemoving<T>(WorldComponentRemovingHandler<T> action) where T : IEcsComponentData =>
            Publishers<WorldComponentRemovingEvent<T>>.WorldInstance.Subscribe(
                (in WorldComponentRemovingEvent<T> _) => action(ref Get<T>()), id);

        public IDisposable SubscribeWorldComponentRemoved<T>(WorldComponentRemovedHandler<T> action) where T : IEcsComponentData =>
            Publishers<WorldComponentRemovedEvent<T>>.WorldInstance.Subscribe(
                (in WorldComponentRemovedEvent<T> @event) => action(@event.component), id);

        public IDisposable SubscribeEntityCreated(EntityCreatedHandler action) =>
            Publishers<EntityCreatedEvent>.WorldInstance.Subscribe(
                (in EntityCreatedEvent @event) => action(@event.entity), id);

        public IDisposable SubscribeEntityEnabled(EntityEnabledHandler action) =>
            Publishers<EntityEnabledEvent>.WorldInstance.Subscribe(
                (in EntityEnabledEvent @event) => action(@event.entity), id);

        public IDisposable SubscribeEntityDisabling(EntityDisablingHandler action) =>
            Publishers<EntityDisablingEvent>.WorldInstance.Subscribe(
                (in EntityDisablingEvent @event) => action(@event.entity), id);

        public IDisposable SubscribeEntityDisabled(EntityDisabledHandler action) =>
            Publishers<EntityDisabledEvent>.WorldInstance.Subscribe(
                (in EntityDisabledEvent @event) => action(@event.entity), id);

        public IDisposable SubscribeEntityDestroying(EntityDestroyingHandler action) =>
            Publishers<EntityDestroyingEvent>.WorldInstance.Subscribe(
                (in EntityDestroyingEvent @event) => action(@event.entity), id);

        public IDisposable SubscribeEntityDestroyed(EntityDestroyedHandler action) =>
            Publishers<EntityDestroyedEvent>.WorldInstance.Subscribe(
                (in EntityDestroyedEvent @event) => action(@event.entity), id);

        public IDisposable SubscribeEntityComponentAdded<T>(EntityComponentAddedHandler<T> action) where T : IEcsComponentData =>
            Publishers<EntityComponentAddedEvent<T>>.WorldInstance.Subscribe(
                (in EntityComponentAddedEvent<T> @event) => action(@event.entity, ref @event.entity.Get<T>()), id);

        public IDisposable SubscribeEntityComponentEnabled<T>(EntityComponentEnabledHandler<T> action) where T : IEcsComponentData =>
            Publishers<EntityComponentEnabledEvent<T>>.WorldInstance.Subscribe(
                (in EntityComponentEnabledEvent<T> @event) => action(@event.entity, ref @event.entity.Get<T>()), id);

        public IDisposable SubscribeEntityComponentChanged<T>(EntityComponentChangedHandler<T> action) where T : IEcsComponentData =>
            Publishers<EntityComponentChangedEvent<T>>.WorldInstance.Subscribe(
                (in EntityComponentChangedEvent<T> @event) => 
                    action(@event.entity, @event.entity.GetPrevious<T>(), ref @event.entity.Get<T>()), id);

        public IDisposable SubscribeEntityComponentDisabling<T>(EntityComponentDisablingHandler<T> action) where T : IEcsComponentData =>
            Publishers<EntityComponentDisablingEvent<T>>.WorldInstance.Subscribe(
                (in EntityComponentDisablingEvent<T> @event) => action(@event.entity, ref @event.entity.Get<T>()), id);

        public IDisposable SubscribeEntityComponentDisabled<T>(EntityComponentDisabledHandler<T> action) where T : IEcsComponentData =>
            Publishers<EntityComponentDisabledEvent<T>>.WorldInstance.Subscribe(
                (in EntityComponentDisabledEvent<T> @event) => action(@event.entity, ref @event.entity.Get<T>()), id);

        public IDisposable SubscribeEntityComponentRemoving<T>(EntityComponentRemovingHandler<T> action) where T : IEcsComponentData =>
            Publishers<EntityComponentRemovingEvent<T>>.WorldInstance.Subscribe(
                (in EntityComponentRemovingEvent<T> @event) => action(@event.entity, ref @event.entity.Get<T>()), id);

        public IDisposable SubscribeEntityComponentRemoved<T>(EntityComponentRemovedHandler<T> action) where T : IEcsComponentData =>
            Publishers<EntityComponentRemovedEvent<T>>.WorldInstance.Subscribe(
                (in EntityComponentRemovedEvent<T> @event) => action(@event.entity, @event.component), id);

        internal IDisposable SubscribeEntityDisableComponents(EntityDisableComponentsHandler action) =>
            Publishers<EntityDisableComponentsEvent>.WorldInstance.Subscribe(
                (in EntityDisableComponentsEvent @event) => action(@event.entity), id);
        
        public IDisposable SubscribeReset(WorldResetHandler action) =>
            Publishers<WorldResetEvent>.WorldInstance.Subscribe(
                (in WorldResetEvent _) => action(), id);

        internal IDisposable WhenEntityRemoveComponents(EntityRemoveComponentsHandler action) =>
            Publishers<EntityRemoveComponentsEvent>.WorldInstance.Subscribe(
                (in EntityRemoveComponentsEvent @event) => action(@event.entity), id);
        
        internal IDisposable SubscribeEntityComponentParentEnabled<T>(EntityComponentParentEnabledHandler<T> action) 
            where T : IEcsComponentData =>
            Publishers<EntityComponentParentEnabledEvent<T>>.WorldInstance.Subscribe(
                (in EntityComponentParentEnabledEvent<T> @event) => action(@event.entity, ref @event.entity.Get<T>()), id);
        
        internal IDisposable SubscribeEntityComponentParentDisabled<T>(EntityComponentParentDisabledHandler<T> action) 
            where T : IEcsComponentData =>
            Publishers<EntityComponentParentDisabledEvent<T>>.WorldInstance.Subscribe(
                (in EntityComponentParentDisabledEvent<T> @event) => action(@event.entity, ref @event.entity.Get<T>()), id);

        internal IDisposable SubscribeAnyEntityComponentAdded(AnyEntityComponentAddedHandler action) =>
            Publishers<AnyEntityComponentAddedEvent>.WorldInstance.Subscribe(
                (in AnyEntityComponentAddedEvent @event) => action(@event.entity, @event.type), id);
        
        internal IDisposable SubscribeAnyEntityComponentRemoved(AnyEntityComponentAddedHandler action) =>
            Publishers<AnyEntityComponentAddedEvent>.WorldInstance.Subscribe(
                (in AnyEntityComponentAddedEvent @event) => action(@event.entity, @event.type), id);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Publish<T>(in T @event) => Publishers<T>.WorldInstance.Publish(@event, id);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Set<T>(in T component = default, bool enableIfAdded = true) where T : IEcsComponentData {
            var stash = Stashes<T>.GetOrCreate(id);
            var alreadyHas = stash.WorldHas();
            if (alreadyHas) {
                var previousStash = Stashes<T>.GetOrCreatePrevious(id);
                previousStash.WorldSet(Get<T>());
            }
            ref var compRef = ref stash.WorldSet(component);
            if (!alreadyHas) {
                m_bitset[Stashes<T>.Bit] = enableIfAdded;
                m_componentTypes.Add(typeof(T));
                Publishers<WorldComponentAddedEvent<T>>.WorldInstance.Publish(new WorldComponentAddedEvent<T>(), id);
                if (enableIfAdded)
                    Publishers<WorldComponentEnabledEvent<T>>.WorldInstance.Publish(new WorldComponentEnabledEvent<T>(), id);
                else
                    Publishers<WorldComponentDisabledEvent<T>>.WorldInstance.Publish(new WorldComponentDisabledEvent<T>(), id); 
                var previousStash = Stashes<T>.GetOrCreatePrevious(id);
                previousStash.WorldSet(Get<T>());
            }
            return ref compRef;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Add<T>(in T component = default) where T : IEcsComponentData {
            if (Stashes<T>.TryGet(id, out var stash) && stash.WorldHas())
                throw new Exception($"World already has component of type {typeof(T)}");
            return ref Set(component);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryAdd<T>(in T component = default) where T : IEcsComponentData {
            if (Stashes<T>.TryGet(id, out var stash) && stash.WorldHas())
                return false;
            Set(component);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void NotifyChanged<T>() where T : IEcsComponentData {
            Publishers<WorldComponentChangedEvent<T>>.WorldInstance.Publish(new WorldComponentChangedEvent<T>(), id);
            var previousStash = Stashes<T>.GetOrCreatePrevious(id);
            previousStash.WorldSet(Get<T>());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T SetWithNotify<T>(in T component) where T : IEcsComponentData {
            ref var compRef = ref Set(component);
            NotifyChanged<T>();
            return ref compRef;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Get<T>() where T : IEcsComponentData => ref Stashes<T>.Get(id).WorldGet();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGet<T>(out T component) where T : IEcsComponentData {
            if (!Stashes<T>.TryGet(id, out var stash) || !stash.WorldHas()) {
                component = default;
                return false;
            }
            
            component = stash.WorldGet();
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetPrevious<T>() where T : IEcsComponentData => ref Stashes<T>.GetPrevious(id).WorldGet();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetPrevious<T>(out T component) where T : IEcsComponentData {
            if (!Stashes<T>.TryGetPrevious(id, out var stash) || !stash.WorldHas()) {
                component = default;
                return false;
            }
            component = stash.WorldGet();
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has<T>() where T : IEcsComponentData => Stashes<T>.TryGet(id, out var stash) && stash.WorldHas();

        public void Remove<T>() where T : IEcsComponentData {
            if (!Stashes<T>.TryGet(id, out var stash) || !stash.WorldHas())
                throw new Exception($"World does not have component of type {typeof(T)}");
            var comp = stash.WorldGet();
            Disable<T>();
            Publishers<WorldComponentRemovingEvent<T>>.WorldInstance.Publish(new WorldComponentRemovingEvent<T>(), id);
            stash.WorldRemove();
            m_bitset[Stashes<T>.Bit] = false;
            m_componentTypes.Remove(typeof(T));
            Publishers<WorldComponentRemovedEvent<T>>.WorldInstance.Publish(new WorldComponentRemovedEvent<T>(comp), id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryRemove<T>() where T : IEcsComponentData {
            if (!Stashes<T>.TryGet(id, out var stash) || !stash.WorldHas())
                return false;
            Remove<T>();
            return true;
        }

        public void SetEnabled<T>(bool enabled) where T : IEcsComponentData {
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
        public void Enable<T>() where T : IEcsComponentData => SetEnabled<T>(true);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Disable<T>() where T : IEcsComponentData => SetEnabled<T>(false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEnabled<T>() where T : IEcsComponentData => m_bitset[Stashes<T>.Bit];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public QueryBuilder BuildQuery(QueryEnabledFlags enabledFlags = QueryEnabledFlags.Enabled) => new(this, enabledFlags);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetOrCreateSystemGroup<T>() where T : BaseSystemGroup => Systems.GetOrCreateGroup<T>(id);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BaseSystemGroup GetOrCreateSystemGroup(in Type type) => Systems.GetOrCreateGroup(id, type);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetupSystemGroup<T>() where T : BaseSystemGroup => Systems.SetupGroup<T>(id);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RunSystemGroup<T>(float delta) where T : BaseSystemGroup => Systems.RunGroup<T>(id, delta);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CleanupSystemGroup<T>() where T : BaseSystemGroup => Systems.CleanupGroup<T>(id);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetupSystemGroups() => Systems.SetupGroups(id);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CleanupSystemGroups() => Systems.CleanupGroups(id);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ResetSystemGroups() => Systems.ResetGroups(id);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetSystemGroupEnabled<T>(bool value) where T : BaseSystemGroup => Systems.SetGroupEnabled<T>(id, value);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnableSystemGroup<T>() where T : BaseSystemGroup => Systems.EnableGroup<T>(id);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DisableSystemGroup<T>() where T : BaseSystemGroup => Systems.DisableGroup<T>(id);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsSystemGroupEnabled<T>() where T : BaseSystemGroup => Systems.IsGroupEnabled<T>(id);

        public Entity CreateEntity(bool enable = true) {
            var entityId = m_entityIdStack.GetNextId();
            UFlowUtils.Collections.EnsureIndex(ref m_entityInfos, entityId);
            ref var info = ref m_entityInfos[entityId];
            return CreateEntityWithIdAndGen(entityId, info.gen, enable);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity GetEntity(int entityId) {
            ref var info = ref m_entityInfos[entityId];
            return new Entity(entityId, info.gen, id);
        }

        public IEnumerable<Entity> GetEntitiesEnumerable() {
            for (var i = 1; i < NextEntityId; i++) {
                var info = m_entityInfos[i];
                if (!info.bitset[Bits.IsAlive]) continue;
                yield return new Entity(i, info.gen, id);
            }
        }

        public void DestroyAllEntities() {
            foreach (var entity in GetEntitiesEnumerable())
                entity.Destroy();
        }

        internal Entity CreateEntityWithIdAndGen(int entityId, uint entityGen, bool enable = true) {
            UFlowUtils.Collections.EnsureIndex(ref m_entityInfos, entityId);
            ref var info = ref m_entityInfos[entityId];
            info.bitset[Bits.IsAlive] = true;
            info.bitset[Bits.IsEnabled] = enable;
            if (info.componentTypes == null)
                info.componentTypes = new List<Type>();
            else
                info.componentTypes.Clear();
            info.gen = entityGen;
            var entity = new Entity(entityId, entityGen, id);
            EntityCount++;
            Publish(new EntityCreatedEvent(entity));
            if (enable)
                Publish(new EntityEnabledEvent(entity));
            else
                Publish(new EntityDisabledEvent(entity));
            return entity;
        }
        
        internal void DestroyEntity(in Entity entity) {
            if (!entity.IsAlive())
                throw new Exception($"Attempting to destroy {entity}, but it is already destroyed.");
            if (entity.IsEnabled()) {
                Publish(new EntityDisableComponentsEvent(entity));
                Publish(new EntityDisabledEvent(entity));
            }
            LogicHook<EntityDestroyedHook>.Execute(new EntityDestroyedHook(id, entity));
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
            if (!IsAlive() || m_entityInfos.Length == 0 || entity.id == 0 || entity.id >= m_entityInfos.Length) return false;
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
            if (entity.TryGet(out SceneEntityRef sceneEntityRef))
                sceneEntityRef.value.GameObject.SetActive(enabled);
            if (enabled)
                Publish(new EntityEnabledEvent(entity));
            else
                Publish(new EntityDisabledEvent(entity));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool IsEntityEnabled(in Entity entity) => m_entityInfos[entity.id].bitset[Bits.IsEnabled];

        internal void SetEntityComponentEnabled<T>(in Entity entity, bool enabled) where T : IEcsComponentData {
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
        internal bool IsEntityComponentEnabled<T>(in Entity entity) where T : IEcsComponentData => 
            m_entityInfos[entity.id].bitset[Stashes<T>.Bit];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetComponentBit<T>(in Entity entity, bool value) where T : IEcsComponentData =>
            m_entityInfos[entity.id].bitset[Stashes<T>.Bit] = value;

        internal void AddEntityComponentType(in Entity entity, in Type type) => m_entityInfos[entity.id].componentTypes.Add(type);
        
        internal void RemoveEntityComponentType(in Entity entity, in Type type) => m_entityInfos[entity.id].componentTypes.Remove(type);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Bitset GetEntityComponentBitset(int entityId) => m_entityInfos[entityId].bitset;
        
        internal List<Type> GetWorldComponentTypes() => m_componentTypes;
        
        internal List<Type> GetEntityComponentTypes(in Entity entity) => m_entityInfos[entity.id].componentTypes;
    }
}