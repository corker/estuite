using System;

namespace Estuite
{
    public interface IProvideUtcDateTime
    {
        DateTime Now { get; }
    }
}