using System;
using System.Runtime.InteropServices;

namespace UFlow.Addon.Entities.Core.Runtime {
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct Entity : IEquatable<Entity> {
        [FieldOffset(0)] internal readonly uint id;
        [FieldOffset(4)] internal readonly uint gen;
        [FieldOffset(8)] internal readonly ushort worldId;

        public static implicit operator uint(in Entity entity) => entity.id;
        
        public bool Equals(Entity other) => id == other.id && gen == other.gen && worldId == other.worldId;
        
        public override bool Equals(object obj) => obj is Entity other && Equals(other);
        
        public override int GetHashCode() => HashCode.Combine(id, gen);
    }
}