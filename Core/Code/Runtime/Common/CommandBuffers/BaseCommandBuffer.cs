using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UFlow.Addon.Entities.Core.Runtime {
    public abstract class BaseCommandBuffer : IDisposable {
        internal event Action<BaseCommandBuffer> OnDisposed;
        private static readonly EcsIdStack s_idStack;
        private static readonly Dictionary<Type, TryExecuteNextCommandDelegate> s_delegates = new();
        private readonly Queue<Type> m_queue = new();

        internal int Id { get; }

        static BaseCommandBuffer() {
            s_idStack = new EcsIdStack(0);
            ExternalEngineEvents.clearStaticCachesEvent += ClearStaticCache;
        }

        protected BaseCommandBuffer() => Id = s_idStack.GetNextId();

        public void Dispose() {
            OnDisposed?.Invoke(this);
            s_idStack.RecycleId(Id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ExecuteCommands() {
            while (m_queue.TryDequeue(out var commandType))
                GetOrCreateTryExecuteNextCommandDelegate(commandType).Invoke(Id);
        }

        protected void EnqueueCommand<T>(in Entity entity, in T command) where T : IEcsCommand {
            EnsureCommandQueueAllocated<T>();
            Commands<T>.Enqueue(Id, entity, command);
            m_queue.Enqueue(typeof(T));
        }
        
        private static void ClearStaticCache() {
            s_idStack.Reset();
            s_delegates.Clear();
        }

        private void EnsureCommandQueueAllocated<T>() where T : IEcsCommand {
            if (!Commands<T>.HasQueue(this))
                Commands<T>.AllocateQueue(this);
        }
        
        private static TryExecuteNextCommandDelegate GetOrCreateTryExecuteNextCommandDelegate(Type commandType) {
            if (s_delegates.TryGetValue(commandType, out var @delegate)) return @delegate;
            @delegate = typeof(Commands<>).MakeGenericType(commandType).GetMethod("TryExecuteNextCommand")
                !.CreateDelegate(typeof(TryExecuteNextCommandDelegate)) as TryExecuteNextCommandDelegate;
            s_delegates.Add(commandType, @delegate);
            return @delegate;
        }

        private delegate bool TryExecuteNextCommandDelegate(int id);
    }
}