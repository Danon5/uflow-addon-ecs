using System;
using System.Collections.Generic;
using System.Linq;

namespace UFlow.Addon.Ecs.Core.Runtime {
    internal sealed class EcsDisposableGroup : IDisposable {
        private readonly IDisposable[] m_disposables;
        private bool m_disposed;

        public EcsDisposableGroup(in IEnumerable<IDisposable> disposables) {
            m_disposables = disposables.ToArray();
        }

        ~EcsDisposableGroup() {
            Dispose();
        }
        
        public void Dispose() {
            if (m_disposed) return;
            m_disposed = true;
            for (var i = m_disposables.Length - 1; i >= 0; i--)
                m_disposables[i].Dispose();
            GC.SuppressFinalize(this);
        }
    }
}