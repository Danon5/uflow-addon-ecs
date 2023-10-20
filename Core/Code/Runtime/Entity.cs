using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#if UNITY_EDITOR
using UnityEngine;
#endif

namespace UFlow.Addon.ECS.Core.Runtime {
#if IL2CPP_ENABLED
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct Entity : IEquatable<Entity> {
        [FieldOffset(0)] internal readonly int id;
        [FieldOffset(4)] internal readonly ushort gen;
        [FieldOffset(6)] private readonly short worldId;
        private static readonly Dictionary<Type, MethodInfo> s_setRawCache = new();
        private static readonly Dictionary<Type, MethodInfo> s_getRawCache = new();
        private static readonly Dictionary<Type, MethodInfo> s_hasRawCache = new();
        private static readonly Dictionary<Type, MethodInfo> s_removeRawCache = new();
        private static readonly Dictionary<Type, MethodInfo> s_tryRemoveRawCache = new();
        private static readonly Dictionary<Type, MethodInfo> s_setEnabledRawCache = new();
        private static readonly Dictionary<Type, MethodInfo> s_enableRawCache = new();
        private static readonly Dictionary<Type, MethodInfo> s_disableRawCache = new();
        private static readonly Dictionary<Type, MethodInfo> s_isEnabledRawCache = new();
        private static readonly object[] s_emptyObjectBuffer = Array.Empty<object>();
        private static readonly object[] s_singleObjectBuffer = new object[1];
        private static readonly object[] s_doubleObjectBuffer = new object[2];

        public World World => Worlds.Has(worldId) ? Worlds.Get(worldId) : default;
        internal Bitset Bitset => Worlds.Get(worldId).GetEntityComponentBitset(id);
        internal List<Type> ComponentTypes => World.GetEntityComponentTypes(this);
        internal int ComponentCount => World.GetEntityComponentTypes(this).Count;

        internal Entity(int id, ushort gen, in short worldId) {
            this.id = id;
            this.gen = gen;
            this.worldId = worldId;
        }
        
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod]
        private static void InitializeOnLoad() {
            s_setRawCache.Clear();
            s_getRawCache.Clear();
            s_removeRawCache.Clear();
            s_tryRemoveRawCache.Clear();
            s_setEnabledRawCache.Clear();
            s_isEnabledRawCache.Clear();
        }
#endif

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
                Publishers<EntityComponentAddedEvent<T>>.WorldInstance.Publish(new EntityComponentAddedEvent<T>(this), worldId);
                var previousStash = Stashes<T>.GetOrCreatePrevious(worldId);
                previousStash.Set(id, Get<T>());
                World.AddEntityComponentType(this, typeof(T));
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
            ref var comp = ref stash.Get(id);
            if (IsEnabled())
                Disable<T>();
            Publishers<EntityComponentRemovingEvent<T>>.WorldInstance.Publish(new EntityComponentRemovingEvent<T>(this), worldId);
            stash.Remove(id);
            World.SetComponentBit<T>(this, false);
            Publishers<EntityComponentRemovedEvent<T>>.WorldInstance.Publish(new EntityComponentRemovedEvent<T>(this, comp), worldId);
            World.RemoveEntityComponentType(this, typeof(T));
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
        public IEcsComponent SetRaw(in IEcsComponent component, in Type type, bool enableIfAdded = true) {
            var method = GetOrCreateGenericMethod(s_setRawCache, type, nameof(SetRawInternal));
            s_doubleObjectBuffer[0] = component;
            s_doubleObjectBuffer[1] = enableIfAdded;
            return method.Invoke(this, s_doubleObjectBuffer) as IEcsComponent;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEcsComponent SetRaw(in IEcsComponent component, bool enableIfAdded = true) => 
            SetRaw(component, component.GetType(), enableIfAdded);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveRaw(in Type type) {
            var method = GetOrCreateGenericMethod(s_removeRawCache, type, nameof(RemoveRawInternal));
            method.Invoke(this, s_emptyObjectBuffer);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEcsComponent GetRaw(in Type type) {
            var method = GetOrCreateGenericMethod(s_getRawCache, type, nameof(GetRawInternal));
            return (IEcsComponent)method.Invoke(this, s_emptyObjectBuffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasRaw(in Type type) {
            var method = GetOrCreateGenericMethod(s_hasRawCache, type, nameof(HasRawInternal));
            return (bool)method.Invoke(this, s_emptyObjectBuffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryRemoveRaw(in Type type) {
            var method = GetOrCreateGenericMethod(s_tryRemoveRawCache, type, nameof(TryRemoveRawInternal));
            return (bool)method.Invoke(this, s_emptyObjectBuffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetEnabledRaw(in Type type, bool value) {
            var method = GetOrCreateGenericMethod(s_setEnabledRawCache, type, nameof(SetEnabledRawInternal));
            s_singleObjectBuffer[0] = value;
            method.Invoke(this, s_singleObjectBuffer);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnableRaw(in Type type) {
            var method = GetOrCreateGenericMethod(s_enableRawCache, type, nameof(EnableRawInternal));
            method.Invoke(this, s_emptyObjectBuffer);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DisableRaw(in Type type) {
            var method = GetOrCreateGenericMethod(s_disableRawCache, type, nameof(DisableRawInternal));
            method.Invoke(this, s_emptyObjectBuffer);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEnabledRaw(in Type type) {
            var method = GetOrCreateGenericMethod(s_isEnabledRawCache, type, nameof(IsEnabledRawInternal));
            return (bool)method.Invoke(this, s_emptyObjectBuffer);
        }

        private MethodInfo GetOrCreateGenericMethod(in Dictionary<Type, MethodInfo> cache, in Type type, in string methodName) {
            if (cache.TryGetValue(type, out var method)) return method;
            method = GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance)!.MakeGenericMethod(type);
            cache.Add(type, method);
            return method;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T SetRawInternal<T>(T component, bool enableIfAdded) where T : IEcsComponent => Set(component, enableIfAdded);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T GetRawInternal<T>() where T : IEcsComponent => Get<T>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool HasRawInternal<T>() where T : IEcsComponent => Has<T>();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RemoveRawInternal<T>() where T : IEcsComponent => Remove<T>();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryRemoveRawInternal<T>() where T : IEcsComponent => TryRemove<T>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnableRawInternal<T>() where T : IEcsComponent => Enable<T>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DisableRawInternal<T>() where T : IEcsComponent => Disable<T>();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetEnabledRawInternal<T>(bool value) where T : IEcsComponent => SetEnabled<T>(value);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsEnabledRawInternal<T>() where T : IEcsComponent => IsEnabled<T>();
    }
}