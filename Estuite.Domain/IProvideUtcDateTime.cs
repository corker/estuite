using System;

namespace Estuite.Domain
{
    public interface IProvideUtcDateTime
    {
        DateTime Now { get; }
    }
}