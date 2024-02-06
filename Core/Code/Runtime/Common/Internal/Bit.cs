namespace UFlow.Addon.Entities.Core.Runtime {
#if IL2CPP_ENABLED
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    internal readonly struct Bit {
        public readonly int index;
        public readonly uint value;
        private const uint c_uint_max = 2147483648;
        private static Bit s_lastBit;

        static Bit() {
            s_lastBit = new Bit(0, 1);
        }

        public Bit(int index, uint value) {
            this.index = index;
            this.value = value;
        }

        public static Bit GetNextBit() {
            var bit = s_lastBit;
            s_lastBit = bit.value != c_uint_max ? new Bit(bit.index, bit.value << 1) : new Bit(bit.index + 1, 1);
            return bit;
        }
    }
}