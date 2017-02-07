using System;

namespace Estuite.AzureEventStore
{
    public interface IProvideUtcDateTime
    {
        DateTime Now { get; }
    }
}