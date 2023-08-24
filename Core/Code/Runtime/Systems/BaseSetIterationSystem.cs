using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UFlow.Addon.ECS.Core.Runtime {
    [Preserve]
    public abstract class BaseSetIterationSystem : IPreSetupSystem, 
                                                   ISetupSystem, 
                                                   IRunSystem, 
                                                   IPreCleanupSystem, 
                                                   ICleanupSystem, 
                                                   IResetSystem {
        private readonly World m_world;
        private readonly DynamicEntitySet m_query;
        private bool m_enabled;
        
        public BaseSetIterationSystem(in World world, QueryBuilder query) {
            m_world = world;
            m_query = query.AsSet();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PreSetup() => PreSetup(m_world);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Setup() => Setup(m_world);

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

        protected virtual void PreSetup(World world) { }
        
        protected virtual void Setup(World world) { }

        protected virtual void PreIterate(World world) { }

        protected virtual void IterateEntity(World world, in Entity entity) { }

        protected virtual void PostIterate(World world) { }
        
        protected virtual void PreCleanup(World world) { }
        
        protected virtual void Cleanup(World world) { }

        protected virtual void Reset(World world) { }
    }
}