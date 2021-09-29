namespace OrganisationRegistry.Api.Backoffice.Parameters.SeatType.Queries
{
    using System.Collections.Generic;
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
    using Infrastructure.Search;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Sorting;
    using SqlServer.Infrastructure;
    using SqlServer.SeatType;

    public class SeatTypeListQuery: Query<SeatTypeListItem>
    {
        private readonly OrganisationRegistryContext _context;

        protected override ISorting Sorting => new SeatTypeListSorting();

        public SeatTypeListQuery(OrganisationRegistryContext context)
        {
            _context = context;
        }

        protected override IQueryable<SeatTypeListItem> Filter(FilteringHeader<SeatTypeListItem> filtering)
        {
            var seatTypes = _context.SeatTypeList.AsQueryable();

            if (!filtering.ShouldFilter)
                return seatTypes;

            if (!filtering.Filter.Name.IsNullOrWhiteSpace())
                seatTypes = seatTypes.Where(x => x.Name.Contains(filtering.Filter.Name));

            return seatTypes;
        }

        private class SeatTypeListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(SeatTypeListItem.Name),
                nameof(SeatTypeListItem.Order),
                nameof(SeatTypeListItem.IsEffective)
            };

            public SortingHeader DefaultSortingHeader { get; } =
                new SortingHeader(nameof(SeatTypeListItem.Name), SortOrder.Ascending);
        }
    }
}
