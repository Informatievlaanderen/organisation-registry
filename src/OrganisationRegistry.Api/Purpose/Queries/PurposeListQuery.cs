namespace OrganisationRegistry.Api.Purpose.Queries
{
    using System.Collections.Generic;
    using System.Linq;
    using Infrastructure.Search;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Sorting;
    using SqlServer.Purpose;
    using SqlServer.Infrastructure;
    using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;

    public class PurposeListQuery: Query<PurposeListItem>
    {
        private readonly OrganisationRegistryContext _context;

        protected override ISorting Sorting => new PurposeListSorting();

        public PurposeListQuery(OrganisationRegistryContext context)
        {
            _context = context;
        }

        protected override IQueryable<PurposeListItem> Filter(FilteringHeader<PurposeListItem> filtering)
        {
            var purposes = _context.PurposeList.AsQueryable();

            if (!filtering.ShouldFilter)
                return purposes;

            if (!filtering.Filter.Name.IsNullOrWhiteSpace())
                purposes = purposes.Where(x => x.Name.Contains(filtering.Filter.Name));

            return purposes;
        }

        private class PurposeListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(PurposeListItem.Name)
            };

            public SortingHeader DefaultSortingHeader { get; } =
                new SortingHeader(nameof(PurposeListItem.Name), SortOrder.Ascending);
        }
    }
}
