using System;
using System.Collections.Generic;

namespace UFlow.Addon.ECS.Core.Runtime {
    public static class DisposableExtensions {
        public static IDisposable MergeIntoGroup(this IEnumerable<IDisposable> disposables) => new EcsDisposableGroup(disposables);
    }
}