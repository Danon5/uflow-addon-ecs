namespace UFlow.Addon.ECS.Core.Runtime {
    internal readonly struct WorldComponentDisabledEvent<T> {
    }

    public delegate void WorldComponentDisabledHandler<T>(ref T component);
}