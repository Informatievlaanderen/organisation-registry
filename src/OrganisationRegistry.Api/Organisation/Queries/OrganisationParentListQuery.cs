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

    public class OrganisationParentListQueryResult
    {
        public Guid OrganisationOrganisationParentId { get; }
        public Guid ParentOrganisationId { get; }
        public string ParentOrganisationName { get; }
        public DateTime? ValidFrom { get; }
        public DateTime? ValidTo { get; }

        public OrganisationParentListQueryResult(
            Guid organisationOrganisationParentId,
            Guid parentOrganisationId,
            string parentOrganisationName,
            DateTime? validFrom,
            DateTime? validTo)
        {
            OrganisationOrganisationParentId = organisationOrganisationParentId;
            ParentOrganisationId = parentOrganisationId;
            ParentOrganisationName = parentOrganisationName;
            ValidFrom = validFrom;
            ValidTo = validTo;
        }
    }

    public class OrganisationParentListQuery : Query<OrganisationParentListItem, OrganisationParentListItem, OrganisationParentListQueryResult>
    {
        private readonly OrganisationRegistryContext _context;
        private readonly Guid _organisationId;

        protected override ISorting Sorting => new OrganisationParentListSorting();

        protected override Expression<Func<OrganisationParentListItem, OrganisationParentListQueryResult>> Transformation =>
            x => new OrganisationParentListQueryResult(
                x.OrganisationOrganisationParentId,
                x.ParentOrganisationId,
                x.ParentOrganisationName,
                x.ValidFrom,
                x.ValidTo);

        public OrganisationParentListQuery(OrganisationRegistryContext context, Guid organisationId)
        {
            _context = context;
            _organisationId = organisationId;
        }

        protected override IQueryable<OrganisationParentListItem> Filter(FilteringHeader<OrganisationParentListItem> filtering)
        {
            var organisationParents = _context.OrganisationParentList
                .AsQueryable()
                .Where(x => x.OrganisationId == _organisationId).AsQueryable();

            if (!filtering.ShouldFilter)
                return organisationParents;

            //if (!filtering.Filter.Name.IsNullOrWhiteSpace())
            //    organisations = organisations.Where(x => x.Name.Contains(filtering.Filter.Name));

            return organisationParents;
        }

        private class OrganisationParentListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(OrganisationParentListItem.ParentOrganisationName),
                nameof(OrganisationFunctionListItem.ValidFrom),
                nameof(OrganisationFunctionListItem.ValidTo)
            };

            public SortingHeader DefaultSortingHeader { get; } =
                new SortingHeader(nameof(OrganisationParentListItem.ParentOrganisationName), SortOrder.Ascending);
        }
    }
}
