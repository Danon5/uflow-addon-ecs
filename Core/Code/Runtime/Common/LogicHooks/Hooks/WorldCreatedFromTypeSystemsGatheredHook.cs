using System;
using System.Collections.Generic;
using UFlow.Core.Runtime;

namespace UFlow.Addon.Entities.Core.Runtime {
    public readonly struct WorldCreatedFromTypeSystemsGatheredHook : IHook {
        public readonly World world;
        public readonly Type worldType;
        public readonly List<EcsUtils.Worlds.ReflectedSystemInfo> systemInfos;
        
        public WorldCreatedFromTypeSystemsGatheredHook(World world, 
                                                       Type worldType, 
                                                       List<EcsUtils.Worlds.ReflectedSystemInfo> systemInfos) {
            this.world = world;
            this.worldType = worldType;
            this.systemInfos = systemInfos;
        }
    }
}