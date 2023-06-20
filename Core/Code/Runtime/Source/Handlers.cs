namespace UFlow.Core.Runtime {
    public delegate void PublishedEventHandler<T>(in T @event);

    public delegate void WorldDestroyedHandler(World world);

    public delegate void EntityCreatedHandler(in Entity entity);

    public delegate void EntityDestroyedHandler(in Entity entity);
    
    public delegate void EntityEnabledHandler(in Entity entity);
    
    public delegate void EntityDisabledHandler(in Entity entity);

    public delegate void EntityComponentAddedHandler<T>(in Entity entity, in T component);
    
    public delegate void EntityComponentRemovedHandler<T>(in Entity entity, in T component);
}