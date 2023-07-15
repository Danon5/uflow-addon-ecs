using System;

namespace UFlow.Addon.Ecs.Core.Runtime {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class UFlowSerializableAttribute : Attribute {
        public readonly ushort id;

        public UFlowSerializableAttribute(ushort id) {
            this.id = id;
        }
    }
}