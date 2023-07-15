using System.Runtime.CompilerServices;

namespace UFlow.Addon.Ecs.Core.Runtime {
    public abstract class BaseSetIterationSystem : IPreSetupSystem, ISetupSystem, IRunSystem, IPreCleanupSystem, ICleanupSystem {
        private readonly World m_world;
        private readonly DynamicEntitySet m_query;

        public BaseSetIterationSystem(in World world, QueryBuilder query) {
            m_world = world;
            m_query = query.AsSet();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PreSetup() => PreSetup(m_world);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Setup() {
            Setup(m_world);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PreRun() => PreIterate(m_world);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Run() {
            foreach (var entity in m_query)
                IterateEntity(m_world, entity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PostRun() => PostIterate(m_world);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PreCleanup() => PreCleanup(m_world);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Cleanup() => Cleanup(m_world);

        protected virtual void PreSetup(World world) { }
        protected virtual void Setup(World world) { }

        protected virtual void PreIterate(World world) { }

        protected virtual void IterateEntity(World world, in Entity entity) { }

        protected virtual void PostIterate(World world) { }
        
        protected virtual void PreCleanup(World world) { }
        protected virtual void Cleanup(World world) { }
    }
}