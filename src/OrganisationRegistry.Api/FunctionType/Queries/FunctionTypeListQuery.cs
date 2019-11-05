namespace OrganisationRegistry.Api.FunctionType.Queries
{
    using System.Collections.Generic;
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
    using Infrastructure.Search;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Sorting;
    using SqlServer.FunctionType;
    using SqlServer.Infrastructure;

    public class FunctionTypeListQuery : Query<FunctionTypeListItem>
    {
        private readonly OrganisationRegistryContext _context;

        protected override ISorting Sorting => new FunctionTypeListSorting();

        public FunctionTypeListQuery(OrganisationRegistryContext context)
        {
            _context = context;
        }

        protected override IQueryable<FunctionTypeListItem> Filter(FilteringHeader<FunctionTypeListItem> filtering)
        {
            var functionTypes = _context.FunctionTypeList.AsQueryable();

            if (!filtering.ShouldFilter)
                return functionTypes;

            if (!filtering.Filter.Name.IsNullOrWhiteSpace())
                functionTypes = functionTypes.Where(x => x.Name.Contains(filtering.Filter.Name));

            return functionTypes;
        }

        private class FunctionTypeListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(FunctionTypeListItem.Name)
            };

            public SortingHeader DefaultSortingHeader { get; } =
                new SortingHeader(nameof(FunctionTypeListItem.Name), SortOrder.Ascending);
        }
    }
}
