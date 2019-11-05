namespace OrganisationRegistry.Api.LifecyclePhaseType.Queries
{
    using System.Collections.Generic;
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
    using Infrastructure.Search;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Sorting;
    using SqlServer.Infrastructure;
    using SqlServer.LifecyclePhaseType;

    public class LifecyclePhaseTypeListQuery: Query<LifecyclePhaseTypeListItem>
    {
        private readonly OrganisationRegistryContext _context;

        protected override ISorting Sorting => new LifecyclePhaseTypeListSorting();

        public LifecyclePhaseTypeListQuery(OrganisationRegistryContext context)
        {
            _context = context;
        }

        protected override IQueryable<LifecyclePhaseTypeListItem> Filter(FilteringHeader<LifecyclePhaseTypeListItem> filtering)
        {
            var lifecyclePhaseTypes = _context.LifecyclePhaseTypeList.AsQueryable();

            if (!filtering.ShouldFilter)
                return lifecyclePhaseTypes;

            if (!filtering.Filter.Name.IsNullOrWhiteSpace())
                lifecyclePhaseTypes = lifecyclePhaseTypes.Where(x => x.Name.Contains(filtering.Filter.Name));

            return lifecyclePhaseTypes;
        }

        private class LifecyclePhaseTypeListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(LifecyclePhaseTypeListItem.Name),
                nameof(LifecyclePhaseTypeListItem.RepresentsActivePhase),
                nameof(LifecyclePhaseTypeListItem.IsDefaultPhase)
            };

            public SortingHeader DefaultSortingHeader { get; } =
                new SortingHeader(nameof(LifecyclePhaseTypeListItem.Name), SortOrder.Ascending);
        }
    }
}
