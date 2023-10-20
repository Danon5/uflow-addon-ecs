using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UFlow.Addon.ECS.Core.Runtime {
    internal sealed class CommandQueue<T> : ICommandQueue where T : IEcsCommand {
        private readonly Queue<QueuedCommand> m_queue = new();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryExecuteNextCommand() {
            if (!m_queue.TryDequeue(out var command)) return false;
            command.Execute();
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Enqueue(in Entity entity, in T command) => m_queue.Enqueue(new QueuedCommand(entity, command));

        private readonly struct QueuedCommand {
            private readonly Entity m_entity;
            private readonly T m_command;

            public QueuedCommand(in Entity entity, in T command) {
                m_entity = entity;
                m_command = command;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Execute() => m_command.Execute(m_entity);
        }
    }
}