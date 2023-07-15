using System;

namespace UFlow.Addon.Ecs.Core.Runtime {
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ExecuteAfterAttribute : Attribute {
        public Type SystemType { get; }

        public ExecuteAfterAttribute(Type systemType) {
            SystemType = systemType;
        }
    }
}