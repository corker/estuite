using System;
using Estuite.Domain;

namespace Estuite.StreamStore
{
    public class UtcDateTimeProvider : IProvideUtcDateTime
    {
        public DateTime Now => DateTime.UtcNow;
    }
}