using System.Collections.Generic;

namespace UFlow.Addon.ECS.Core.Runtime {
    public sealed class EntityCommandBuffer {
        private readonly Queue<BufferedCommand> m_buffer = new(); 

        public void Destroy(in Entity entity) => m_buffer.Enqueue(new BufferedCommand(entity, new DestroyCommand()));

        public void ExecuteCommands() {
            while (m_buffer.TryDequeue(out var command)) 
                command.Execute();
        }

        private readonly struct BufferedCommand {
            public readonly Entity entity;
            public readonly IEntityCommand command;

            public BufferedCommand(in Entity entity, in IEntityCommand command) {
                this.entity = entity;
                this.command = command;
            }

            public void Execute() => command.Execute(entity);
        }
    }
}