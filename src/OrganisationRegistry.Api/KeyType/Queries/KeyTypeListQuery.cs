namespace OrganisationRegistry.Api.KeyType.Queries
{
    using System.Collections.Generic;
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
    using Infrastructure.Search;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Sorting;
    using SqlServer.Infrastructure;
    using SqlServer.KeyType;

    public class KeyTypeListQuery: Query<KeyTypeListItem>
    {
        private readonly OrganisationRegistryContext _context;

        protected override ISorting Sorting => new KeyTypeListSorting();

        public KeyTypeListQuery(OrganisationRegistryContext context)
        {
            _context = context;
        }

        protected override IQueryable<KeyTypeListItem> Filter(FilteringHeader<KeyTypeListItem> filtering)
        {
            var keyTypes = _context.KeyTypeList.AsQueryable();

            if (!filtering.ShouldFilter)
                return keyTypes;

            if (!filtering.Filter.Name.IsNullOrWhiteSpace())
                keyTypes = keyTypes.Where(x => x.Name.Contains(filtering.Filter.Name));

            return keyTypes;
        }

        private class KeyTypeListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(KeyTypeListItem.Name)
            };

            public SortingHeader DefaultSortingHeader { get; } =
                new SortingHeader(nameof(KeyTypeListItem.Name), SortOrder.Ascending);
        }
    }
}
