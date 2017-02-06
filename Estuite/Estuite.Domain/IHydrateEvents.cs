using System.Collections.Generic;

namespace Estuite.Domain
{
    public interface IHydrateEvents
    {
        void Hydrate(IEnumerable<object> events);
    }
}