namespace UFlow.Addon.ECS.Core.Runtime {
    internal readonly struct WorldComponentRemovingEvent<T> {
    }

    public delegate void WorldComponentRemovingHandler<T>(ref T component);
}