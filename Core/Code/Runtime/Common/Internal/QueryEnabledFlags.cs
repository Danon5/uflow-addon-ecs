using System;

namespace UFlow.Addon.Entities.Core.Runtime {
    [Flags]
    public enum QueryEnabledFlags : byte {
        Enabled = 0,
        Disabled = 1
    }
}