namespace OrganisationRegistry.Api.Backoffice.Admin.Configuration.Queries
{
    using System.Collections.Generic;
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
    using Infrastructure.Search;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Sorting;
    using OrganisationRegistry.Configuration.Database;

    public class ConfigurationListQuery : Query<ConfigurationValue>
    {
        private readonly ConfigurationContext _context;

        protected override ISorting Sorting => new ConfigurationListSorting();

        public ConfigurationListQuery(ConfigurationContext context)
        {
            _context = context;
        }

        protected override IQueryable<ConfigurationValue> Filter(FilteringHeader<ConfigurationValue> filtering)
        {
            var configuration = _context.Configuration.AsQueryable();

            if (!filtering.ShouldFilter)
                return configuration;

            if (!filtering.Filter.Key.IsNullOrWhiteSpace())
                configuration = configuration.Where(x =>
                    x.Key.Contains(filtering.Filter.Key) ||
                    x.Description.Contains(filtering.Filter.Key) ||
                    x.Value.Contains(filtering.Filter.Key));

            return configuration;
        }

        private class ConfigurationListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(ConfigurationValue.Key)
            };

            public SortingHeader DefaultSortingHeader { get; } =
                new SortingHeader(nameof(ConfigurationValue.Key), SortOrder.Ascending);
        }
    }
}
