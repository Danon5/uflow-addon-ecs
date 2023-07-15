namespace UFlow.Addon.Ecs.Core.Runtime {
    internal readonly struct WorldComponentChangedEvent<T> {
    }

    public delegate void WorldComponentChangedHandler<T>(in T oldValue, ref T newValue);
}