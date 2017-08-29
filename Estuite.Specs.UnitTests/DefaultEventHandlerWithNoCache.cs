using System;
using System.Collections.Generic;
using System.Reflection;
using Estuite.Domain;

namespace Estuite.Specs.UnitTests
{
    public class DefaultEventHandlerWithNoCache : IApplyEvents
    {
        private static readonly MethodInfo HandleMethod;
        private static readonly Dictionary<Type, object> CachedEventHandlers;
        private static readonly object EventHandlersLock;

        static DefaultEventHandlerWithNoCache()
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            HandleMethod = typeof(DefaultEventHandlerWithNoCache).GetMethod("Handle", flags);
            CachedEventHandlers = new Dictionary<Type, object>();
            EventHandlersLock = new object();
        }

        public void Apply(object aggregate, object @event)
        {
            var aggregateType = aggregate.GetType();
            var eventType = @event.GetType();
            var genericMethod = HandleMethod.MakeGenericMethod(aggregateType, eventType);
            genericMethod.Invoke(this, new[] {aggregate, @event});
        }

        private void Handle<TAggregate, TEvent>(TAggregate aggregate, TEvent @event)
        {
            var handler = GetOrCreateHandler<TAggregate>();
            handler.Apply(aggregate, @event);
        }

        private static IApplyEvents<TAggregate> GetOrCreateHandler<TAggregate>()
        {
            lock (EventHandlersLock)
            {
                object handler;
                var isCached = CachedEventHandlers.TryGetValue(typeof(TAggregate), out handler);
                handler = new DefaultEventApplier<TAggregate>();
                if (!isCached) CachedEventHandlers.Add(typeof(TAggregate), handler);
                return (IApplyEvents<TAggregate>) handler;
            }
        }
    }
}