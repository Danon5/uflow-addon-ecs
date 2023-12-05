using System.Runtime.CompilerServices;

namespace UFlow.Addon.ECS.Core.Runtime {
    public sealed class EntityCommandBuffer : BaseCommandBuffer {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set<T>(in Entity entity, in T component, bool enableIfAdded) where T : IEcsComponent =>
            EnqueueCommand(entity, new SetCommand<T>(component, enableIfAdded));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetWithNotify<T>(in Entity entity, in T component, bool enableIfAdded) where T : IEcsComponent =>
            EnqueueCommand(entity, new SetWithNotifyCommand<T>(component, enableIfAdded));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void NotifyChanged<T>(in Entity entity) where T : IEcsComponent =>
            EnqueueCommand(entity, new NotifyChangedCommand<T>());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove<T>(in Entity entity) where T : IEcsComponent =>
            EnqueueCommand(entity, new RemoveCommand<T>());
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnsureAddedAndEnabled<T>(in Entity entity) where T : IEcsComponent =>
            EnqueueCommand(entity, new EnsureAddedAndEnabledCommand<T>());
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnsureAddedAndDisabled<T>(in Entity entity) where T : IEcsComponent =>
            EnqueueCommand(entity, new EnsureAddedAndDisabledCommand<T>());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnsureAddedAndSetEnabled<T>(in Entity entity, bool enabled) where T : IEcsComponent =>
            EnqueueCommand(entity, new EnsureAddedAndSetEnabled<T>(enabled));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Destroy(in Entity entity) =>
            EnqueueCommand(entity, new DestroyCommand());
    }
}