using System;

namespace UFlow.Addon.Ecs.Core.Runtime {
    public sealed class ExecuteBeforeAttribute : Attribute {
        public Type SystemType { get; }

        public ExecuteBeforeAttribute(Type systemType) {
            SystemType = systemType;
        }
    }
}