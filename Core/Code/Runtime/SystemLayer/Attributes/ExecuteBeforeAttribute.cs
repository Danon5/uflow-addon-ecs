using System;

namespace UFlow.Addon.Entities.Core.Runtime {
    public sealed class ExecuteBeforeAttribute : Attribute {
        public Type[] SystemTypes { get; }

        public ExecuteBeforeAttribute(params Type[] systemTypes) {
            SystemTypes = systemTypes;
        }
    }
}