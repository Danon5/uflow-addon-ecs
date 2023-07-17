﻿using System;
using System.Runtime.CompilerServices;
using UFlow.Core.Runtime;

namespace UFlow.Addon.Ecs.Core.Runtime {
#if IL2CPP_ENABLED
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    internal static class Worlds {
        private static readonly IdStack s_worldIdStack;
        private static World[] s_worlds;

        static Worlds() {
            s_worldIdStack = new IdStack(0);
            s_worlds = Array.Empty<World>();
            ExternalEngineEvents.clearStaticCachesEvent += ClearStaticCaches;
        }

        public static World AddWorld(in World world) {
            var id = world.id;
            if (id >= short.MaxValue) throw new Exception("Created too many worlds!");
            UFlowUtils.Collections.EnsureIndex(ref s_worlds, id);
            s_worlds[id] = world;
            Publishers<WorldCreatedEvent>.Global.Publish(new WorldCreatedEvent(world));
            return world;
        }

        public static void DestroyWorld(in World world) {
            Publishers<WorldDestroyingEvent>.Global.Publish(new WorldDestroyingEvent(world.id));
            Publishers<WorldDestroyingEvent>.WorldInstance.Publish(new WorldDestroyingEvent(world.id), world.id);
            s_worlds[world.id] = null;
            s_worldIdStack.RecycleId(world.id);
            Publishers<WorldDestroyedEvent>.Global.Publish(new WorldDestroyedEvent(world.id));
            Publishers<WorldDestroyedEvent>.WorldInstance.Publish(new WorldDestroyedEvent(world.id), world.id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static World Get(short worldId) => s_worlds[worldId];

        public static short GetNextId() => (short)s_worldIdStack.GetNextId();
        
        private static void ClearStaticCaches() {
            s_worldIdStack.Reset();
            foreach (var world in s_worlds)
                world?.Destroy();
            s_worlds = Array.Empty<World>();
        }
    }
}