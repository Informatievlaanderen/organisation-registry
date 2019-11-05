namespace OrganisationRegistry.Api.Organisation.Queries
{
    using System.Collections.Generic;
    using System.Linq;
    using Infrastructure.Search;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Sorting;
    using SqlServer.Infrastructure;
    using SqlServer.Organisation;
    using System;
    using System.Linq.Expressions;

    public class OrganisationOrganisationClassificationListQueryResult
    {
        public Guid OrganisationOrganisationClassificationId { get; }
        public string OrganisationClassificationTypeName { get; }
        public string OrganisationClassificationName { get; }
        public DateTime? ValidFrom { get; }
        public DateTime? ValidTo { get; }

        public bool IsActive { get; }

        public bool IsEditable { get; }

        public OrganisationOrganisationClassificationListQueryResult(
            Guid organisationOrganisationClassificationId,
            string organisationClassificationTypeName,
            string organisationClassificationName,
            DateTime? validFrom,
            DateTime? validTo,
            bool isEditable)
        {
            OrganisationOrganisationClassificationId = organisationOrganisationClassificationId;
            OrganisationClassificationTypeName = organisationClassificationTypeName;
            OrganisationClassificationName = organisationClassificationName;
            ValidFrom = validFrom;
            ValidTo = validTo;
            IsEditable = isEditable;

            IsActive = new Period(new ValidFrom(validFrom), new ValidTo(validTo)).OverlapsWith(DateTime.Today);
        }
    }

    public class OrganisationOrganisationClassificationListQuery : Query<OrganisationOrganisationClassificationListItem, OrganisationOrganisationClassificationListItemFilter, OrganisationOrganisationClassificationListQueryResult>
    {
        private readonly OrganisationRegistryContext _context;
        private readonly Guid _organisationId;

        protected override ISorting Sorting => new OrganisationOrganisationClassificationListSorting();

        protected override Expression<Func<OrganisationOrganisationClassificationListItem, OrganisationOrganisationClassificationListQueryResult>> Transformation =>
            x => new OrganisationOrganisationClassificationListQueryResult(
                x.OrganisationOrganisationClassificationId,
                x.OrganisationClassificationTypeName,
                x.OrganisationClassificationName,
                x.ValidFrom,
                x.ValidTo,
                x.IsEditable);

        public OrganisationOrganisationClassificationListQuery(OrganisationRegistryContext context, Guid organisationId)
        {
            _context = context;
            _organisationId = organisationId;
        }

        protected override IQueryable<OrganisationOrganisationClassificationListItem> Filter(FilteringHeader<OrganisationOrganisationClassificationListItemFilter> filtering)
        {
            var organisationOrganisationClassifications = _context.OrganisationOrganisationClassificationList.Where(x => x.OrganisationId == _organisationId).AsQueryable();

            if (!filtering.ShouldFilter)
                return organisationOrganisationClassifications;

            if (filtering.Filter.ActiveOnly)
                organisationOrganisationClassifications = organisationOrganisationClassifications.Where(x =>
                    (!x.ValidFrom.HasValue || x.ValidFrom <= DateTime.Today) &&
                    (!x.ValidTo.HasValue || x.ValidTo >= DateTime.Today));

            return organisationOrganisationClassifications;
        }

        private class OrganisationOrganisationClassificationListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(OrganisationOrganisationClassificationListItem.OrganisationClassificationTypeName),
                nameof(OrganisationOrganisationClassificationListItem.OrganisationClassificationName),
                nameof(OrganisationOrganisationClassificationListItem.ValidFrom),
                nameof(OrganisationOrganisationClassificationListItem.ValidTo)
            };

            public SortingHeader DefaultSortingHeader { get; } =
                new SortingHeader(nameof(OrganisationOrganisationClassificationListItem.OrganisationClassificationTypeName), SortOrder.Ascending);
        }
    }

    public class OrganisationOrganisationClassificationListItemFilter
    {
        public bool ActiveOnly { get; set; }
    }
}
