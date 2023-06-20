namespace UFlow.Core.Runtime {
    public class BaseDeltaSystem : IInitSystem, IRunDeltaSystem {
        protected World World { get; private set; }
        
        public void Init(World world) {
            World = world;
            Init();
        }

        public void Run(World world, float delta) {
            Run(delta);
        }

        protected virtual void Init() {
        }

        protected virtual void Run(float delta) {
        }
    }
}