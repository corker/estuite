using System;
using Estuite.Domain;

namespace Estuite
{
    public class UtcDateTimeProvider : IProvideUtcDateTime
    {
        public DateTime Now => DateTime.UtcNow;
    }
}