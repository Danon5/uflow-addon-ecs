using System;

namespace UFlow.Addon.Entities.Core.Runtime {
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ExecuteInGroupAttribute : Attribute {
        public Type[] GroupTypes { get; }
        
        public ExecuteInGroupAttribute(params Type[] types) {
            GroupTypes = types;
        }
    }
}