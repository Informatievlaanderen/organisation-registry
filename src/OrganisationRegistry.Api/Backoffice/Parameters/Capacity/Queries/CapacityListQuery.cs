namespace OrganisationRegistry.Api.Backoffice.Parameters.Capacity.Queries
{
    using System.Collections.Generic;
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
    using Infrastructure.Search;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Sorting;
    using SqlServer.Capacity;
    using SqlServer.Infrastructure;

    public class CapacityListQuery: Query<CapacityListItem>
    {
        private readonly OrganisationRegistryContext _context;

        protected override ISorting Sorting => new CapacityListSorting();

        public CapacityListQuery(OrganisationRegistryContext context)
        {
            _context = context;
        }

        protected override IQueryable<CapacityListItem> Filter(FilteringHeader<CapacityListItem> filtering)
        {
            var capacities = _context.CapacityList.AsQueryable();

            if (filtering.Filter is not { } filter)
                return capacities;

            if (!filter.Name.IsNullOrWhiteSpace())
                capacities = capacities.Where(x => x.Name.Contains(filter.Name));

            return capacities;
        }

        private class CapacityListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(CapacityListItem.Name)
            };

            public SortingHeader DefaultSortingHeader { get; } =
                new SortingHeader(nameof(CapacityListItem.Name), SortOrder.Ascending);
        }
    }
}
