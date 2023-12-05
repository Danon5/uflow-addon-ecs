namespace UFlow.Addon.ECS.Core.Runtime {
    internal readonly struct WorldDestroyingEvent {
        public readonly short worldId;

        public WorldDestroyingEvent(short worldId) {
            this.worldId = worldId;
        }
    }

    public delegate void WorldDestroyingHandler(in World world);
}