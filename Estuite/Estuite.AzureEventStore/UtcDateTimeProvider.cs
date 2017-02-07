using System;

namespace Estuite.AzureEventStore
{
    public class UtcDateTimeProvider : IProvideUtcDateTime
    {
        public DateTime Now => DateTime.UtcNow;
    }
}