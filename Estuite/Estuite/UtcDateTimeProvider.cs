using System;

namespace Estuite
{
    public class UtcDateTimeProvider : IProvideUtcDateTime
    {
        public DateTime Now => DateTime.UtcNow;
    }
}