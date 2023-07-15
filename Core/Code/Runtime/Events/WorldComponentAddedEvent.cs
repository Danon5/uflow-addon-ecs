namespace UFlow.Addon.Ecs.Core.Runtime {
    internal readonly struct WorldComponentAddedEvent<T> {
    }

    public delegate void WorldComponentAddedHandler<T>(ref T component);
}