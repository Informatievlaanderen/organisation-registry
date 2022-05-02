namespace OrganisationRegistry.Api.Backoffice.Parameters.BodyClassification.Queries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
    using Infrastructure.Search;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Sorting;
    using OrganisationRegistry.Infrastructure;
    using SqlServer.BodyClassification;
    using SqlServer.Infrastructure;

    public class BodyClassificationListQueryResult
    {
        public Guid Id { get; }
        public bool Active { get; }
        public string Name { get; }
        public int Order { get; }
        public string BodyClassificationTypeName { get; }

        public BodyClassificationListQueryResult(
            Guid id,
            bool active,
            string name,
            int order,
            string bodyClassificationTypeName)
        {
            Id = id;
            Active = active;
            Name = name;
            Order = order;
            BodyClassificationTypeName = bodyClassificationTypeName;
        }
    }

    public class BodyClassificationListQuery : Query<BodyClassificationListItem, BodyClassificationListItemFilter, BodyClassificationListQueryResult>
    {
        private readonly OrganisationRegistryContext _context;

        protected override ISorting Sorting => new BodyClassificationListSorting();

        protected override Expression<Func<BodyClassificationListItem, BodyClassificationListQueryResult>> Transformation =>
            x => new BodyClassificationListQueryResult(
                x.Id,
                x.Active,
                x.Name,
                x.Order,
                x.BodyClassificationTypeName);

        public BodyClassificationListQuery(OrganisationRegistryContext context)
        {
            _context = context;
        }

        protected override IQueryable<BodyClassificationListItem> Filter(FilteringHeader<BodyClassificationListItemFilter> filtering)
        {
            var bodyClassifications = _context.BodyClassificationList.AsQueryable();

            if (filtering.Filter is not { } filter)
                return bodyClassifications;

            if (filter.Name is { } name && name.IsNotEmptyOrWhiteSpace())
                bodyClassifications = bodyClassifications.Where(x => x.Name.Contains(name));

            if (filter.BodyClassificationTypeName is { } bodyClassificationTypeName && bodyClassificationTypeName.IsNotEmptyOrWhiteSpace())
                bodyClassifications = bodyClassifications.Where(x => x.BodyClassificationTypeName.Contains(bodyClassificationTypeName));

            if (!filter.BodyClassificationTypeId.IsEmptyGuid())
                bodyClassifications = bodyClassifications.Where(x => x.BodyClassificationTypeId == filter.BodyClassificationTypeId);

            return bodyClassifications;
        }

        private class BodyClassificationListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(BodyClassificationListItem.Name),
                nameof(BodyClassificationListItem.Order),
                nameof(BodyClassificationListItem.Active),
                nameof(BodyClassificationListItem.BodyClassificationTypeName),
            };

            public SortingHeader DefaultSortingHeader { get; } =
                new SortingHeader(nameof(BodyClassificationListItem.Name), SortOrder.Ascending);
        }
    }

    public class BodyClassificationListItemFilter
    {
        public string? Name { get; set; }
        public bool Active { get; set; }
        public Guid BodyClassificationTypeId { get; set; }
        public string? BodyClassificationTypeName { get; set; }
    }
}
