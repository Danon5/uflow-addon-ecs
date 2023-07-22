using System.Runtime.CompilerServices;

namespace UFlow.Addon.ECS.Core.Runtime {
    public abstract class BaseRunSystem : IPreSetupSystem, ISetupSystem, IRunSystem, IPreCleanupSystem, ICleanupSystem {
        private readonly World m_world;
        
        public BaseRunSystem(in World world) {
            m_world = world;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PreSetup() => PreSetup(m_world);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Setup() => Setup(m_world);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PreRun() => PreRun(m_world);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Run() => Run(m_world);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PostRun() => PostRun(m_world);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PreCleanup() => PreCleanup(m_world);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Cleanup() => Cleanup(m_world);

        protected virtual void PreSetup(World world) { }
        
        protected virtual void Setup(World world) { }
        
        protected virtual void PreRun(World world) { }
        
        protected virtual void Run(World world) { }
        
        protected virtual void PostRun(World world) { }
        
        protected virtual void PreCleanup(World world) { }
        
        protected virtual void Cleanup(World world) { }
    }
}