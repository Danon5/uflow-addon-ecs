namespace UFlow.Addon.Entities.Core.Runtime {
    internal readonly struct WorldComponentEnabledEvent<T> {
    }

    public delegate void WorldComponentEnabledHandler<T>(ref T component);
}