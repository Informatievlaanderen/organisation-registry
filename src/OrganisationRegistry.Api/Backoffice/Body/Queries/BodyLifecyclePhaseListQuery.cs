namespace OrganisationRegistry.Api.Backoffice.Body.Queries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Infrastructure.Search;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Sorting;
    using SqlServer.Body;
    using SqlServer.Infrastructure;

    public class BodyLifecyclePhaseListQueryResult
    {
        public Guid BodyLifecyclePhaseId { get; }
        public Guid LifecyclePhaseTypeId { get; }
        public string LifecyclePhaseTypeName { get; }
        public DateTime? ValidFrom { get; }
        public DateTime? ValidTo { get; }
        public bool HasAdjacentGaps { get; }

        public BodyLifecyclePhaseListQueryResult(
            Guid bodyLifecyclePhaseId,
            Guid lifecyclePhaseTypeId,
            string lifecyclePhaseTypeName,
            DateTime? validFrom,
            DateTime? validTo,
            bool hasAdjacentGaps)
        {
            BodyLifecyclePhaseId = bodyLifecyclePhaseId;
            LifecyclePhaseTypeId = lifecyclePhaseTypeId;
            LifecyclePhaseTypeName = lifecyclePhaseTypeName;
            ValidFrom = validFrom;
            ValidTo = validTo;
            HasAdjacentGaps = hasAdjacentGaps;
        }
    }

    public class BodyLifecyclePhaseListQuery : Query<BodyLifecyclePhaseListItem, BodyLifecyclePhaseListItem, BodyLifecyclePhaseListQueryResult>
    {
        private readonly OrganisationRegistryContext _context;
        private readonly Guid _bodyId;

        protected override ISorting Sorting => new BodyLifecyclePhaseListSorting();

        protected override Expression<Func<BodyLifecyclePhaseListItem, BodyLifecyclePhaseListQueryResult>> Transformation =>
            x => new BodyLifecyclePhaseListQueryResult(
                x.BodyLifecyclePhaseId,
                x.LifecyclePhaseTypeId,
                x.LifecyclePhaseTypeName,
                x.ValidFrom,
                x.ValidTo,
                x.HasAdjacentGaps);

        public BodyLifecyclePhaseListQuery(OrganisationRegistryContext context, Guid bodyId)
        {
            _context = context;
            _bodyId = bodyId;
        }

        protected override IQueryable<BodyLifecyclePhaseListItem> Filter(FilteringHeader<BodyLifecyclePhaseListItem> filtering)
        {
            var bodyLifecyclePhases = _context.BodyLifecyclePhaseList
                .AsQueryable()
                .Where(x => x.BodyId == _bodyId).AsQueryable();

            if (!filtering.ShouldFilter)
                return bodyLifecyclePhases;

            //if (!filtering.Filter.Name.IsNullOrWhiteSpace())
            //    bodies = bodies.Where(x => x.Name.Contains(filtering.Filter.Name));

            return bodyLifecyclePhases;
        }

        private class BodyLifecyclePhaseListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(BodyLifecyclePhaseListItem.LifecyclePhaseTypeName),
                nameof(BodyLifecyclePhaseListItem.ValidFrom),
                nameof(BodyLifecyclePhaseListItem.ValidTo)
            };

            public SortingHeader DefaultSortingHeader { get; } =
                new SortingHeader(nameof(BodyLifecyclePhaseListItem.ValidFrom), SortOrder.Ascending);
        }
    }
}
