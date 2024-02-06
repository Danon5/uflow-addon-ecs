namespace UFlow.Addon.Entities.Core.Runtime {
    internal readonly struct SetWithNotifyCommand<T> : IEcsCommand where T : IEcsComponentData {
        private readonly T m_component;
        private readonly bool m_enableIfAdded;

        public SetWithNotifyCommand(in T component, bool enableIfAdded) {
            m_component = component;
            m_enableIfAdded = enableIfAdded;
        }

        public void Execute(in Entity entity) => entity.SetWithNotify(m_component, m_enableIfAdded);
    }
}