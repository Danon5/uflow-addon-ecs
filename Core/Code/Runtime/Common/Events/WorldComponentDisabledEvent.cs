namespace UFlow.Addon.Entities.Core.Runtime {
    internal readonly struct WorldComponentDisabledEvent<T> {
    }

    public delegate void WorldComponentDisabledHandler<T>(ref T component);
}