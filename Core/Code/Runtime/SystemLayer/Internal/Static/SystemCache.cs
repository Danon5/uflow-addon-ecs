using Unity.Burst;
using Unity.Jobs;

namespace UFlow.Addon.Entities.Core.Runtime {
    internal static class SystemCache {
        public static void Test() {
            var job = new RunSystemJob<TestSystem> {
                world = new World(),
                system = new TestSystem()
            };
            var handle = job.Schedule();
            handle.Complete();
        }
        
        [BurstCompile]
        private struct RunSystemJob<T> : IJob where T : unmanaged, IRunSystem {
            public World world;
            public T system;

            public void Execute() => system.Run(world);
        }
    }
}