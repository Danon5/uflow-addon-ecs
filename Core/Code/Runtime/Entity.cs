using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UFlow.Addon.ECS.Core.Runtime {
#if IL2CPP_ENABLED
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct Entity : IEquatable<Entity> {
        [FieldOffset(0)] internal readonly int id;
        [FieldOffset(4)] internal readonly uint gen;
        [FieldOffset(8)] private readonly short worldId;

        public World World => Worlds.Has(worldId) ? Worlds.Get(worldId) : default;
        internal Bitset Bitset => Worlds.Get(worldId).GetEntityComponentBitset(id);
        internal List<Type> ComponentTypes => World.GetEntityComponentTypes(this);
        internal int ComponentCount => World.GetEntityComponentTypes(this).Count;

        internal Entity(int id, uint gen, in short worldId) {
            this.id = id;
            this.gen = gen;
            this.worldId = worldId;
        }

        public static implicit operator int(in Entity entity) => entity.id;

        public static bool operator ==(in Entity lhs, in Entity rhs) => lhs.Equals(rhs);

        public static bool operator !=(in Entity lhs, in Entity rhs) => !lhs.Equals(rhs);

        public override string ToString() => $"Entity[{id}:{gen}]";

        public override int GetHashCode() => HashCode.Combine(id, gen);

        public override bool Equals(object obj) => obj is Entity entity && Equals(entity);

        public bool Equals(Entity other) => id == other.id && gen == other.gen && worldId == other.worldId;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Destroy() => World.DestroyEntity(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsAlive() => World != null && World.IsEntityAlive(this);

        public ref T Set<T>(in T component = default, bool enableIfAdded = true) where T : IEcsComponent {
            var stash = Stashes<T>.GetOrCreate(worldId);
            var alreadyHas = stash.Has(id);
            if (alreadyHas) {
                var previousStash = Stashes<T>.GetOrCreatePrevious(worldId);
                previousStash.Set(id, Get<T>());
            }
            ref var compRef = ref stash.Set(id, component);
            if (!alreadyHas) {
                SetEnabled<T>(enableIfAdded);
                World.AddEntityComponentType(this, typeof(T));
                World.Publish(new EntityComponentAddedEvent<T>(this));
                var previousStash = Stashes<T>.GetOrCreatePrevious(worldId);
                previousStash.Set(id, Get<T>());
            }
            return ref compRef;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Add<T>(in T component = default, bool enableIfAdded = true) where T : IEcsComponent {
            if (Stashes<T>.TryGet(worldId, out var stash) && stash.Has(id))
                throw new Exception($"Entity already has component of type {typeof(T)}");
            return ref Set(component, enableIfAdded);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryAdd<T>(in T component = default, bool enableIfAdded = true) where T : IEcsComponent {
            if (Stashes<T>.TryGet(worldId, out var stash) && stash.Has(id))
                return false;
            Set(component, enableIfAdded);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnsureAddedAndEnabled<T>() where T : IEcsComponent => EnsureAddedAndSetEnabled<T>(true);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnsureAddedAndDisabled<T>() where T : IEcsComponent => EnsureAddedAndSetEnabled<T>(false);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnsureAddedAndSetEnabled<T>(bool state) where T : IEcsComponent {
            if (!Has<T>())
                Add<T>(default, state);
            else if (IsEnabled<T>() != state)
                SetEnabled<T>(state);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void NotifyChanged<T>() where T : IEcsComponent {
            Publishers<EntityComponentChangedEvent<T>>.WorldInstance.Publish(new EntityComponentChangedEvent<T>(this), worldId);
            var previousStash = Stashes<T>.GetOrCreatePrevious(worldId);
            previousStash.Set(id, Get<T>());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T SetWithNotify<T>(in T component, bool enableIfAdded = true) where T : IEcsComponent {
            ref var compRef = ref Set(component, enableIfAdded);
            NotifyChanged<T>();
            return ref compRef;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Get<T>() where T : IEcsComponent => ref Stashes<T>.Get(worldId).Get(id);
        
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGet<T>(out T component) where T : IEcsComponent {
            if (!Stashes<T>.TryGet(worldId, out var stash) || !stash.Has(id)) {
                component = default;
                return false;
            }
            component = stash.Get(id);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetPrevious<T>() where T : IEcsComponent => ref Stashes<T>.GetPrevious(worldId).Get(id);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetPrevious<T>(out T component) where T : IEcsComponent {
            if (!Stashes<T>.TryGetPrevious(worldId, out var stash) || !stash.Has(id)) {
                component = default;
                return false;
            }
            component = stash.Get(id);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has<T>() where T : IEcsComponent => IsAlive() && Stashes<T>.TryGet(worldId, out var stash) && stash.Has(id);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasAndEnabled<T>() where T : IEcsComponent => IsAlive() && Has<T>() && IsEnabled<T>();

        public void Remove<T>() where T : IEcsComponent {
            if (!Stashes<T>.TryGet(worldId, out var stash) || !stash.Has(id))
                throw new Exception($"Entity does not have component of type {typeof(T)}");
            var comp = stash.Get(id);
            if (IsEnabled())
                Disable<T>();
            World.Publish(new EntityComponentRemovingEvent<T>(this));
            stash.Remove(id);
            World.SetComponentBit<T>(this, false);
            World.RemoveEntityComponentType(this, typeof(T));
            World.Publish(new EntityComponentRemovedEvent<T>(this, comp));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryRemove<T>() where T : IEcsComponent {
            if (!Stashes<T>.TryGet(worldId, out var stash) || !stash.Has(id))
                return false;
            Remove<T>();
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetEnabled(bool value) => World.SetEntityEnabled(this, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Enable() => World.SetEntityEnabled(this, true);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Disable() => World.SetEntityEnabled(this, false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEnabled() => World.IsEntityEnabled(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetEnabled<T>(bool value) where T : IEcsComponent => World.SetEntityComponentEnabled<T>(this, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Enable<T>() where T : IEcsComponent => World.SetEntityComponentEnabled<T>(this, true);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Disable<T>() where T : IEcsComponent => World.SetEntityComponentEnabled<T>(this, false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEnabled<T>() where T : IEcsComponent => World.IsEntityComponentEnabled<T>(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetRaw(Type type, IEcsComponent value, bool enableIfAdded = true) =>
            RawComponentMethodCache.InvokeSet(this, type, value, enableIfAdded);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetRaw(IEcsComponent value, bool enableIfAdded = true) =>
            RawComponentMethodCache.InvokeSet(this, value.GetType(), value, enableIfAdded);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEcsComponent GetRaw(Type type) => 
            RawComponentMethodCache.InvokeGet(this, type);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasRaw(Type type) => 
            RawComponentMethodCache.InvokeHas(this, type);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveRaw(Type type) => 
            RawComponentMethodCache.InvokeRemove(this, type);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryRemoveRaw(Type type) => 
            RawComponentMethodCache.InvokeTryRemove(this, type);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetEnabledRaw(Type type, bool value) => 
            RawComponentMethodCache.InvokeSetEnabled(this, type, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnableRaw(Type type) => 
            RawComponentMethodCache.InvokeSetEnabled(this, type, true);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DisableRaw(Type type) => 
            RawComponentMethodCache.InvokeSetEnabled(this, type, false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEnabledRaw(Type type) => 
            RawComponentMethodCache.InvokeIsEnabled(this, type);
        
        internal ref T SetWithoutEvents<T>(in T component = default, bool enableIfAdded = true) where T : IEcsComponent {
            var stash = Stashes<T>.GetOrCreate(worldId);
            var alreadyHas = stash.Has(id);
            if (alreadyHas) {
                var previousStash = Stashes<T>.GetOrCreatePrevious(worldId);
                previousStash.Set(id, Get<T>());
            }
            ref var compRef = ref stash.Set(id, component);
            if (!alreadyHas) {
                SetEnabledWithoutEvents<T>(enableIfAdded);
                World.AddEntityComponentType(this, typeof(T));
                var previousStash = Stashes<T>.GetOrCreatePrevious(worldId);
                previousStash.Set(id, Get<T>());
            }
            return ref compRef;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref T AddWithoutEvents<T>(in T component = default, bool enableIfAdded = true) where T : IEcsComponent {
            if (Stashes<T>.TryGet(worldId, out var stash) && stash.Has(id))
                throw new Exception($"Entity already has component of type {typeof(T)}");
            return ref SetWithoutEvents(component, enableIfAdded);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetEnabledWithoutEvents<T>(bool value) where T : IEcsComponent => 
            World.SetEntityComponentEnabledWithoutEvents<T>(this, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void InvokeAddedEvents<T>() where T : IEcsComponent =>
            World.Publish(new EntityComponentAddedEvent<T>(this));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void InvokeEnabledEvents<T>() where T : IEcsComponent =>
            World.InvokeEntityComponentEnabledEvents<T>(this, World.IsEntityComponentEnabled<T>(this));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetWithoutEventsRaw(IEcsComponent value, bool enableIfAdded = true) =>
            RawComponentMethodCache.InvokeSetWithoutEvents(this, value.GetType(), value, enableIfAdded);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void InvokeAddedEventsRaw(Type type) =>
            RawComponentMethodCache.InvokeAddedEvents(this, type);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void InvokeEnabledEventsRaw(Type type, bool enabled) =>
            RawComponentMethodCache.InvokeEnabledEvents(this, type, enabled);

    }
}