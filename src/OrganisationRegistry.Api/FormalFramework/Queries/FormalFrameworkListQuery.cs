namespace OrganisationRegistry.Api.FormalFramework.Queries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
    using Infrastructure.Search;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Sorting;
    using SqlServer.FormalFramework;
    using SqlServer.Infrastructure;

    public class FormalFrameworkListQueryResult
    {
        public Guid Id { get; }
        public string Code { get; }
        public string FormalFrameworkCategoryName { get; }
        public string Name { get; }

        public FormalFrameworkListQueryResult(
            Guid id,
            string code,
            string formalFrameworkCategoryName,
            string name)
        {
            Id = id;
            Code = code;
            FormalFrameworkCategoryName = formalFrameworkCategoryName;
            Name = name;
        }
    }

    public class FormalFrameworkListQuery : Query<FormalFrameworkListItem, FormalFrameworkListItemFilter, FormalFrameworkListQueryResult>
    {
        private readonly OrganisationRegistryContext _context;

        protected override ISorting Sorting => new FormalFrameworkListSorting();

        protected override Expression<Func<FormalFrameworkListItem, FormalFrameworkListQueryResult>> Transformation =>
            x => new FormalFrameworkListQueryResult(
                x.Id,
                x.Code,
                x.FormalFrameworkCategoryName,
                x.Name);

        public FormalFrameworkListQuery(OrganisationRegistryContext context)
        {
            _context = context;
        }

        protected override IQueryable<FormalFrameworkListItem> Filter(FilteringHeader<FormalFrameworkListItemFilter> filtering)
        {
            var formalFrameworks = _context.FormalFrameworkList.AsQueryable();

            if (!filtering.ShouldFilter)
                return formalFrameworks;

            if (!filtering.Filter.Ids.IsNullOrEmpty())
                formalFrameworks = formalFrameworks.Where(x => filtering.Filter.Ids.Contains(x.Id));

            if (!filtering.Filter.Name.IsNullOrWhiteSpace())
                formalFrameworks = formalFrameworks.Where(x => x.Name.Contains(filtering.Filter.Name));

            if (!filtering.Filter.Code.IsNullOrWhiteSpace())
                formalFrameworks = formalFrameworks.Where(x => x.Code.Contains(filtering.Filter.Code));

            if (!filtering.Filter.FormalFrameworkCategoryName.IsNullOrWhiteSpace())
                formalFrameworks = formalFrameworks.Where(x => x.FormalFrameworkCategoryName.Contains(filtering.Filter.FormalFrameworkCategoryName));

            return formalFrameworks;
        }

        private class FormalFrameworkListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(FormalFrameworkListItem.Name),
                nameof(FormalFrameworkListItem.Code),
                nameof(FormalFrameworkListItem.FormalFrameworkCategoryName),
            };

            public SortingHeader DefaultSortingHeader { get; } =
                new SortingHeader(nameof(FormalFrameworkListItem.Name), SortOrder.Ascending);
        }
    }

    public class FormalFrameworkListItemFilter
    {
        public IList<Guid> Ids { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public string FormalFrameworkCategoryName { get; set; }

        public FormalFrameworkListItemFilter()
        {
            Ids = new List<Guid>();
        }
    }
}
