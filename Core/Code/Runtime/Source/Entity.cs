using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UFlow.Core.Runtime.DataStructures;

namespace UFlow.Core.Runtime
{
#if IL2CPP_ENABLED
    using Ecs;
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct Entity : IEquatable<Entity> {
        [FieldOffset(0)] private readonly int m_id;
        [FieldOffset(4)] private readonly ushort m_gen;
        [FieldOffset(6)] private readonly ushort m_worldId;
        
        internal World World => World.worlds[m_worldId];
        internal ref ComponentBitset Components => ref World.entityInfos[m_id].componentBitset;

        internal Entity(int id, ushort gen, ushort worldId)
        {
            m_id = id;
            m_gen = gen;
            m_worldId = worldId;
        }

        public static implicit operator int(in Entity entity) => entity.m_id;

        public override string ToString() => $"Entity {m_id}:{m_gen}";
        
        public bool Equals(Entity other) => m_id == other.m_id && m_gen == other.m_gen;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Enable() {
            World.SetEntityEnabled(m_id, true);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Disable() {
            World.SetEntityEnabled(m_id, false);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetActive(bool active) {
            World.SetEntityEnabled(m_id, active);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set<T>() {
            Set((T)default);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set<T>(in T component) {
            Components[StashManager<T>.Bit] = true;
            var stash = StashManager<T>.GetOrCreate(m_worldId);
            var alreadyHas = stash.Has(m_id);
            stash.Set(m_id, component);
            if (alreadyHas)
                Publisher.Publish(m_worldId, new EntityComponentAddedEvent<T>(m_id, component));
            
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Get<T>() {
            return ref StashManager<T>.GetOrCreate(m_worldId).Get(m_id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has<T>() {
            var stash = StashManager<T>.Get(m_worldId);
            return stash != null && stash.Has(m_id);
        }

        public void Remove<T>() {
            Components[StashManager<T>.Bit] = false;
            var stash = StashManager<T>.Get(m_worldId);
            if (stash == null) return;
            var alreadyHas = stash.Has(m_id);
            if (!alreadyHas) return;
            var component = stash.Get(m_id);
            stash.Remove(m_id);
            Publisher.Publish(m_worldId, new EntityComponentRemovedEvent<T>(m_id, component));
        }

        public void Destroy() {
            var world = World.worlds[m_worldId];
            if (world == null)
                throw new Exception("Attempting to destroy Entity in invalid World.");
            world.RecycleEntity(m_id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsAlive() {
            var world = World.worlds[m_worldId];
            return world != null && world.IsEntityAlive(m_id, m_gen);
        }
    }
}