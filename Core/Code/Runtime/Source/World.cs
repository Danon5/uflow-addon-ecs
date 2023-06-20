using System;
using System.Runtime.CompilerServices;
using UFlow.Core.Runtime.DataStructures;
using UFlow.Core.Runtime.Extensions;

namespace UFlow.Core.Runtime {
#if IL2CPP_ENABLED
    using Ecs;
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    public sealed class World : IPublisher {
        internal static readonly ComponentBit IsAliveBit;
        internal static readonly ComponentBit IsEnabledBit;
        internal static World[] worlds;
        internal readonly ushort id;
        internal EntityInfo[] entityInfos;
        private static IdStack s_worldIdStack;
        private readonly IdStack m_entityIdStack;
        private bool m_isDestroyed;
        
        internal int NextEntityId => m_entityIdStack.NextId;

        static World() {
            IsAliveBit = ComponentBit.GetNextBit();
            IsEnabledBit = ComponentBit.GetNextBit();
            worlds = Array.Empty<World>();
            s_worldIdStack = new IdStack(1);
            ExternalEngineUtilities.ClearStaticCacheEvent += ClearStaticCache;
        }

        public World() {
            id = (ushort)s_worldIdStack.GetNextId();
            entityInfos = Array.Empty<EntityInfo>();
            m_entityIdStack = new IdStack(1);
            ArrayExtensions.EnsureLength(ref worlds, id + 1);
            worlds[id] = this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IDisposable Subscribe<T>(PublishedEventHandler<T> action) => Publisher.Subscribe(id, action);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Publish<T>(in T @event) => Publisher.Publish(id, @event);

        public void Destroy() {
            if (m_isDestroyed) return;
            m_isDestroyed = true;
            s_worldIdStack.RecycleId(id);
            Publish(new WorldDestroyedEvent(id));
            Publisher.Publish(new WorldDestroyedEvent(id));
            worlds[id] = null;
        }

        public Entity CreateEntity() {
            var entityId = m_entityIdStack.GetNextId();
            ArrayExtensions.EnsureLength(ref entityInfos, entityId + 1);
            ref var info = ref entityInfos[entityId];
            info.componentBitset[IsAliveBit] = true;
            info.componentBitset[IsEnabledBit] = true;
            Publish(new EntityCreatedEvent(entityId));
            Publish(new EntityEnabledEvent(entityId));
            return new Entity(entityId, info.gen, id);
        }

        public bool IsAlive() {
            return !m_isDestroyed;
        }

        public Query CreateQuery() {
            return new Query(this, true);
        }

        public IDisposable SubscribeWorldDestroyed(WorldDestroyedHandler handler) {
            return Subscribe((in WorldDestroyedEvent @event) => handler(this));
        }

        public IDisposable SubscribeEntityCreated(EntityCreatedHandler handler) {
            return Subscribe((in EntityCreatedEvent @event) => handler(GetEntity(@event.entityId)));
        }
        
        public IDisposable SubscribeEntityDestroyed(EntityDestroyedHandler handler) {
            return Subscribe((in EntityDestroyedEvent @event) => handler(GetEntity(@event.entityId)));
        }
        
        public IDisposable SubscribeEntityEnabled(EntityEnabledHandler handler) {
            return Subscribe((in EntityEnabledEvent @event) => handler(GetEntity(@event.entityId)));
        }
        
        public IDisposable SubscribeEntityDisabled(EntityDisabledHandler handler) {
            return Subscribe((in EntityDisabledEvent @event) => handler(GetEntity(@event.entityId)));
        }

        public IDisposable SubscribeEntityComponentAdded<T>(EntityComponentAddedHandler<T> handler) {
            return Subscribe((in EntityComponentAddedEvent<T> @event) => handler(GetEntity(@event.entityId), @event.component));
        }
        
        public IDisposable SubscribeEntityComponentRemoved<T>(EntityComponentRemovedHandler<T> handler) {
            return Subscribe((in EntityComponentRemovedEvent<T> @event) => handler(GetEntity(@event.entityId), @event.component));
        }

        internal void SetEntityEnabled(int entityId, bool enabled) {
            ref var info = ref entityInfos[entityId];
            if (!IsEntityAlive(entityId, info.gen))
                throw new Exception(enabled ? "Cannot enable destroyed entity." : "Cannot disable destroyed entity.");
            var wasEnabled = info.componentBitset[IsEnabledBit];
            if (!enabled == wasEnabled) return;
            if (enabled) {
                info.componentBitset[IsEnabledBit] = true;
                Publish(new EntityEnabledEvent(entityId));
            }
            else {
                info.componentBitset[IsEnabledBit] = false;
                Publish(new EntityDisabledEvent(entityId));
            }
        }

        internal void RecycleEntity(int entityId) {
            ref var info = ref entityInfos[entityId];
            if (!IsEntityAlive(entityId, info.gen))
                throw new Exception("Cannot recycle destroyed entity.");
            Publish(new EntityDisabledEvent(entityId));
            Publish(new EntityDestroyedEvent(entityId, id));
            info.gen++;
            info.componentBitset.Clear();
            m_entityIdStack.RecycleId(entityId);
        }

        internal bool IsEntityAlive(int entityId, ushort gen) {
            if (m_isDestroyed) return false;
            if (entityId >= entityInfos.Length) return false;
            ref var info = ref entityInfos[entityId];
            return info.gen == gen && info.componentBitset[IsAliveBit];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Entity GetEntity(int entityId) {
            ref var info = ref entityInfos[entityId];
            return new Entity(entityId, info.gen, id);
        }

        private static void ClearStaticCache() {
            worlds = Array.Empty<World>();
            s_worldIdStack = new IdStack(1);
        }
    }
}