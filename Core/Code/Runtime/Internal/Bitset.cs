using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace UFlow.Addon.Ecs.Core.Runtime {
#if IL2CPP_ENABLED
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    internal struct Bitset {
        private uint[] m_bitset;

        public bool IsNull => m_bitset == null;

        public bool this[Bit bit] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => bit.index < m_bitset?.Length && (m_bitset[bit.index] & bit.value) != 0;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set {
                if ((m_bitset?.Length ?? 0) < bit.index + 1) {
                    var newBitset = new uint[bit.index + 1];
                    m_bitset?.CopyTo(newBitset, 0);
                    m_bitset = newBitset;
                }
                if (value)
                    m_bitset![bit.index] |= bit.value;
                else
                    m_bitset![bit.index] &= ~bit.value;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(in Bitset set) {
            if (set.m_bitset == null) return true;
            for (var i = 0; i < set.m_bitset.Length; i++) {
                var bit = set.m_bitset[i];
                if (bit != 0 && (i >= m_bitset.Length || (m_bitset[i] & bit) != bit))
                    return false;
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool DoesNotContain(in Bitset set) {
            if (set.m_bitset == null) return true;
            for (var i = 0; i < set.m_bitset.Length; i++) {
                var bit = set.m_bitset[i];
                if (bit != 0 && i < m_bitset.Length && (m_bitset[i] & bit) != 0)
                    return false;
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bitset Copy() {
            var copy = new Bitset {
                m_bitset = m_bitset?.ToArray()
            };
            return copy;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear() {
            Array.Clear(m_bitset, 0, m_bitset.Length);
        }

        public override unsafe string ToString() {
            fixed (uint* bits = m_bitset) {
                return new string((char*)bits, 0, (m_bitset?.Length ?? 0) * 2);
            }
        }
    }
}