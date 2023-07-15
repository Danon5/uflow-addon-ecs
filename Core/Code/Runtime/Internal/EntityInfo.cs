using System;
using System.Collections.Generic;

namespace UFlow.Addon.Ecs.Core.Runtime {
#if IL2CPP_ENABLED
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    internal struct EntityInfo {
        public ushort gen;
        public Bitset bitset;
        public List<Type> componentTypes;
    }
}