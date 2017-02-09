using System;

namespace Estuite.Domain
{
    public class DefaultEventFactory : ICreateEvents
    {
        public TEvent Create<TEvent>()
        {
            return Activator.CreateInstance<TEvent>();
        }
    }
}