using System;

namespace UFlow.Addon.Entities.Core.Runtime {
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ExecuteAfterAttribute : Attribute {
        public Type[] SystemTypes { get; }

        public ExecuteAfterAttribute(params Type[] systemTypes) {
            SystemTypes = systemTypes;
        }
    }
}