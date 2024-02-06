namespace UFlow.Addon.Entities.Core.Runtime {
    internal sealed class RawComponentMethods<T> : IRawComponentMethods where T : IEcsComponentData {
        public void InvokeSet(in Entity entity, IEcsComponentData value, bool enableIfAdded) => entity.Set((T)value, enableIfAdded);
        
        public void InvokeSet(in Entity entity, bool enableIfAdded) => entity.Set<T>(default, enableIfAdded);

        public IEcsComponentData InvokeGet(in Entity entity) => entity.Get<T>();

        public bool InvokeHas(in Entity entity) => entity.Has<T>();

        public void InvokeRemove(in Entity entity) => entity.Remove<T>();

        public bool InvokeTryRemove(in Entity entity) => entity.TryRemove<T>();

        public void InvokeSetEnabled(in Entity entity, bool enabled) => entity.SetEnabled<T>(enabled);

        public bool InvokeIsEnabled(in Entity entity) => entity.IsEnabled<T>();
    }
}