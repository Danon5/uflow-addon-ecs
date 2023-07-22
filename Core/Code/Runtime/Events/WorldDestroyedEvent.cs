namespace UFlow.Addon.ECS.Core.Runtime {
    internal readonly struct WorldDestroyedEvent {
        public readonly short worldId;

        public WorldDestroyedEvent(short worldId) {
            this.worldId = worldId;
        }
    }

    public delegate void WorldDestroyedHandler();
}