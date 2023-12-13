using System;
using System.Runtime.InteropServices;

namespace UFlow.Addon.ECS.Core.Runtime {
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct Entity : IEquatable<Entity> {
        [FieldOffset(0)] public readonly uint id;
        [FieldOffset(4)] public readonly uint gen;
        [FieldOffset(8)] public readonly ushort worldId;

        public static implicit operator uint(in Entity entity) => entity.id;
        
        public bool Equals(Entity other) => id == other.id && gen == other.gen && worldId == other.worldId;
        
        public override bool Equals(object obj) => obj is Entity other && Equals(other);
        
        public override int GetHashCode() => HashCode.Combine(id, gen);
    }
}