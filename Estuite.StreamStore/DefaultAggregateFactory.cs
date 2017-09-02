using System;
using System.Reflection;
using Estuite.Domain;

namespace Estuite.StreamStore
{
    public class DefaultAggregateFactory : ICreateAggregates
    {
        private const BindingFlags Flags = BindingFlags.CreateInstance | BindingFlags.NonPublic | BindingFlags.Instance;

        public T Create<T>(object id) where T : IReceiveEvents
        {
            var args = new[] {id};
            return (T) Activator.CreateInstance(typeof(T), Flags, null, args, null);
        }
    }
}