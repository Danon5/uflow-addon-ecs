using UFlow.Core.Runtime;

namespace UFlow.Addon.ECS.Core.Runtime {
    public readonly struct EntityDestroyedHook : IHook {
        public readonly short worldId;
        public readonly Entity entity;

        public EntityDestroyedHook(in short worldId, in Entity entity) {
            this.worldId = worldId;
            this.entity = entity;
        }
    }
}