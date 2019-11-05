namespace OrganisationRegistry.Api.LabelType.Queries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Infrastructure.Search;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Sorting;
    using SqlServer.Infrastructure;
    using SqlServer.LabelType;
    using OrganisationRegistry.Organisation;
    using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;

    public class LabelTypeListQuery: Query<LabelTypeListItem, LabelTypeListItem, LabelTypeListItemResult>
    {
        private readonly OrganisationRegistryContext _context;
        private readonly IOrganisationRegistryConfiguration _configuration;

        protected override IQueryable<LabelTypeListItem> Filter(FilteringHeader<LabelTypeListItem> filtering)
        {
            var labelTypes = _context.LabelTypeList.AsQueryable();

            if (!filtering.ShouldFilter)
                return labelTypes;

            if (!filtering.Filter.Name.IsNullOrWhiteSpace())
                labelTypes = labelTypes.Where(x => x.Name.Contains(filtering.Filter.Name));

            return labelTypes;
        }

        protected override Expression<Func<LabelTypeListItem, LabelTypeListItemResult>> Transformation =>
            x => new LabelTypeListItemResult
            {
                Id = x.Id,
                Name = x.Name,
                UserPermitted = x.Id != _configuration.KboV2FormalNameLabelTypeId
            };

        protected override ISorting Sorting => new LabelTypeListSorting();

        public LabelTypeListQuery(OrganisationRegistryContext context, IOrganisationRegistryConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        private class LabelTypeListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(LabelTypeListItem.Name)
            };

            public SortingHeader DefaultSortingHeader { get; } =
                new SortingHeader(nameof(LabelTypeListItem.Name), SortOrder.Ascending);
        }
    }

    public class LabelTypeListItemResult
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool UserPermitted { get; set; }
    }
}
