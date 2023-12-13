using System;
using System.Runtime.InteropServices;

namespace UFlow.Addon.ECS.Core.Runtime {
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct World : IEquatable<World> {
        [FieldOffset(0)] internal readonly ushort id;
        [FieldOffset(2)] internal readonly uint gen;

        public static implicit operator ushort(in World world) => world.id;
        
        public bool Equals(World other) => id == other.id;
        
        public override bool Equals(object obj) => obj is World other && Equals(other);
        
        public override int GetHashCode() => id.GetHashCode();
    }
}