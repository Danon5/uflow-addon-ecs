﻿using System.Runtime.CompilerServices;

namespace UFlow.Addon.ECS.Core.Runtime {
    public abstract class BaseSetIterationCallbackSystem : IPreSetupSystem, 
                                                           ISetupSystem, 
                                                           IRunSystem, 
                                                           IPreCleanupSystem, 
                                                           ICleanupSystem, 
                                                           IResetSystem, 
                                                           IEnableDisableSystem {
        private readonly World m_world;
        private readonly DynamicEntitySet m_query;
        private bool m_enabled;
        
        protected EntityCommandBuffer CommandBuffer { get; }
        
        public BaseSetIterationCallbackSystem(in World world, QueryBuilder query) {
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
            CommandBuffer = new EntityCommandBuffer();
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
        public void PostRun() {
            CommandBuffer.ExecuteCommands();
            PostIterate(m_world);
        }

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
        
        protected virtual void EntityAdded(World world, in Entity entity) { }
        
        protected virtual void EntityRemoved(World world, in Entity entity) { }

        protected virtual void PreIterate(World world) { }

        protected virtual void IterateEntity(World world, in Entity entity) { }

        protected virtual void PostIterate(World world) { }
        
        protected virtual void PreCleanup(World world) { }
        
        protected virtual void Cleanup(World world) { }

        protected virtual void Reset(World world) { }
    }
}