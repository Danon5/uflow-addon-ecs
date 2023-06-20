using System.Collections.Generic;

namespace UFlow.Addon.Ecs.Core.Runtime {
    public sealed class SystemGroup {
        private readonly World m_world;
        private readonly List<ISystem> m_systems;

        public SystemGroup(in World world) {
            m_world = world;
            m_systems = new List<ISystem>();
        }

        public SystemGroup Add(in ISystem system) {
            m_systems.Add(system);
            return this;
        }

        /// <summary>
        /// Generally shouldn't be used.
        /// </summary>
        public SystemGroup Remove(in ISystem system) {
            m_systems.Remove(system);
            return this;
        }

        public void Init() {
            foreach (var system in m_systems) {
                if (system is IInitSystem initSystem)
                    initSystem.Init(m_world);
            }
        }

        public void Run(float delta = 0f) {
            foreach (var system in m_systems) {
                switch (system) {
                    case IRunSystem runSystem:
                        runSystem.Run(m_world);
                        break;
                    case IRunDeltaSystem deltaRunSystem:
                        deltaRunSystem.Run(m_world, delta);
                        break;
                }
            }
        }
    }
}