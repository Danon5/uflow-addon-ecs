namespace UFlow.Addon.Entities.Core.Runtime {
    internal readonly struct WorldComponentDisablingEvent<T> {
    }

    public delegate void WorldComponentDisablingHandler<T>(ref T component);
}