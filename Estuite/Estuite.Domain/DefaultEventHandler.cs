using System;

namespace Estuite.Domain
{
    public class DefaultEventHandler : IHandleEvents
    {
        public void Handle(object aggregate, object @event)
        {
            throw new NotImplementedException();
        }
    }
}