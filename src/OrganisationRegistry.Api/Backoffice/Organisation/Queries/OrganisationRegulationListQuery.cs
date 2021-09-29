namespace OrganisationRegistry.Api.Backoffice.Organisation.Queries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Infrastructure.Search;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Sorting;
    using SqlServer.Infrastructure;
    using SqlServer.Organisation;

    public class OrganisationRegulationListQueryResult
    {
        public Guid OrganisationRegulationId { get; set; }
        public string? RegulationTypeName { get; set; }
        public DateTime? Date { get; set; }
        public string? Link { get; set; }
        public string? Description { get; set; }
        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }

        public bool IsActive { get; }

        public OrganisationRegulationListQueryResult(
            Guid organisationRegulationId,
            string? regulationTypeName,
            DateTime? date,
            string? link,
            string? description,
            DateTime? validFrom,
            DateTime? validTo)
        {
            OrganisationRegulationId = organisationRegulationId;
            RegulationTypeName = regulationTypeName;
            Date = date;
            Link = link;
            ValidFrom = validFrom;
            ValidTo = validTo;
            Description = description;

            IsActive = new Period(new ValidFrom(validFrom), new ValidTo(validTo)).OverlapsWith(DateTime.Today);
        }
    }

    public class OrganisationRegulationListQuery : Query<OrganisationRegulationListItem, OrganisationRegulationListItemFilter, OrganisationRegulationListQueryResult>
    {
        private readonly OrganisationRegistryContext _context;
        private readonly Guid _organisationId;

        protected override ISorting Sorting => new OrganisationRegulationListSorting();

        protected override Expression<Func<OrganisationRegulationListItem, OrganisationRegulationListQueryResult>> Transformation =>
            x => new OrganisationRegulationListQueryResult(
                x.OrganisationRegulationId,
                x.RegulationTypeName,
                x.Date,
                x.Link,
                x.Description,
                x.ValidFrom,
                x.ValidTo);

        public OrganisationRegulationListQuery(OrganisationRegistryContext context, Guid organisationId)
        {
            _context = context;
            _organisationId = organisationId;
        }

        protected override IQueryable<OrganisationRegulationListItem> Filter(FilteringHeader<OrganisationRegulationListItemFilter> filtering)
        {
            var organisationRegulations = _context.OrganisationRegulationList
                .AsQueryable()
                .Where(x => x.OrganisationId == _organisationId).AsQueryable();

            if (!filtering.ShouldFilter)
                return organisationRegulations;

            if (filtering.Filter.ActiveOnly)
                organisationRegulations = organisationRegulations.Where(x =>
                    (!x.ValidFrom.HasValue || x.ValidFrom <= DateTime.Today) &&
                    (!x.ValidTo.HasValue || x.ValidTo >= DateTime.Today));

            return organisationRegulations;
        }

        private class OrganisationRegulationListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(OrganisationRegulationListItem.RegulationTypeName),
                nameof(OrganisationRegulationListItem.Link),
                nameof(OrganisationRegulationListItem.ValidFrom),
                nameof(OrganisationRegulationListItem.ValidTo)
            };

            public SortingHeader DefaultSortingHeader { get; } =
                new SortingHeader(nameof(OrganisationRegulationListItem.RegulationTypeName), SortOrder.Ascending);
        }
    }

    public class OrganisationRegulationListItemFilter
    {
        public bool ActiveOnly { get; set; }
    }
}
