using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Estuite.StreamStore;

namespace Estuite.StreamDispatcher.Azure
{
    public class DispatchEventRecordQueue : IConfirmEventsDispatched, IPullEventsForDispatching<DispatchEventRecordTableEntity>
    {
        private static readonly DispatchEventRecordEqualityComparer Comparer;
        private static readonly List<DispatchEventRecordTableEntity> Empty = new List<DispatchEventRecordTableEntity>();

        private readonly object _cachedRecordsLock;
        private readonly IDeleteEventRecords _deleteEventRecords;
        private readonly IReadEventRecords _readEventRecords;
        private HashSet<DispatchEventRecordTableEntity> _cachedRecords;
        private StreamId _streamId;

        static DispatchEventRecordQueue()
        {
            Comparer = new DispatchEventRecordEqualityComparer();
        }

        public DispatchEventRecordQueue(IReadEventRecords readEventRecords, IDeleteEventRecords deleteEventRecords)
        {
            _cachedRecordsLock = new object();
            _cachedRecords = new HashSet<DispatchEventRecordTableEntity>(Comparer);
            _readEventRecords = readEventRecords;
            _deleteEventRecords = deleteEventRecords;
        }

        public async Task Confirm(CancellationToken token = new CancellationToken())
        {
            HashSet<DispatchEventRecordTableEntity> cachedRecords;
            lock (_cachedRecordsLock)
            {
                if (_streamId == null) return;
                cachedRecords = _cachedRecords;
                _streamId = null;
                _cachedRecords = new HashSet<DispatchEventRecordTableEntity>(Comparer);
            }
            if (cachedRecords.Any()) await _deleteEventRecords.Delete(cachedRecords, token);
        }

        public async Task<List<DispatchEventRecordTableEntity>> Pull(StreamId streamId, CancellationToken token)
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
    }
}