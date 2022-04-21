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

    public class BodyBodyClassificationListQueryResult
    {
        public Guid BodyBodyClassificationId { get; }
        public string BodyClassificationTypeName { get; }
        public string BodyClassificationName { get; }
        public DateTime? ValidFrom { get; }
        public DateTime? ValidTo { get; }

        public bool IsActive { get; }

        public BodyBodyClassificationListQueryResult(
            Guid bodyBodyClassificationId,
            string bodyClassificationTypeName,
            string bodyClassificationName,
            DateTime? validFrom,
            DateTime? validTo)
        {
            BodyBodyClassificationId = bodyBodyClassificationId;
            BodyClassificationTypeName = bodyClassificationTypeName;
            BodyClassificationName = bodyClassificationName;
            ValidFrom = validFrom;
            ValidTo = validTo;

            IsActive = new Period(new ValidFrom(validFrom), new ValidTo(validTo)).OverlapsWith(DateTime.Today);
        }
    }

    public class BodyBodyClassificationListQuery : Query<BodyBodyClassificationListItem, BodyBodyClassificationListItemFilter, BodyBodyClassificationListQueryResult>
    {
        private readonly OrganisationRegistryContext _context;
        private readonly Guid _bodyId;

        protected override ISorting Sorting => new BodyBodyClassificationListSorting();

        protected override Expression<Func<BodyBodyClassificationListItem, BodyBodyClassificationListQueryResult>> Transformation =>
            x => new BodyBodyClassificationListQueryResult(
                x.BodyBodyClassificationId,
                x.BodyClassificationTypeName,
                x.BodyClassificationName,
                x.ValidFrom,
                x.ValidTo);

        public BodyBodyClassificationListQuery(OrganisationRegistryContext context, Guid bodyId)
        {
            _context = context;
            _bodyId = bodyId;
        }

        protected override IQueryable<BodyBodyClassificationListItem> Filter(FilteringHeader<BodyBodyClassificationListItemFilter> filtering)
        {
            var bodyBodyClassifications = _context.BodyBodyClassificationList
                .AsQueryable()
                .Where(x => x.BodyId == _bodyId).AsQueryable();

            if (filtering.Filter is not { } filter)
                return bodyBodyClassifications;

            if (filter.ActiveOnly)
                bodyBodyClassifications = bodyBodyClassifications.Where(x =>
                    (!x.ValidFrom.HasValue || x.ValidFrom <= DateTime.Today) &&
                    (!x.ValidTo.HasValue || x.ValidTo >= DateTime.Today));

            return bodyBodyClassifications;
        }

        private class BodyBodyClassificationListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(BodyBodyClassificationListItem.BodyClassificationTypeName),
                nameof(BodyBodyClassificationListItem.BodyClassificationName),
                nameof(BodyBodyClassificationListItem.ValidFrom),
                nameof(BodyBodyClassificationListItem.ValidTo)
            };

            public SortingHeader DefaultSortingHeader { get; } =
                new SortingHeader(nameof(BodyBodyClassificationListItem.BodyClassificationTypeName), SortOrder.Ascending);
        }
    }

    public class BodyBodyClassificationListItemFilter
    {
        public bool ActiveOnly { get; set; }
    }
}
