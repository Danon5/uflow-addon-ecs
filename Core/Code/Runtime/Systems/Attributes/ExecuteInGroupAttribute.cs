using System;

namespace UFlow.Addon.ECS.Core.Runtime {
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ExecuteInGroupAttribute : Attribute {
        public Type GroupType { get; }
        
        public ExecuteInGroupAttribute(Type type) {
            GroupType = type;
        }
    }
}