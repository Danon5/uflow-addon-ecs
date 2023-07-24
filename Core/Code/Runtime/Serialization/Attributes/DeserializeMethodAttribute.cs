using System;

namespace UFlow.Addon.ECS.Core.Runtime {
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class DeserializeMethodAttribute : Attribute { }
}