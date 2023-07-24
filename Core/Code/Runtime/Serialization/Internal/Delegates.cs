namespace UFlow.Addon.ECS.Core.Runtime {
    public delegate void SerializeDelegate<T>(in ByteBuffer buffer, ref T obj);
    public delegate void DeserializeDelegate<T>(in ByteBuffer buffer, ref T obj);
}