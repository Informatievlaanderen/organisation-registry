namespace OrganisationRegistry.Api.Backoffice.Parameters.OrganisationClassification.Queries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
    using Infrastructure.Search;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Sorting;
    using SqlServer.Infrastructure;
    using SqlServer.OrganisationClassification;

    public class OrganisationClassificationListQueryResult
    {
        public Guid Id { get; }
        public bool Active { get; }
        public string Name { get; }
        public int Order { get; }
        public string ExternalKey { get; }
        public string OrganisationClassificationTypeName { get; }

        public OrganisationClassificationListQueryResult(
            Guid id,
            bool active,
            string name,
            int order,
            string externalKey,
            string organisationClassificationTypeName)
        {
            Id = id;
            Active = active;
            Name = name;
            Order = order;
            ExternalKey = externalKey;
            OrganisationClassificationTypeName = organisationClassificationTypeName;
        }
    }

    public class OrganisationClassificationListQuery: Query<OrganisationClassificationListItem, OrganisationClassificationListItemFilter, OrganisationClassificationListQueryResult>
    {
        private readonly OrganisationRegistryContext _context;

        protected override ISorting Sorting => new OrganisationClassificationListSorting();

        protected override Expression<Func<OrganisationClassificationListItem, OrganisationClassificationListQueryResult>> Transformation =>
            x => new OrganisationClassificationListQueryResult(
                x.Id,
                x.Active,
                x.Name,
                x.Order,
                x.ExternalKey,
                x.OrganisationClassificationTypeName);

        public OrganisationClassificationListQuery(OrganisationRegistryContext context)
        {
            _context = context;
        }

        protected override IQueryable<OrganisationClassificationListItem> Filter(FilteringHeader<OrganisationClassificationListItemFilter> filtering)
        {
            var organisationClassifications = _context.OrganisationClassificationList.AsQueryable();

            if (!filtering.ShouldFilter)
                return organisationClassifications;

            if (!filtering.Filter.Name.IsNullOrWhiteSpace())
                organisationClassifications = organisationClassifications.Where(x => x.Name.Contains(filtering.Filter.Name));

            // TODO: be able to filter on active

            if (!filtering.Filter.OrganisationClassificationTypeName.IsNullOrWhiteSpace())
                organisationClassifications = organisationClassifications.Where(x => x.OrganisationClassificationTypeName.Contains(filtering.Filter.OrganisationClassificationTypeName));

            if (!filtering.Filter.OrganisationClassificationTypeId.IsEmptyGuid())
                organisationClassifications = organisationClassifications.Where(x => x.OrganisationClassificationTypeId == filtering.Filter.OrganisationClassificationTypeId);

            return organisationClassifications;
        }

        private class OrganisationClassificationListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(OrganisationClassificationListItem.Name),
                nameof(OrganisationClassificationListItem.Order),
                nameof(OrganisationClassificationListItem.Active),
                nameof(OrganisationClassificationListItem.OrganisationClassificationTypeName),
            };

            public SortingHeader DefaultSortingHeader { get; } =
                new SortingHeader(nameof(OrganisationClassificationListItem.Name), SortOrder.Ascending);
        }
    }

    public class OrganisationClassificationListItemFilter
    {
        public string Name { get; set; }
        public bool Active { get; set; }
        public Guid OrganisationClassificationTypeId { get; set; }
        public string OrganisationClassificationTypeName { get; set; }
    }
}
