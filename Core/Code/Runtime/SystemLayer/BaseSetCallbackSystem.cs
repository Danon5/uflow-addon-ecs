using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UFlow.Addon.ECS.Core.Runtime {
    [Preserve]
    public abstract class BaseSetCallbackSystem : IPreSetupSystem, 
                                         ISetupSystem,
                                         IPreCleanupSystem, 
                                         ICleanupSystem, 
                                         IResetSystem,
                                         IEnableDisableSystem {
        private readonly World m_world;
        private readonly DynamicEntitySet m_query;
        private bool m_enabled;

        public BaseSetCallbackSystem(in World world, QueryBuilder query) {
            m_world = world;
            m_query = query.AsSet();
            m_query.OnEntityAdded += e => {
                if (!m_enabled) return;
                EntityAdded(m_world, e);
            };
            m_query.OnEntityRemoved += e => {
                if (!m_enabled) return;
                EntityRemoved(m_world, e);
            };
        }

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
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetEnabled(bool value) => m_enabled = value;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Enable() => m_enabled = true;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Disable() => m_enabled = false;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEnabled() => m_enabled;

        protected bool QueryContains(in Entity entity) => m_query.Contains(entity);

        protected virtual void PreSetup(World world) { }
        
        protected virtual void Setup(World world) { }
        
        protected virtual void EntityAdded(World world, in Entity entity) { }
        
        protected virtual void EntityRemoved(World world, in Entity entity) { }
        
        protected virtual void PreCleanup(World world) { }
        
        protected virtual void Cleanup(World world) { }
        
        protected virtual void Reset(World world) { }
    }
}