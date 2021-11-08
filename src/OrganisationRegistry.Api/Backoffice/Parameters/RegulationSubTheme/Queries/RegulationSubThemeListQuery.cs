namespace OrganisationRegistry.Api.Backoffice.Parameters.RegulationSubTheme.Queries
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
    using SqlServer.RegulationSubTheme;

    public class RegulationSubThemeListQueryResult
    {
        public Guid Id { get; }
        public string Name { get; }
        public string RegulationThemeName { get; }

        public RegulationSubThemeListQueryResult(
            Guid id,
            string name,
            string regulationThemeName)
        {
            Id = id;
            Name = name;
            RegulationThemeName = regulationThemeName;
        }
    }

    public class RegulationSubThemeListQuery: Query<RegulationSubThemeListItem, RegulationSubThemeListItemFilter, RegulationSubThemeListQueryResult>
    {
        private readonly OrganisationRegistryContext _context;

        protected override ISorting Sorting => new RegulationSubThemeListSorting();

        protected override Expression<Func<RegulationSubThemeListItem, RegulationSubThemeListQueryResult>> Transformation =>
            x => new RegulationSubThemeListQueryResult(
                x.Id,
                x.Name,
                x.RegulationThemeName);

        public RegulationSubThemeListQuery(OrganisationRegistryContext context)
        {
            _context = context;
        }

        protected override IQueryable<RegulationSubThemeListItem> Filter(FilteringHeader<RegulationSubThemeListItemFilter> filtering)
        {
            var regulationSubThemes = _context.RegulationSubThemeList.AsQueryable();

            if (!filtering.ShouldFilter)
                return regulationSubThemes;

            if (!filtering.Filter.Name.IsNullOrWhiteSpace())
                regulationSubThemes = regulationSubThemes.Where(x => x.Name.Contains(filtering.Filter.Name));

            // TODO: be able to filter on active

            if (!filtering.Filter.RegulationThemeName.IsNullOrWhiteSpace())
                regulationSubThemes = regulationSubThemes.Where(x => x.RegulationThemeName.Contains(filtering.Filter.RegulationThemeName));

            if (!filtering.Filter.RegulationThemeId.IsEmptyGuid())
                regulationSubThemes = regulationSubThemes.Where(x => x.RegulationThemeId == filtering.Filter.RegulationThemeId);

            return regulationSubThemes;
        }

        private class RegulationSubThemeListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(RegulationSubThemeListItem.Name),
                nameof(RegulationSubThemeListItem.RegulationThemeName),
            };

            public SortingHeader DefaultSortingHeader { get; } =
                new SortingHeader(nameof(RegulationSubThemeListItem.Name), SortOrder.Ascending);
        }
    }

    public class RegulationSubThemeListItemFilter
    {
        public string Name { get; set; }
        public Guid RegulationThemeId { get; set; }
        public string RegulationThemeName { get; set; }
    }
}
