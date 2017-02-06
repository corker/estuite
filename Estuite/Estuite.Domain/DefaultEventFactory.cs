using System;

namespace Estuite
{
    public class DefaultEventFactory : ICreateEvents
    {
        public TEvent Create<TEvent>()
        {
            return Activator.CreateInstance<TEvent>();
        }
    }
}