using System;

namespace UFlow.Addon.Ecs.Core.Runtime {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class EcsSerializableAttribute : Attribute {
        public readonly string id;

        public EcsSerializableAttribute(string id) => this.id = id;
    }
}