using System.Collections.Generic;

namespace UFlow.Addon.ECS.Core.Runtime {
#if IL2CPP_ENABLED
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    internal sealed class IdStack {
        private readonly Stack<int> m_stack;
        private readonly int m_startValue;

        public int NextId { get; private set; }

        public IdStack(int startValue) {
            m_startValue = startValue;
            m_stack = new Stack<int>();
            NextId = startValue;
        }

        public int GetNextId() {
            if (!m_stack.TryPop(out var nextId))
                nextId = NextId++;
            return nextId;
        }

        public void RecycleId(int id) => m_stack.Push(id);

        public void Reset() {
            m_stack.Clear();
            NextId = m_startValue;
        }

        public void OverrideNext(int id) => NextId = id;
    }
}