namespace UFlow.Addon.ECS.Core.Runtime {
    public interface IRawComponentMethods {
        void InvokeSet(in Entity entity, IEcsComponentData value, bool enableIfAdded);
        void InvokeSet(in Entity entity, bool enableIfAdded);
        IEcsComponentData InvokeGet(in Entity entity);
        bool InvokeHas(in Entity entity);
        void InvokeRemove(in Entity entity);
        bool InvokeTryRemove(in Entity entity);
        void InvokeSetEnabled(in Entity entity, bool enabled);
        bool InvokeIsEnabled(in Entity entity);
    }
}