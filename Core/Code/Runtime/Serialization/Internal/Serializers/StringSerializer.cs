namespace UFlow.Addon.ECS.Core.Runtime {
    internal sealed class StringSerializer : ISerializer<string> {
        public void Serialize(in ByteBuffer buffer, ref string value) => buffer.Write(value);

        public void Deserialize(in ByteBuffer buffer, ref string value) => buffer.ReadString();
    }
}