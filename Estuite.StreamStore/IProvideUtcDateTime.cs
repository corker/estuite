using System;

namespace Estuite.StreamStore
{
    public interface IProvideUtcDateTime
    {
        DateTime Now { get; }
    }
}