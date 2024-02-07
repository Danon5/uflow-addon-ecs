using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UFlow.Addon.Entities.Core.Runtime {
    [Preserve]
    public abstract class BaseSetIterationSystem : IPreSetupSystem, 
                                                   ISetupSystem, 
                                                   IRunSystem, 
                                                   IPreCleanupSystem, 
                                                   ICleanupSystem, 
                                                   IResetSystem, 
                                                   IEnableDisableSystem {
        private readonly World m_world;
        private bool m_enabled;

        protected World World => m_world;
        protected DynamicEntitySet Query { get; }
        protected EntityCommandBuffer CommandBuffer { get; }

        public BaseSetIterationSystem(in World world, QueryBuilder query) {
            m_world = world;
            Query = query.AsSet();
            CommandBuffer = new EntityCommandBuffer();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PreSetup() => PreSetup(m_world);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Setup() => Setup(m_world);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PreRun() => PreRun(m_world);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Run() {
            PreIterate(m_world);
            foreach (var entity in Query)
                IterateEntity(m_world, entity);
            ExecuteCommandBuffers();
            PostIterate(m_world);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PostRun() => PostRun(m_world);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PreCleanup() => PreCleanup(m_world);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Cleanup() {
            Cleanup(m_world);
            CommandBuffer.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset() => Reset(m_world);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetEnabled(bool value) => m_enabled = value;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Enable() => m_enabled = true;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Disable() => m_enabled = false;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEnabled() => m_enabled;
        
        internal virtual void ExecuteCommandBuffers() => CommandBuffer.ExecuteCommands();

        protected virtual void PreSetup(World world) { }
        
        protected virtual void Setup(World world) { }
        
        protected virtual void PreRun(World world) { }

        protected virtual void PreIterate(World world) { }

        protected virtual void IterateEntity(World world, in Entity entity) { }

        protected virtual void PostIterate(World world) { }
        
        protected virtual void PostRun(World world) { }
        
        protected virtual void PreCleanup(World world) { }
        
        protected virtual void Cleanup(World world) { }

        protected virtual void Reset(World world) { }
    }
}