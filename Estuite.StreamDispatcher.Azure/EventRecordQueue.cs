using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Estuite.StreamDispatcher.Azure
{
    public class EventRecordQueue : IConfirmEventsDispatched, IPullEventsForDispatching<EventRecordTableEntity>
    {
        private static readonly EventRecordEqualityComparer Comparer;
        private static readonly List<EventRecordTableEntity> Empty = new List<EventRecordTableEntity>();

        private readonly object _cachedRecordsLock;
        private readonly IDeleteEventRecords _deleteEventRecords;
        private readonly IDeleteStreamMarkers _deleteStreamMarkers;
        private readonly IFindStreamMarkers _findStreamMarkers;
        private readonly IReadEventRecords _readEventRecords;
        private HashSet<EventRecordTableEntity> _cachedRecords;
        private string _partitionKey;
        private StreamMarkerTableEntity _streamMarker;

        static EventRecordQueue()
        {
            Comparer = new EventRecordEqualityComparer();
        }

        public EventRecordQueue(
            IReadEventRecords readEventRecords,
            IDeleteEventRecords deleteEventRecords,
            IFindStreamMarkers findStreamMarkers,
            IDeleteStreamMarkers deleteStreamMarkers)
        {
            _cachedRecordsLock = new object();
            _cachedRecords = new HashSet<EventRecordTableEntity>(Comparer);
            _readEventRecords = readEventRecords;
            _deleteEventRecords = deleteEventRecords;
            _findStreamMarkers = findStreamMarkers;
            _deleteStreamMarkers = deleteStreamMarkers;
        }

        public async Task Confirm(CancellationToken token = new CancellationToken())
        {
            HashSet<EventRecordTableEntity> cachedRecords;
            StreamMarkerTableEntity streamMarker;
            lock (_cachedRecordsLock)
            {
                if (_partitionKey == null) return;
                streamMarker = _streamMarker;
                cachedRecords = _cachedRecords;
                _partitionKey = null;
                _streamMarker = null;
                _cachedRecords = new HashSet<EventRecordTableEntity>(Comparer);
            }
            if (cachedRecords.Any()) await _deleteEventRecords.Delete(cachedRecords, token);
            if (streamMarker != null) await _deleteStreamMarkers.TryDelete(streamMarker, token);
        }

        public async Task<List<EventRecordTableEntity>> Pull(
            StreamId streamId,
            CancellationToken token = new CancellationToken())
        {
            if (streamId == null) throw new ArgumentNullException(nameof(streamId));
            if (_partitionKey == null) _partitionKey = streamId.Value;
            if (_partitionKey != streamId.Value) throw new ArgumentOutOfRangeException(nameof(streamId));
            var records = await _readEventRecords.Read(_partitionKey, token);
            var newRecords = records.Where(x =>
            {
                if (_cachedRecords.Contains(x)) return false;
                _cachedRecords.Add(x);
                return true;
            }).ToList();
            if (!newRecords.Any()) return Empty;
            if (_streamMarker == null) _streamMarker = await _findStreamMarkers.Find(streamId, token);
            return newRecords;
        }
    }
}