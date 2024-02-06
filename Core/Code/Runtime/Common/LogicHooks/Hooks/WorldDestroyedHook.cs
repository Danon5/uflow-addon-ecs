using UFlow.Core.Runtime;

namespace UFlow.Addon.Entities.Core.Runtime {
    public readonly struct WorldDestroyedHook : IHook {
        public readonly short worldId;
        
        public WorldDestroyedHook(short worldId) {
            this.worldId = worldId;
        }
    }
}