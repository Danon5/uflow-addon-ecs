using UFlow.Core.Runtime;

namespace UFlow.Addon.ECS.Core.Runtime {
    public readonly struct WorldDestroyedHook : IHook {
        public readonly short worldId;
        
        public WorldDestroyedHook(short worldId) {
            this.worldId = worldId;
        }
    }
}