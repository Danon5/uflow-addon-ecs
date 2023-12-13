namespace UFlow.Addon.ECS.Core.Runtime {
    public interface IRunSystem : ISystem {
        void Run(in World world);
    }
}