namespace UFlow.Addon.ECS.Core.Runtime {
    internal sealed class RawComponentMethod<T> : IRawComponentMethod where T : IEcsComponent {
        public void InvokeSet(in Entity entity, IEcsComponent value, bool enableIfAdded) => entity.Set((T)value, enableIfAdded);
        
        public IEcsComponent InvokeGet(in Entity entity) => entity.Get<T>();
        
        public bool InvokeHas(in Entity entity) => entity.Has<T>();
        
        public void InvokeRemove(in Entity entity) => entity.Remove<T>();
        
        public bool InvokeTryRemove(in Entity entity) => entity.TryRemove<T>();
        
        public void InvokeSetEnabled(in Entity entity, bool enabled) => entity.SetEnabled<T>(enabled);
        
        public bool InvokeIsEnabled(in Entity entity) => entity.IsEnabled<T>();
    }
}