using System.Runtime.CompilerServices;

namespace UFlow.Addon.Entities.Core.Runtime {
    public abstract class BaseSystem : IPreSetupSystem,
                                     ISetupSystem,
                                     IPreCleanupSystem,
                                     ICleanupSystem,
                                     IResetSystem{
        private readonly World m_world;

        public BaseSystem(in World world) => m_world = world;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PreSetup() => PreSetup(m_world);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Setup() => Setup(m_world);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PreCleanup() => PreCleanup(m_world);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Cleanup() => Cleanup(m_world);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset() => Reset(m_world);
        protected virtual void PreSetup(World world) { }
        
        protected virtual void Setup(World world) { }

        protected virtual void PreCleanup(World world) { }
        
        protected virtual void Cleanup(World world) { }
        
        protected virtual void Reset(World world) { }
    }
}