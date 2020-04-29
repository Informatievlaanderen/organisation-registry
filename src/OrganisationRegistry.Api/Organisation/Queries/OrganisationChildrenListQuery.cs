namespace OrganisationRegistry.Api.Organisation.Queries
{
    using System.Collections.Generic;
    using System.Linq;
    using Infrastructure.Search;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Sorting;
    using SqlServer.Infrastructure;
    using System;
    using System.Linq.Expressions;
    using SqlServer.Organisation;

    public class OrganisationChildListQueryResult
    {
        public Guid Id { get; }
        public string Name { get; }
        public string OvoNumber { get; }

        public OrganisationChildListQueryResult(Guid id, string ovoNumber, string name)
        {
            Id = id;
            OvoNumber = ovoNumber;
            Name = name;
        }
    }

    public class OrganisationChildListQuery : Query<OrganisationChildListItem, OrganisationChildListItem, OrganisationChildListQueryResult>
    {
        private readonly OrganisationRegistryContext _context;
        private readonly Guid _organisationId;

        protected override ISorting Sorting => new OrganisationChildListSorting();

        protected override Expression<Func<OrganisationChildListItem, OrganisationChildListQueryResult>> Transformation =>
            x => new OrganisationChildListQueryResult(
                x.Id,
                x.OvoNumber,
                x.Name);

        public OrganisationChildListQuery(OrganisationRegistryContext context, Guid organisationId)
        {
            _context = context;
            _organisationId = organisationId;
        }

        protected override IQueryable<OrganisationChildListItem> Filter(FilteringHeader<OrganisationChildListItem> filtering)
        {
            var organisationChildren = _context.OrganisationChildrenList
                .AsQueryable()
                .Where(x => x.ParentOrganisationId == _organisationId).AsQueryable();

            // Only possible to get active children
            organisationChildren = organisationChildren
                .Where(x =>
                    (!x.ValidFrom.HasValue || x.ValidFrom <= DateTime.Today) &&
                    (!x.ValidTo.HasValue || x.ValidTo >= DateTime.Today))
                .Where(x =>
                    (!x.OrganisationValidFrom.HasValue || x.OrganisationValidFrom <= DateTime.Today) &&
                    (!x.OrganisationValidTo.HasValue || x.OrganisationValidTo >= DateTime.Today));

            if (!filtering.ShouldFilter)
                return organisationChildren;

            //if (!filtering.Filter.Name.IsNullOrWhiteSpace())
            //    organisations = organisations.Where(x => x.Name.Contains(filtering.Filter.Name));

            return organisationChildren;
        }

        private class OrganisationChildListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(OrganisationChildListItem.OvoNumber),
                nameof(OrganisationChildListItem.Name)
            };

            public SortingHeader DefaultSortingHeader { get; } =
                new SortingHeader(nameof(OrganisationChildListItem.Name), SortOrder.Ascending);
        }
    }
}
