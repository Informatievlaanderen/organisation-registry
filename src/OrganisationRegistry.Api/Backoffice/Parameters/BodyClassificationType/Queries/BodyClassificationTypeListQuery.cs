namespace OrganisationRegistry.Api.Backoffice.Parameters.BodyClassificationType.Queries
{
    using System.Collections.Generic;
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
    using Infrastructure.Search;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Sorting;
    using SqlServer.BodyClassificationType;
    using SqlServer.Infrastructure;

    public class BodyClassificationTypeListQuery : Query<BodyClassificationTypeListItem>
    {
        private readonly OrganisationRegistryContext _context;

        protected override ISorting Sorting => new BodyClassificationTypeListSorting();

        public BodyClassificationTypeListQuery(OrganisationRegistryContext context)
        {
            _context = context;
        }

        protected override IQueryable<BodyClassificationTypeListItem> Filter(FilteringHeader<BodyClassificationTypeListItem> filtering)
        {
            var bodyClassificationTypes = _context.BodyClassificationTypeList.AsQueryable();

            if (filtering.Filter is not { } filter)
                return bodyClassificationTypes;

            if (!filter.Name.IsNullOrWhiteSpace())
                bodyClassificationTypes = bodyClassificationTypes.Where(x => x.Name.Contains(filter.Name));

            return bodyClassificationTypes;
        }

        private class BodyClassificationTypeListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(BodyClassificationTypeListItem.Name)
            };

            public SortingHeader DefaultSortingHeader { get; } =
                new SortingHeader(nameof(BodyClassificationTypeListItem.Name), SortOrder.Ascending);
        }
    }
}
