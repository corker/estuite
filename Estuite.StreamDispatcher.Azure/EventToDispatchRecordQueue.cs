using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Estuite.StreamStore;
using Estuite.StreamStore.Azure;

namespace Estuite.StreamDispatcher.Azure
{
    public class EventToDispatchRecordQueue : IConfirmDispatchedEvents, IPullEventsForDispatching
    {
        private static readonly EventToDispatchRecordEqualityComparer Comparer;

        private static readonly List<EventToDispatchRecordTableEntity> Empty =
            new List<EventToDispatchRecordTableEntity>();

        private readonly object _cachedRecordsLock;
        private readonly IDeleteEventToDispatchRecords _deleteEventRecords;
        private readonly IReadEventToDispatchRecords _readEventRecords;
        private HashSet<EventToDispatchRecordTableEntity> _cachedRecords;
        private StreamId _streamId;

        static EventToDispatchRecordQueue()
        {
            Comparer = new EventToDispatchRecordEqualityComparer();
        }

        public EventToDispatchRecordQueue(IReadEventToDispatchRecords readEventRecords,
            IDeleteEventToDispatchRecords deleteEventRecords)
        {
            _cachedRecordsLock = new object();
            _cachedRecords = new HashSet<EventToDispatchRecordTableEntity>(Comparer);
            _readEventRecords = readEventRecords;
            _deleteEventRecords = deleteEventRecords;
        }

        public async Task Confirm(CancellationToken token = new CancellationToken())
        {
            HashSet<EventToDispatchRecordTableEntity> cachedRecords;
            lock (_cachedRecordsLock)
            {
                if (_streamId == null) return;
                cachedRecords = _cachedRecords;
                _streamId = null;
                _cachedRecords = new HashSet<EventToDispatchRecordTableEntity>(Comparer);
            }
            if (cachedRecords.Any()) await _deleteEventRecords.Delete(cachedRecords, token);
        }

        public async Task<List<EventToDispatchRecordTableEntity>> Pull(StreamId streamId, CancellationToken token)
        {
            if (streamId == null) throw new ArgumentNullException(nameof(streamId));
            if (_streamId == null) _streamId = streamId;
            if (_streamId != streamId) throw new ArgumentOutOfRangeException(nameof(streamId));
            var records = await _readEventRecords.Read(_streamId, token);
            var newRecords = records.Where(x =>
            {
                if (_cachedRecords.Contains(x)) return false;
                _cachedRecords.Add(x);
                return true;
            }).ToList();
            return newRecords.Any() ? newRecords : Empty;
        }

        private sealed class EventToDispatchRecordEqualityComparer : IEqualityComparer<EventToDispatchRecordTableEntity>
        {
            public bool Equals(EventToDispatchRecordTableEntity x, EventToDispatchRecordTableEntity y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return string.Equals(x.PartitionKey, y.PartitionKey) && string.Equals(x.RowKey, y.RowKey);
            }

            public int GetHashCode(EventToDispatchRecordTableEntity obj)
            {
                unchecked
                {
                    return (obj.PartitionKey.GetHashCode()*397) ^ obj.RowKey.GetHashCode();
                }
            }
        }
    }
}