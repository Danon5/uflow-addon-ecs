using System;
using System.Collections.Generic;
using UnityEngine;

namespace UFlow.Addon.Ecs.Core.Runtime {
    public abstract class BaseSystemRunner<T> : MonoBehaviour where T : Enum {
        private readonly Dictionary<T, SystemGroup> m_groups;

        protected BaseSystemRunner() {
            m_groups = new Dictionary<T, SystemGroup>();
        }

        public void AddGroup(T runTiming, in SystemGroup group) {
            m_groups.Add(runTiming, group);
        }

        public void RemoveGroup(T runTiming) {
            m_groups.Remove(runTiming);
        }

        public void InitGroups() {
            foreach (var group in m_groups)
                group.Value.Init();
        }
        
        protected void RunGroup(T runTiming) {
            if (!m_groups.TryGetValue(runTiming, out var group)) return;
            group.Run();
        }
    }
}