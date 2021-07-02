namespace OrganisationRegistry.Api.RegulationType.Queries
{
    using System.Collections.Generic;
    using System.Linq;
    using Infrastructure.Helpers;
    using Infrastructure.Search;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Sorting;
    using SqlServer.Infrastructure;
    using SqlServer.RegulationType;
    using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;

    public class RegulationTypeListQuery: Query<RegulationTypeListItem>
    {
        private readonly OrganisationRegistryContext _context;

        protected override ISorting Sorting => new RegulationTypeListSorting();

        public RegulationTypeListQuery(OrganisationRegistryContext context)
        {
            _context = context;
        }

        protected override IQueryable<RegulationTypeListItem> Filter(FilteringHeader<RegulationTypeListItem> filtering)
        {
            var regulationTypes = _context.RegulationTypeList.AsQueryable();

            if (!filtering.ShouldFilter)
                return regulationTypes;

            if (!filtering.Filter.Name.IsNullOrWhiteSpace())
                regulationTypes = regulationTypes.Where(x => x.Name.Contains(filtering.Filter.Name));

            return regulationTypes;
        }

        private class RegulationTypeListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(RegulationTypeListItem.Name)
            };

            public SortingHeader DefaultSortingHeader { get; } =
                new SortingHeader(nameof(RegulationTypeListItem.Name), SortOrder.Ascending);
        }
    }
}
