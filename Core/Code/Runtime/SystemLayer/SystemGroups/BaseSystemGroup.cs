using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UFlow.Core.Shared;

// ReSharper disable SuspiciousTypeConversion.Global

namespace UFlow.Addon.Entities.Core.Runtime {
    public abstract class BaseSystemGroup : IEnumerable<ISystem> {
        private readonly List<ISystem> m_systems;
        private bool m_enabled;
        
        public BaseSystemGroup() {
            m_systems = new List<ISystem>();
        }

        public BaseSystemGroup Add(in ISystem system) {
            if (system is IEnableDisableSystem enableDisableSystem)
                enableDisableSystem.Enable();
            m_systems.Add(system);
            return this;
        }

        public IEnumerator<ISystem> GetEnumerator() {
            return ((IEnumerable<ISystem>)m_systems).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        
        public void Remove(Type type) {
            var system = m_systems.Find(s => s.GetType() == type);
            if (system == null) return;
            m_systems.Remove(system);
        }

        public void Remove<T>() where T : ISystem {
            Remove(typeof(T));
        }

        public bool Has<T>() where T : ISystem {
            var type = typeof(T);
            return m_systems.Select(s => s.GetType() == type).Any();
        }

        public void Setup() {
            foreach (var system in m_systems) {
                if (system is IPreSetupSystem preSetupSystem)
                    preSetupSystem.PreSetup();
            }
            
            foreach (var system in m_systems) {
                if (system is ISetupSystem setupSystem)
                    setupSystem.Setup();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Run(float delta) {
            foreach (var system in m_systems) {
                switch (system) {
                    case IRunSystem runSystem when ShouldRun(system):
                        runSystem.PreRun();
                        break;
                    case IRunDeltaSystem runDeltaSystem when ShouldRun(system):
                        runDeltaSystem.PreRun(delta);
                        break;
                }
            }
            
            foreach (var system in m_systems) {
                switch (system) {
                    case IRunSystem runSystem when ShouldRun(system):
                        runSystem.Run();
                        break;
                    case IRunDeltaSystem runDeltaSystem when ShouldRun(system):
                        runDeltaSystem.Run(delta);
                        break;
                }
            }
            
            foreach (var system in m_systems) {
                switch (system) {
                    case IRunSystem runSystem when ShouldRun(system):
                        runSystem.PostRun();
                        break;
                    case IRunDeltaSystem runDeltaSystem when ShouldRun(system):
                        runDeltaSystem.PostRun(delta);
                        break;
                }
            }
        }

        public void Cleanup() {
            foreach (var system in m_systems) {
                if (system is IPreCleanupSystem preCleanupSystem)
                    preCleanupSystem.PreCleanup();
            }

            foreach (var system in m_systems) {
                if (system is ICleanupSystem cleanupSystem)
                    cleanupSystem.Cleanup();
            }
        }

        public void Reset() {
            foreach (var system in m_systems) {
                if (system is IResetSystem resetSystem)
                    resetSystem.Reset();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetEnabled(bool value) => m_enabled = value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Enable() => m_enabled = true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Disable() => m_enabled = false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEnabled() => m_enabled;

        internal void Sort() {
            var sortedSystems = new List<ISystem>();
            foreach (var system in m_systems)
                AddSortedSystem(system, sortedSystems);
            m_systems.Clear();
            m_systems.AddRange(sortedSystems);
        }

        private void AddSortedSystem(ISystem system, List<ISystem> sortedSystems) {
            if (sortedSystems.Count == 0) {
                sortedSystems.Add(system);
                return;
            }
            var systemsThatMustExecuteBefore = new HashSet<ISystem>();
            PopulateWithSystemsThatExecuteBefore(system, systemsThatMustExecuteBefore);
            foreach (var sortedSystem in sortedSystems) {
                if (!systemsThatMustExecuteBefore.Contains(sortedSystem)) continue;
                sortedSystems.Insert(sortedSystems.IndexOf(sortedSystem), system);
                return;
            }
            sortedSystems.Add(system);
        }

        private void PopulateWithSystemsThatExecuteBefore(ISystem system, ISet<ISystem> systems) {
            var systemType = system.GetType();
            foreach (var otherSystem in m_systems) {
                if (ReferenceEquals(system, otherSystem)) continue;
                var otherType = otherSystem.GetType();
                if (!ShouldExecuteBefore(systemType, otherType)) continue;
                systems.Add(otherSystem);
                PopulateWithSystemsThatExecuteBefore(otherSystem, systems);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ShouldExecuteBefore(Type sourceType, Type otherType) {
            if (UFlowUtils.Reflection.TryGetAttribute(sourceType, out ExecuteBeforeAttribute sourceAttribute) &&
                sourceAttribute.SystemTypes.Contains(otherType))
                return true;
            if (UFlowUtils.Reflection.TryGetAttribute(otherType, out ExecuteAfterAttribute otherAttribute) &&
                otherAttribute.SystemTypes.Contains(sourceType))
                return true;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ShouldRun(ISystem system) =>
            system is not IEnableDisableSystem enableDisableSystem || enableDisableSystem.IsEnabled();
    }
}