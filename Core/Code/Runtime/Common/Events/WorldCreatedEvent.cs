namespace UFlow.Addon.Entities.Core.Runtime {
    internal readonly struct WorldCreatedEvent {
        public readonly World world;

        public WorldCreatedEvent(World world) {
            this.world = world;
        }
    }
}