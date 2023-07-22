using System;

namespace UFlow.Addon.ECS.Core.Runtime {
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ExecuteInWorldAttribute : Attribute {
        public Type WorldType { get; }

        public ExecuteInWorldAttribute(Type worldType) {
            WorldType = worldType;
        }
    }
}