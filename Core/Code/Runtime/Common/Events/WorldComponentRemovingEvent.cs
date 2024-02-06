namespace UFlow.Addon.Entities.Core.Runtime {
    internal readonly struct WorldComponentRemovingEvent<T> {
    }

    public delegate void WorldComponentRemovingHandler<T>(ref T component);
}