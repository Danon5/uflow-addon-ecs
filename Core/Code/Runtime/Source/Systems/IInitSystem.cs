namespace UFlow.Core.Runtime {
    public interface IInitSystem : ISystem {
        void Init(World world);
    }
}