namespace OrganisationRegistry.Api.Backoffice.Parameters.Capacity.Queries
{
    using System.Collections.Generic;
    using System.Linq;
    using Infrastructure.Search;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Sorting;
    using OrganisationRegistry.Infrastructure;
    using SqlServer.Capacity;
    using SqlServer.Infrastructure;

    public class CapacityListQuery: Query<CapacityListItem, CapacityListQuery.CapacityListFilter>
    {
        public class CapacityListFilter
        {
            public string? Name { get; set; }
            public bool ShowAll { get; set; } = false;
        }

        private readonly OrganisationRegistryContext _context;

        protected override ISorting Sorting => new CapacityListSorting();

        public CapacityListQuery(OrganisationRegistryContext context)
        {
            _context = context;
        }

        protected override IQueryable<CapacityListItem> Filter(FilteringHeader<CapacityListFilter> filtering)
        {
            var capacities = _context.CapacityList.AsQueryable();

            if (filtering.Filter is not { } filter)
                return capacities.Where(x => !x.IsRemoved);

            if (!filter.ShowAll)
                capacities = capacities.Where(x => !x.IsRemoved);

            if (filter.Name is { } name && name.IsNotEmptyOrWhiteSpace())
                capacities = capacities.Where(x => x.Name.Contains(name));

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
