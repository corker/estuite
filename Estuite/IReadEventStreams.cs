using Estuite.Domain;

namespace Estuite
{
    public interface IReadEventStreams
    {
        void Read(StreamId streamId, IHydrateEvents events);
    }
}