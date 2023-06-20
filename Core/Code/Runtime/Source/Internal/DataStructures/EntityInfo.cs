namespace UFlow.Core.Runtime.DataStructures {
#if IL2CPP_ENABLED
    using Ecs;
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    internal struct EntityInfo {
        public ushort gen;
        public ComponentBitset componentBitset;
    }
}