namespace UFlow.Addon.Ecs.Core.Runtime {
    internal static class Bits {
        internal static readonly Bit IsAlive = Bit.GetNextBit();
        internal static readonly Bit IsEnabled = Bit.GetNextBit();
    }
}