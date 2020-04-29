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

    public class OrganisationFormalFrameworkListQueryResult
    {
        public Guid OrganisationFormalFrameworkId { get; }
        public Guid FormalFrameworkId { get; }
        public string FormalFrameworkName { get; }
        public Guid ParentOrganisationId { get; }
        public string ParentOrganisationName { get; }
        public DateTime? ValidFrom { get; }
        public DateTime? ValidTo { get; }

        public bool IsActive { get; }

        public OrganisationFormalFrameworkListQueryResult(
            Guid organisationFormalFrameworkId,
            Guid formalFrameworkId, string formalFrameworkName,
            Guid parentOrganisationId, string parentOrganisationName,
            DateTime? validFrom, DateTime? validTo)
        {
            OrganisationFormalFrameworkId = organisationFormalFrameworkId;
            FormalFrameworkId = formalFrameworkId;
            FormalFrameworkName = formalFrameworkName;
            ParentOrganisationId = parentOrganisationId;
            ParentOrganisationName = parentOrganisationName;
            ValidFrom = validFrom;
            ValidTo = validTo;

            IsActive = new Period(new ValidFrom(validFrom), new ValidTo(validTo)).OverlapsWith(DateTime.Today);
        }
    }

    public class OrganisationFormalFrameworkListQuery : Query<OrganisationFormalFrameworkListItem, OrganisationFormalFrameworkListItemFilter, OrganisationFormalFrameworkListQueryResult>
    {
        private readonly OrganisationRegistryContext _context;
        private readonly Guid _organisationId;

        protected override ISorting Sorting => new OrganisationFormalFrameworkListSorting();

        protected override Expression<Func<OrganisationFormalFrameworkListItem, OrganisationFormalFrameworkListQueryResult>> Transformation =>
            x => new OrganisationFormalFrameworkListQueryResult(
                x.OrganisationFormalFrameworkId,
                x.FormalFrameworkId,
                x.FormalFrameworkName,
                x.ParentOrganisationId,
                x.ParentOrganisationName,
                x.ValidFrom,
                x.ValidTo);

        public OrganisationFormalFrameworkListQuery(OrganisationRegistryContext context, Guid organisationId)
        {
            _context = context;
            _organisationId = organisationId;
        }

        protected override IQueryable<OrganisationFormalFrameworkListItem> Filter(FilteringHeader<OrganisationFormalFrameworkListItemFilter> filtering)
        {
            var organisationFormalFrameworks = _context.OrganisationFormalFrameworkList
                .AsQueryable()
                .Where(x => x.OrganisationId == _organisationId).AsQueryable();

            if (!filtering.ShouldFilter)
                return organisationFormalFrameworks;

            if (filtering.Filter.ActiveOnly)
                organisationFormalFrameworks = organisationFormalFrameworks.Where(x =>
                    (!x.ValidFrom.HasValue || x.ValidFrom <= DateTime.Today) &&
                    (!x.ValidTo.HasValue || x.ValidTo >= DateTime.Today));

            return organisationFormalFrameworks;
        }

        private class OrganisationFormalFrameworkListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(OrganisationFormalFrameworkListItem.ParentOrganisationName),
                nameof(OrganisationFormalFrameworkListItem.FormalFrameworkName),
                nameof(OrganisationFormalFrameworkListItem.ValidFrom),
                nameof(OrganisationFormalFrameworkListItem.ValidTo)
            };

            public SortingHeader DefaultSortingHeader { get; } =
                new SortingHeader(nameof(OrganisationFormalFrameworkListItem.ParentOrganisationName), SortOrder.Ascending);
        }
    }

    public class OrganisationFormalFrameworkListItemFilter
    {
        public bool ActiveOnly { get; set; }
    }
}
