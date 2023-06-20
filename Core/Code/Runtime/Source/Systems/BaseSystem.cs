namespace UFlow.Core.Runtime {
    public abstract class BaseSystem : IInitSystem, IRunSystem {
        protected World World { get; private set; }
        
        public void Init(World world) {
            World = world;
            Init();
        }

        public void Run(World world) {
            Run();
        }

        protected virtual void Init() {
        }

        protected virtual void Run() {
        }
    }
}