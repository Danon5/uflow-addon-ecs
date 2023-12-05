namespace UFlow.Addon.ECS.Core.Runtime {
    internal readonly struct WorldComponentEnabledEvent<T> {
    }

    public delegate void WorldComponentEnabledHandler<T>(ref T component);
}