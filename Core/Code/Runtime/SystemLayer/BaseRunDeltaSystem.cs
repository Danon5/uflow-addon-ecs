using System.Runtime.CompilerServices;
using Rewired.Utils.Attributes;

namespace UFlow.Addon.Entities.Core.Runtime {
    [Preserve]
    public abstract class BaseRunDeltaSystem : IPreSetupSystem, 
                                               ISetupSystem, 
                                               IRunDeltaSystem, 
                                               IPreCleanupSystem, 
                                               ICleanupSystem, 
                                               IResetSystem,
                                               IEnableDisableSystem {
        private readonly World m_world;
        private bool m_enabled;
        
        protected World World => m_world;
        protected EntityCommandBuffer CommandBuffer { get; }
        
        public BaseRunDeltaSystem(in World world) {
            m_world = world;
            CommandBuffer = new EntityCommandBuffer();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PreSetup() => PreSetup(m_world);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Setup() => Setup(m_world);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PreRun(float delta) => PreRun(m_world, delta);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Run(float delta) => Run(m_world, delta);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PostRun(float delta) {
            ExecuteCommandBuffers();
            PostRun(m_world, delta);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetEnabled(bool value) => m_enabled = value;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Enable() => m_enabled = true;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Disable() => m_enabled = false;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEnabled() => m_enabled;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PreCleanup() => PreCleanup(m_world);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Cleanup() {
            Cleanup(m_world);
            CommandBuffer.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset() => Reset(m_world);
        
        internal virtual void ExecuteCommandBuffers() {
            CommandBuffer.ExecuteCommands();
        }

        protected virtual void PreSetup(World world) { }
        
        protected virtual void Setup(World world) { }
        
        protected virtual void PreRun(World world, float delta) { }
        
        protected virtual void Run(World world, float delta) { }
        
        protected virtual void PostRun(World world, float delta) { }
        
        protected virtual void PreCleanup(World world) { }
        
        protected virtual void Cleanup(World world) { }

        protected virtual void Reset(World world) { }
    }
}