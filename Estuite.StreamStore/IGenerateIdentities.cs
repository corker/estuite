using System;

namespace Estuite.StreamStore
{
    public interface IGenerateIdentities
    {
        Guid Generate();
    }
}