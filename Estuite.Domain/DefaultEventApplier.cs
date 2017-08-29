using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Estuite.Domain
{
    public class DefaultEventApplier : IApplyEvents
    {
        private static readonly MethodInfo GenericMethod;
        private static readonly Dictionary<Type, object> CachedEventAppliers;
        private static readonly object EventAppliersLock;

        static DefaultEventApplier()
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            GenericMethod = typeof(DefaultEventApplier).GetMethod("Apply", flags);
            CachedEventAppliers = new Dictionary<Type, object>();
            EventAppliersLock = new object();
        }

        public void Apply(object aggregate, object @event)
        {
            var aggregateType = aggregate.GetType();
            var eventType = @event.GetType();
            var genericMethod = GenericMethod.MakeGenericMethod(aggregateType, eventType);
            genericMethod.Invoke(this, new[] {aggregate, @event});
        }

        private void Apply<TAggregate, TEvent>(TAggregate aggregate, TEvent @event)
        {
            var applier = GetOrCreateApplier<TAggregate>();
            applier.Apply(aggregate, @event);
        }

        private static IApplyEvents<TAggregate> GetOrCreateApplier<TAggregate>()
        {
            lock (EventAppliersLock)
            {
                var isCached = CachedEventAppliers.TryGetValue(typeof(TAggregate), out var applier);
                if (isCached) return (IApplyEvents<TAggregate>) applier;
                applier = new DefaultEventApplier<TAggregate>();
                CachedEventAppliers.Add(typeof(TAggregate), applier);
                return (IApplyEvents<TAggregate>) applier;
            }
        }
    }

    public class DefaultEventApplier<TAggregate> : IApplyEvents<TAggregate>
    {
        private readonly Dictionary<Type, MethodInfo> _appliers;

        public DefaultEventApplier()
        {
            var appliers = typeof(TAggregate)
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(x => x.Name == "Handle")
                .Select(x => new {MethodInfo = x, Parameters = x.GetParameters()})
                .Where(x => x.Parameters.Length == 1);

            _appliers = appliers.ToDictionary(x => x.Parameters[0].ParameterType, x => x.MethodInfo);
        }

        public void Apply<TEvent>(TAggregate aggregate, TEvent @event)
        {
            if (_appliers.TryGetValue(typeof(TEvent), out var methodInfo))
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