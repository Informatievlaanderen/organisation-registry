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

    public class BodyFormalFrameworkListQueryResult
    {
        public Guid BodyFormalFrameworkId { get; }
        public Guid FormalFrameworkId { get; }
        public string FormalFrameworkName { get; }
        public DateTime? ValidFrom { get; }
        public DateTime? ValidTo { get; }

        public bool IsActive { get; }

        public BodyFormalFrameworkListQueryResult(
            Guid bodyFormalFrameworkId,
            Guid formalFrameworkId, string formalFrameworkName,
            DateTime? validFrom, DateTime? validTo)
        {
            BodyFormalFrameworkId = bodyFormalFrameworkId;
            FormalFrameworkId = formalFrameworkId;
            FormalFrameworkName = formalFrameworkName;
            ValidFrom = validFrom;
            ValidTo = validTo;

            IsActive = new Period(new ValidFrom(validFrom), new ValidTo(validTo)).OverlapsWith(DateTime.Today);
        }
    }

    public class BodyFormalFrameworkListQuery : Query<BodyFormalFrameworkListItem, BodyFormalFrameworkListItemFilter, BodyFormalFrameworkListQueryResult>
    {
        private readonly OrganisationRegistryContext _context;
        private readonly Guid _bodyId;

        protected override ISorting Sorting => new BodyFormalFrameworkListSorting();

        protected override Expression<Func<BodyFormalFrameworkListItem, BodyFormalFrameworkListQueryResult>> Transformation =>
            x => new BodyFormalFrameworkListQueryResult(
                x.BodyFormalFrameworkId,
                x.FormalFrameworkId,
                x.FormalFrameworkName,
                x.ValidFrom,
                x.ValidTo);

        public BodyFormalFrameworkListQuery(OrganisationRegistryContext context, Guid bodyId)
        {
            _context = context;
            _bodyId = bodyId;
        }

        protected override IQueryable<BodyFormalFrameworkListItem> Filter(FilteringHeader<BodyFormalFrameworkListItemFilter> filtering)
        {
            var bodyFormalFrameworks = _context.BodyFormalFrameworkList
                .AsQueryable()
                .Where(x => x.BodyId == _bodyId).AsQueryable();

            if (filtering.Filter is not { } filter)
                return bodyFormalFrameworks;

            if (filter.ActiveOnly)
                bodyFormalFrameworks = bodyFormalFrameworks.Where(x =>
                    (!x.ValidFrom.HasValue || x.ValidFrom <= DateTime.Today) &&
                    (!x.ValidTo.HasValue || x.ValidTo >= DateTime.Today));

            return bodyFormalFrameworks;
        }

        private class BodyFormalFrameworkListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(BodyFormalFrameworkListItem.FormalFrameworkName),
                nameof(BodyFormalFrameworkListItem.ValidFrom),
                nameof(BodyFormalFrameworkListItem.ValidTo)
            };

            public SortingHeader DefaultSortingHeader { get; } =
                new SortingHeader(nameof(BodyFormalFrameworkListItem.FormalFrameworkName), SortOrder.Ascending);
        }
    }

    public class BodyFormalFrameworkListItemFilter
    {
        public bool ActiveOnly { get; set; }
    }
}
