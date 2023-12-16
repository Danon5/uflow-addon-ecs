using System;
using System.Runtime.InteropServices;

namespace UFlow.Addon.Entities.Core.Runtime {
#if UFLOW_IL2CPP_ENABLED
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct World : IEquatable<World> {
        [FieldOffset(0)] internal readonly ushort id;
        [FieldOffset(2)] internal readonly int gen;

        public static implicit operator ushort(in World world) => world.id;
        
        public bool Equals(World other) => id == other.id && gen == other.gen;
        
        public override bool Equals(object obj) => obj is World other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(id, gen);
    }
}