namespace UFlow.Core.Runtime {
    public interface IRunDeltaSystem : ISystem {
        void Run(World world, float delta);
    }
}