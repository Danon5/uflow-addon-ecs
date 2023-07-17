namespace UFlow.Addon.Ecs.Core.Runtime {
    public static class SaveSerializer {
        private static readonly SaveTypeMap s_map = new();

        static SaveSerializer() {
            s_map.RegisterTypes();
        }

        public static void SerializeComponent<T>(in ByteBuffer buffer, in T component) where T : IEcsComponent {
            
        }
    }
}