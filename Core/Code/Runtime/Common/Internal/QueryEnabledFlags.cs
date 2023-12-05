using System;

namespace UFlow.Addon.ECS.Core.Runtime {
    [Flags]
    public enum QueryEnabledFlags : byte {
        Enabled = 0,
        Disabled = 1
    }
}