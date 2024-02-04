namespace UFlow.Addon.ECS.Core.Runtime {
    internal readonly struct SetCommand<T> : IEcsCommand where T : IEcsComponentData {
        private readonly T m_component;
        private readonly bool m_enableIfAdded;

        public SetCommand(in T component, bool enableIfAdded) {
            m_component = component;
            m_enableIfAdded = enableIfAdded;
        }

        public void Execute(in Entity entity) => entity.Set(m_component, m_enableIfAdded);
    }
}