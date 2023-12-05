namespace UFlow.Addon.ECS.Core.Runtime {
    internal readonly struct EnsureAddedAndSetEnabled<T> : IEcsCommand where T : IEcsComponent {
        private readonly bool m_enabled;

        public EnsureAddedAndSetEnabled(bool enabled) => m_enabled = enabled;

        public void Execute(in Entity entity) => entity.EnsureAddedAndSetEnabled<T>(m_enabled);
    }
}