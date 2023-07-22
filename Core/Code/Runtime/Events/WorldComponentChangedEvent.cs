namespace UFlow.Addon.ECS.Core.Runtime {
    internal readonly struct WorldComponentChangedEvent<T> {
    }

    public delegate void WorldComponentChangedHandler<T>(in T oldValue, ref T newValue);
}