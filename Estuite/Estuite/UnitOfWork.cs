using System;
using System.Collections.Generic;
using System.Linq;
using Estuite.Domain;

namespace Estuite
{
    public class UnitOfWork : IRegisterAggregates, IHydrateAggregates, ICommitAggregates
    {
        private readonly Dictionary<StreamId, IFlushEvents> _aggregates;
        private readonly BucketId _bucketId;
        private readonly ICreateStreamIdentities _streamIdentities;

        public UnitOfWork(BucketId bucketId)
        {
            if (bucketId == null) throw new ArgumentNullException(nameof(bucketId));
            _bucketId = bucketId;
            _aggregates = new Dictionary<StreamId, IFlushEvents>(StreamIdEqualityComparer.Instance);
            _streamIdentities = this as ICreateStreamIdentities ?? new DefaultStreamIdentityFactory();
        }

        public void Commit()
        {
            var eventsMap = _aggregates.ToDictionary(events => events.Key, events => events.Value.Flush());
            switch (eventsMap.Keys.Count)
            {
                case 0:
                    return;
                case 1:
                    break;
                default:
                    var ids = string.Join(", ", eventsMap.Keys);
                    string message = $"Can't commit multiple streams. Stream ids {ids}";
                    throw new InvalidOperationException(message);
            }

            throw new NotImplementedException();
        }

        public void Hydrate<TId>(TId id, IHydrateEvents events)
        {
            events.Hydrate(new object[0]);
            throw new NotImplementedException();
        }

        public void Hydrate(ICanBeHydrated aggregate)
        {
            aggregate.HydrateWith(this);
        }

        public void Register(ICanBeRegistered aggregate)
        {
            aggregate.RegisterWith(this);
        }

        public void Register<TId>(TId id, IFlushEvents aggregate)
        {
            var streamId = _streamIdentities.Create(_bucketId, id, aggregate);
            _aggregates.Add(streamId, aggregate);
        }
    }
}