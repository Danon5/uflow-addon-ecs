using System.Runtime.CompilerServices;

namespace UFlow.Addon.Ecs.Core.Runtime {
    public static class SaveSerializer {
        private static readonly SaveTypeMap s_map = new();

        static SaveSerializer() {
            s_map.RegisterTypes();
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
    }
}