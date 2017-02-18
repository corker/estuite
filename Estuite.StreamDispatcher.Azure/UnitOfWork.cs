using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Estuite.StreamDispatcher.Azure
{
    public class UnitOfWork : IConfirmEventsDispatched, IPullEventsForDispatching<EventRecord>
    {
        private static readonly EventRecordEqualityComparer Comparer;
        private readonly object _cachedRecordsLock;
        private readonly IDeleteEventRecords _deleteEventRecords;
        private readonly IReadEventRecords _readEventRecords;
        private HashSet<EventRecord> _cachedRecords;
        private string _partitionKey;

        static UnitOfWork()
        {
            Comparer = new EventRecordEqualityComparer();
        }

        public UnitOfWork(IReadEventRecords readEventRecords, IDeleteEventRecords deleteEventRecords)
        {
            _cachedRecordsLock = new object();
            _cachedRecords = new HashSet<EventRecord>(Comparer);
            _readEventRecords = readEventRecords;
            _deleteEventRecords = deleteEventRecords;
        }

        public async Task Commit(CancellationToken token = new CancellationToken())
        {
            HashSet<EventRecord> recordsToDelete;
            lock (_cachedRecordsLock)
            {
                if (_partitionKey == null) return;
                recordsToDelete = _cachedRecords;
                _partitionKey = null;
                _cachedRecords = new HashSet<EventRecord>(Comparer);
            }
            if (recordsToDelete.Any()) await _deleteEventRecords.Delete(recordsToDelete, token);
        }

        public async Task<List<EventRecord>> Pull(StreamId id, CancellationToken token = new CancellationToken())
        {
            if (_partitionKey == null) _partitionKey = id.Value;
            if (_partitionKey != id.Value) throw new ArgumentOutOfRangeException(nameof(id));
            var records = await _readEventRecords.Read(_partitionKey, token);
            var newRecords = records.Where(x =>
            {
                if (_cachedRecords.Contains(x)) return false;
                _cachedRecords.Add(x);
                return true;
            }).ToList();
            return newRecords;
        }
    }
}