using System;

namespace UFlow.Addon.Entities.Core.Runtime {
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ExecuteInWorldAttribute : Attribute {
        public Type[] WorldTypes { get; }

        public ExecuteInWorldAttribute(params Type[] worldTypes) {
            WorldTypes = worldTypes;
        }
    }
}