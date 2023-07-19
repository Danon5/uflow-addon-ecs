using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace UFlow.Addon.Ecs.Core.Runtime {
    public static class SaveSerializer {
        private static readonly SaveTypeMap s_map = new();
        private static readonly Dictionary<Type, MethodInfo> s_entityComponentSerializeCache = new();
        private static readonly Dictionary<Type, MethodInfo> s_entityComponentDeserializeCache = new();
        private static readonly Dictionary<Type, MethodInfo> s_worldComponentSerializeCache = new();
        private static readonly Dictionary<Type, MethodInfo> s_worldComponentDeserializeCache = new();
        private static readonly object[] s_singleObjectBuffer = new object[1];
        private static readonly object[] s_doubleObjectBuffer = new object[2];
        private static Entity s_currentEntity;

        static SaveSerializer() {
            s_map.RegisterTypes();
            var type = typeof(SaveSerializer);
            foreach (var registeredType in s_map.GetRegisteredTypesEnumerable())
                s_entityComponentSerializeCache.Add(registeredType, 
                    type.GetMethod(nameof(SerializeEntityComponent), BindingFlags.Static | BindingFlags.NonPublic)!
                        .MakeGenericMethod(registeredType));
            foreach (var registeredType in s_map.GetRegisteredTypesEnumerable())
                s_entityComponentDeserializeCache.Add(registeredType, 
                    type.GetMethod(nameof(DeserializeEntityComponent), BindingFlags.Static | BindingFlags.NonPublic)!
                        .MakeGenericMethod(registeredType));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SerializeComponent<T>(in ByteBuffer buffer, ref T component) where T : IEcsComponent {
            buffer.Write(s_map.GetHash(typeof(T)));
            ComponentSerializer<SaveAttribute, T>.Serialize(buffer, ref component);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DeserializeComponent<T>(in ByteBuffer buffer, ref T component) where T : IEcsComponent {
            buffer.ReadInt();
            ComponentSerializer<SaveAttribute, T>.Deserialize(buffer, ref component);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T DeserializeComponent<T>(in ByteBuffer buffer) where T : IEcsComponent, new() {
            var component = new T();
            buffer.ReadInt();
            ComponentSerializer<SaveAttribute, T>.Deserialize(buffer, ref component);
            return component;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SerializeEntity(in ByteBuffer buffer, in Entity entity) {
            s_singleObjectBuffer[0] = buffer;
            s_currentEntity = entity;
            buffer.Write((byte)s_currentEntity.ComponentCount);
            foreach (var componentType in entity.ComponentTypes) {
                buffer.Write(s_map.GetHash(componentType));
                s_entityComponentSerializeCache[componentType].Invoke(null, s_singleObjectBuffer);
            }
            buffer.Write(entity.IsEnabled());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Entity DeserializeEntity(in ByteBuffer buffer, in World world) {
            s_singleObjectBuffer[0] = buffer;
            s_currentEntity = world.CreateEntity();
            var componentCount = (int)buffer.ReadByte();
            for (var i = 0; i < componentCount; i++) {
                var componentType = s_map.GetType(buffer.ReadInt());
                s_entityComponentDeserializeCache[componentType].Invoke(null, s_singleObjectBuffer);
            }
            s_currentEntity.SetEnabled(buffer.ReadBool());
            return s_currentEntity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SerializeWorld(in ByteBuffer buffer, in World world) {
            s_singleObjectBuffer[0] = buffer;
            buffer.Write(world.ComponentCount);
            foreach (var componentType in world.ComponentTypes) {
                buffer.Write(s_map.GetHash(componentType));
                s_entityComponentSerializeCache[componentType].Invoke(null, s_singleObjectBuffer);
            }
            buffer.Write(world.EntityCount);
            foreach (var entity in world.GetEntitiesEnumerable())
                SerializeEntity(buffer, entity);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DeserializeWorld(in ByteBuffer buffer, in World world) {
            s_doubleObjectBuffer[0] = buffer;
            s_doubleObjectBuffer[1] = world;
            world.ResetForDeserialization();
            var componentCount = buffer.ReadInt();
            for (var i = 0; i < componentCount; i++) {
                var componentType = s_map.GetType(buffer.ReadInt());
                s_worldComponentDeserializeCache[componentType].Invoke(null, s_doubleObjectBuffer);
            }
            var entityCount = buffer.ReadInt();
            for (var i = 0; i < entityCount; i++)
                DeserializeEntity(buffer, world);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SerializeEntityComponent<T>(in ByteBuffer buffer) where T : IEcsComponent {
            buffer.Write(s_currentEntity.IsEnabled<T>());
            ComponentSerializer<SaveAttribute, T>.Serialize(buffer, ref s_currentEntity.Get<T>());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DeserializeEntityComponent<T>(in ByteBuffer buffer) where T : IEcsComponent {
            var enabled = buffer.ReadBool();
            ComponentSerializer<SaveAttribute, T>.Deserialize(buffer, ref s_currentEntity.Set<T>(default, enabled));
            s_currentEntity.SetEnabled<T>(enabled);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SerializeWorldComponent<T>(in ByteBuffer buffer, in World world) where T : IEcsComponent {
            buffer.Write(world.IsEnabled<T>());
            ComponentSerializer<SaveAttribute, T>.Serialize(buffer, ref world.Get<T>());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DeserializeWorldComponent<T>(in ByteBuffer buffer, in World world) where T : IEcsComponent {
            var enabled = buffer.ReadBool();
            ComponentSerializer<SaveAttribute, T>.Deserialize(buffer, ref world.Set<T>(default, enabled));
            world.SetEnabled<T>(enabled);
        }
    }
}