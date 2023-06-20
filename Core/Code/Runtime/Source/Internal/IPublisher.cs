using System;

namespace UFlow.Core.Runtime {
    internal interface IPublisher {
        IDisposable Subscribe<T>(PublishedEventHandler<T> action);
        void Publish<T>(in T @event);
    }
}