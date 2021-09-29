namespace OrganisationRegistry.Api.Backoffice.Parameters.OrganisationRelationType.Queries
{
    using System.Collections.Generic;
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
    using Infrastructure.Search;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Sorting;
    using SqlServer.Infrastructure;
    using SqlServer.OrganisationRelationType;

    public class OrganisationRelationTypeListQuery : Query<OrganisationRelationTypeListItem>
    {
        private readonly OrganisationRegistryContext _context;

        protected override ISorting Sorting => new OrganisationRelationTypeListSorting();

        public OrganisationRelationTypeListQuery(OrganisationRegistryContext context)
        {
            _context = context;
        }

        protected override IQueryable<OrganisationRelationTypeListItem> Filter(FilteringHeader<OrganisationRelationTypeListItem> filtering)
        {
            var organisationRelationTypes = _context.OrganisationRelationTypeList.AsQueryable();

            if (!filtering.ShouldFilter)
                return organisationRelationTypes;

            if (!filtering.Filter.Name.IsNullOrWhiteSpace())
                organisationRelationTypes = organisationRelationTypes.Where(x =>
                    x.Name.Contains(filtering.Filter.Name) ||
                    x.InverseName.Contains(filtering.Filter.Name));

            return organisationRelationTypes;
        }

        private class OrganisationRelationTypeListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(OrganisationRelationTypeListItem.Name)
            };

            public SortingHeader DefaultSortingHeader { get; } =
                new SortingHeader(nameof(OrganisationRelationTypeListItem.Name), SortOrder.Ascending);
        }
    }
}
