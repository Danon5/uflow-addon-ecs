namespace UFlow.Addon.Ecs.Core.Runtime {
    internal interface ISerializer<T> {
        public void Serialize(in ByteBuffer buffer, ref T value);
        public void Deserialize(in ByteBuffer buffer, ref T value);
    }
}