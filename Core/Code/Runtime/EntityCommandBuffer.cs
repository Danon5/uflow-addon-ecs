using System.Collections.Generic;

namespace UFlow.Addon.ECS.Core.Runtime {
    public sealed class EntityCommandBuffer {
        private readonly Queue<BufferedCommand> m_buffer = new();

        public void Set<T>(in Entity entity, in T component, bool enableIfAdded = true) where T : IEcsComponent =>
            m_buffer.Enqueue(new BufferedCommand(entity, new SetCommand<T>(component, enableIfAdded)));

        public void SetWithNotify<T>(in Entity entity, in T component, bool enableIfAdded = true) where T : IEcsComponent =>
            m_buffer.Enqueue(new BufferedCommand(entity, new SetWithNotifyCommand<T>(component, enableIfAdded)));

        public void Remove<T>(in Entity entity) where T : IEcsComponent =>
            m_buffer.Enqueue(new BufferedCommand(entity, new RemoveCommand<T>()));

        public void Destroy(in Entity entity) => 
            m_buffer.Enqueue(new BufferedCommand(entity, new DestroyCommand()));

        public void ExecuteCommands() {
            while (m_buffer.TryDequeue(out var command)) 
                command.Execute();
        }

        private readonly struct BufferedCommand {
            public readonly Entity entity;
            public readonly IEcsCommand command;

            public BufferedCommand(in Entity entity, in IEcsCommand command) {
                this.entity = entity;
                this.command = command;
            }

            public void Execute() => command.Execute(entity);
        }
    }
}