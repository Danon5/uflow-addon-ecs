using System;

namespace UFlow.Addon.ECS.Core.Runtime {
    public sealed class ExecuteBeforeAttribute : Attribute {
        public Type SystemType { get; }

        public ExecuteBeforeAttribute(Type systemType) {
            SystemType = systemType;
        }
    }
}