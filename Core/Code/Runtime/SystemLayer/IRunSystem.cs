namespace UFlow.Addon.Entities.Core.Runtime {
    public interface IRunSystem : ISystem {
        void Run(in World world);
    }
}