namespace OrganisationRegistry.Api.FormalFrameworkCategory.Queries
{
    using System.Collections.Generic;
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
    using Infrastructure.Search;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Sorting;
    using SqlServer.FormalFrameworkCategory;
    using SqlServer.Infrastructure;

    public class FormalFrameworkCategoryListQuery: Query<FormalFrameworkCategoryListItem>
    {
        private readonly OrganisationRegistryContext _context;

        protected override ISorting Sorting => new FormalFrameworkCategoryListSorting();

        public FormalFrameworkCategoryListQuery(OrganisationRegistryContext context)
        {
            _context = context;
        }

        protected override IQueryable<FormalFrameworkCategoryListItem> Filter(FilteringHeader<FormalFrameworkCategoryListItem> filtering)
        {
            var formalFrameworkCategories = _context.FormalFrameworkCategoryList.AsQueryable();

            if (!filtering.ShouldFilter)
                return formalFrameworkCategories;

            if (!filtering.Filter.Name.IsNullOrWhiteSpace())
                formalFrameworkCategories = formalFrameworkCategories.Where(x => x.Name.Contains(filtering.Filter.Name));

            return formalFrameworkCategories;
        }

        private class FormalFrameworkCategoryListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(FormalFrameworkCategoryListItem.Name)
            };

            public SortingHeader DefaultSortingHeader { get; } =
                new SortingHeader(nameof(FormalFrameworkCategoryListItem.Name), SortOrder.Ascending);
        }
    }
}
