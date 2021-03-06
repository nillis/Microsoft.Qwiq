using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq.Linq.Tests
{
    class InstrumentedMockWorkItemStore : IWorkItemStore
    {
        private readonly IWorkItemStore _innerWorkItemStore;

        public InstrumentedMockWorkItemStore(IWorkItemStore innerWorkItemStore)
        {
            _innerWorkItemStore = innerWorkItemStore;
        }

        public void Dispose()
        {
            _innerWorkItemStore.Dispose();
        }

        public IEnumerable<IWorkItem> Query(string wiql, bool dayPrecision = false)
        {
            QueryStringCallCount += 1;
            return _innerWorkItemStore.Query(wiql, dayPrecision);
        }
        public int QueryStringCallCount { get; private set; }

        public IEnumerable<IWorkItemLinkInfo> QueryLinks(string wiql, bool dayPrecision = false)
        {
            QueryLinksCallCount += 1;
            return _innerWorkItemStore.QueryLinks(wiql, dayPrecision);
        }
        public int QueryLinksCallCount { get; private set; }

        public IEnumerable<IWorkItem> Query(IEnumerable<int> ids, DateTime? asOf = null)
        {
            QueryIdsCallCount += 1;
            return _innerWorkItemStore.Query(ids, asOf);
        }
        public int QueryIdsCallCount { get; private set; }

        public IWorkItem Query(int id, DateTime? asOf = null)
        {
            QueryIdCallCount += 1;
            return _innerWorkItemStore.Query(id, asOf);
        }
        public int QueryIdCallCount { get; private set; }

        public int QueryCallCount
        {
            get { return QueryIdCallCount + QueryIdsCallCount + QueryLinksCallCount + QueryStringCallCount; }
        }

        public ITfsTeamProjectCollection TeamProjectCollection
        {
            get
            {
                TeamProjectCollectionCallCount += 1;
                return _innerWorkItemStore.TeamProjectCollection;
            }
        }
        public int TeamProjectCollectionCallCount { get; private set; }

        public IEnumerable<IProject> Projects
        {
            get
            {
                ProjectsCallCount += 1;
                return _innerWorkItemStore.Projects;
            }
        }
        public int ProjectsCallCount { get; private set; }

        public IEnumerable<IWorkItemLinkType> WorkItemLinkTypes
        {
            get
            {
                WorkItemLinkTypesCallCount += 1;
                return _innerWorkItemStore.WorkItemLinkTypes;
            }
        }

        public TimeZone TimeZone => _innerWorkItemStore.TimeZone;

        public int WorkItemLinkTypesCallCount { get; private set; }
    }
}

