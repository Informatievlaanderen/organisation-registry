namespace OrganisationRegistry.Api.Backoffice.Parameters.KeyType.Queries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
    using Infrastructure.Search;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Sorting;
    using SqlServer.Infrastructure;
    using SqlServer.KeyType;

    public class KeyTypeListQuery: Query<KeyTypeListItem, KeyTypeListQuery.KeyTypeListItemFilter, KeyTypeListItemResult>
    {
        private readonly OrganisationRegistryContext _context;
        private readonly Func<Guid, bool> _isAuthorizedForKeyType;

        protected override ISorting Sorting => new KeyTypeListSorting();

        public KeyTypeListQuery(OrganisationRegistryContext context, Func<Guid, bool> isAuthorizedForKeyType)
        {
            _context = context;
            _isAuthorizedForKeyType = isAuthorizedForKeyType;
        }

        protected override IQueryable<KeyTypeListItem> Filter(FilteringHeader<KeyTypeListItemFilter> filtering)
        {
            var keyTypes = _context.KeyTypeList.AsQueryable();

            if (filtering.Filter is not { } filter)
                return keyTypes;

            if (!filter.Name.IsNullOrWhiteSpace())
                keyTypes = keyTypes.Where(x => x.Name.Contains(filter.Name));

            if (filter.ExcludeIds != null && filter.ExcludeIds.Any())
                keyTypes = keyTypes.Where(x => !filter.ExcludeIds.Contains(x.Id));

            return keyTypes;
        }

        protected override Expression<Func<KeyTypeListItem, KeyTypeListItemResult>> Transformation =>
            x => new KeyTypeListItemResult(
                x.Id,
                x.Name,
                _isAuthorizedForKeyType);

        public class KeyTypeListItemFilter
        {
            public KeyTypeListItemFilter()
            {
                ExcludeIds = new List<Guid>();
            }
            public Guid Id { get; set; }

            public string Name { get; set; }

            public List<Guid> ExcludeIds { get; }

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

    public class KeyTypeListItemResult
    {
        public KeyTypeListItemResult(Guid id, string name, Func<Guid,bool> isAuthorizedForKeyType)
        {
            Id = id;
            Name = name;
            UserPermitted = isAuthorizedForKeyType(id);
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool UserPermitted { get; set; }
    }
}
