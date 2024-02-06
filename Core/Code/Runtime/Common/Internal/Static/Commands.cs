using System.Collections.Generic;
using System.Runtime.CompilerServices;

// ReSharper disable StaticMemberInGenericType

namespace UFlow.Addon.Entities.Core.Runtime {
    internal static class Commands<T> where T : IEcsCommand {
        private static readonly Dictionary<int, CommandQueue<T>> s_queues = new();

        static Commands() => ExternalEngineEvents.clearStaticCachesEvent += ClearStaticCache;
        public static void AllocateQueue(BaseCommandBuffer buffer) {
            s_queues[buffer.Id] = new CommandQueue<T>();
            buffer.OnDisposed += DisposeQueue;
        }

        public static void DisposeQueue(BaseCommandBuffer buffer) {
            s_queues.Remove(buffer.Id);
            buffer.OnDisposed -= DisposeQueue;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasQueue(BaseCommandBuffer buffer) => s_queues.ContainsKey(buffer.Id);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Enqueue(int id, in Entity entity, in T command) => s_queues[id].Enqueue(entity, command);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryExecuteNextCommand(int id) => s_queues.TryGetValue(id, out var queue) && queue.TryExecuteNextCommand();

        private static void ClearStaticCache() => s_queues.Clear();
    }
}