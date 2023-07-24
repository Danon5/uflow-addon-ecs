using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
// ReSharper disable SuspiciousTypeConversion.Global

namespace UFlow.Addon.ECS.Core.Runtime {
    public abstract class BaseSystemGroup : IEnumerable<ISystem> {
        private readonly List<ISystem> m_systems;
        private bool m_enabled;
        
        internal BaseSystemGroup() {
            m_systems = new List<ISystem>();
        }

        public BaseSystemGroup Add(in ISystem system) {
            if (system is IRunSystem runSystem)
                runSystem.Enable();
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
        public void Run() {
            foreach (var system in m_systems) {
                if (system is IRunSystem runSystem && runSystem.IsEnabled())
                    runSystem.PreRun();
            }
            
            foreach (var system in m_systems) {
                if (system is IRunSystem runSystem && runSystem.IsEnabled())
                    runSystem.Run();
            }
            
            foreach (var system in m_systems) {
                if (system is IRunSystem runSystem && runSystem.IsEnabled())
                    runSystem.PostRun();
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
            var systemBuffer = new List<ISystem>(m_systems);
            
            foreach (var system in m_systems) {
                var systemType = system.GetType();
                foreach (var otherSystem in m_systems) {
                    if (ReferenceEquals(system, otherSystem)) continue;
                    var otherSystemType = otherSystem.GetType();
                    
                    if (ShouldPlaceBefore(systemType, otherSystemType)) {
                        MoveValueTo(systemBuffer, systemBuffer.IndexOf(system), 
                            Math.Max(systemBuffer.IndexOf(otherSystem) - 1, 0));
                        break;
                    }
                    
                    if (ShouldPlaceAfter(systemType, otherSystemType)) {
                        MoveValueTo(systemBuffer, systemBuffer.IndexOf(system), 
                            Math.Min(systemBuffer.IndexOf(otherSystem) + 1, systemBuffer.Count - 1));
                        break;
                    }
                }
            }
            
            m_systems.Clear();
            m_systems.AddRange(systemBuffer);
        }

        private static void MoveValueTo(in List<ISystem> list, in int a, in int b) {
            var temp = list[a];
            list.RemoveAt(a);
            if (b >= list.Count)
                list.Add(temp);
            else
                list.Insert(b, temp);
        }

        private static bool ShouldPlaceBefore(in Type sourceType, in Type otherType) {
            var beforeAttribute = sourceType.GetCustomAttribute<ExecuteBeforeAttribute>();
            return beforeAttribute != null && beforeAttribute.SystemType == otherType;
        }
        
        private static bool ShouldPlaceAfter(in Type sourceType, in Type otherType) {
            var afterAttribute = sourceType.GetCustomAttribute<ExecuteAfterAttribute>();
            return afterAttribute != null && afterAttribute.SystemType == otherType;
        }
    }
}