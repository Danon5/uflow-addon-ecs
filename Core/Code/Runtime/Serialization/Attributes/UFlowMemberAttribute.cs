using System;
using System.Runtime.CompilerServices;

namespace UFlow.Addon.Ecs.Core.Runtime {
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class UFlowMemberAttribute : Attribute {
        internal readonly int order;

        public UFlowMemberAttribute([CallerLineNumber] int order = default) {
            this.order = order;
        }
    }
}