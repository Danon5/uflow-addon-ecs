using System;
using UFlow.Core.Runtime;

namespace UFlow.Addon.ECS.Core.Runtime {
    public readonly struct WorldCreatedFromTypeHook : IHook {
        public readonly World world;
        public readonly Type worldType;
        
        public WorldCreatedFromTypeHook(World world, Type worldType) {
            this.world = world;
            this.worldType = worldType;
        }
    }
}