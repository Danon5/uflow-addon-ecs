namespace UFlow.Addon.Ecs.Core.Runtime {
    internal readonly struct WorldComponentRemovedEvent<T> {
        public readonly T component;

        public WorldComponentRemovedEvent(in T component) {
            this.component = component;
        }
    }

    public delegate void WorldComponentRemovedHandler<T>(in T component);
}