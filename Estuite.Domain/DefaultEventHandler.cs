using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Estuite.Domain
{
    public class DefaultEventHandler : IHandleEvents
    {
        private static readonly MethodInfo HandleMethod;
        private static readonly Dictionary<Type, object> CachedEventHandlers;
        private static readonly object EventHandlersLock;

        static DefaultEventHandler()
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            HandleMethod = typeof(DefaultEventHandler).GetMethod("Handle", flags);
            CachedEventHandlers = new Dictionary<Type, object>();
            EventHandlersLock = new object();
        }

        public void Handle(object aggregate, object @event)
        {
            var aggregateType = aggregate.GetType();
            var eventType = @event.GetType();
            var genericMethod = HandleMethod.MakeGenericMethod(aggregateType, eventType);
            genericMethod.Invoke(this, new[] {aggregate, @event});
        }

        private void Handle<TAggregate, TEvent>(TAggregate aggregate, TEvent @event)
        {
            var handler = GetOrCreateHandler<TAggregate>();
            handler.Handle(aggregate, @event);
        }

        private static IHandleEvents<TAggregate> GetOrCreateHandler<TAggregate>()
        {
            lock (EventHandlersLock)
            {
                object handler;
                var isCached = CachedEventHandlers.TryGetValue(typeof(TAggregate), out handler);
                if (isCached) return (IHandleEvents<TAggregate>) handler;
                handler = new DefaultEventHandler<TAggregate>();
                CachedEventHandlers.Add(typeof(TAggregate), handler);
                return (IHandleEvents<TAggregate>) handler;
            }
        }
    }

    public class DefaultEventHandler<TAggregate> : IHandleEvents<TAggregate>
    {
        private readonly Dictionary<Type, MethodInfo> _handlers;

        public DefaultEventHandler()
        {
            var handlers = typeof(TAggregate)
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(x => x.Name == "Handle")
                .Select(x => new {MethodInfo = x, Parameters = x.GetParameters()})
                .Where(x => x.Parameters.Length == 1);

            _handlers = handlers.ToDictionary(x => x.Parameters[0].ParameterType, x => x.MethodInfo);
        }

        public void Handle<TEvent>(TAggregate aggregate, TEvent @event)
        {
            MethodInfo methodInfo;
            if (_handlers.TryGetValue(typeof(TEvent), out methodInfo))
            {
                methodInfo.Invoke(aggregate, new object[] {@event});
            }
            else
            {
                var message = $"Method {typeof(TAggregate).Name}.Handle({typeof(TEvent).Name}) not found.";
                throw new ArgumentOutOfRangeException(nameof(@event), message);
            }
        }
    }
}