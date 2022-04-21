namespace OrganisationRegistry.Api.Backoffice.Parameters.LabelType.Queries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
    using Infrastructure.Search;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Sorting;
    using OrganisationRegistry.Configuration;
    using SqlServer.Infrastructure;
    using SqlServer.LabelType;

    public class LabelTypeListQuery: Query<LabelTypeListItem, LabelTypeListItem, LabelTypeListItemResult>
    {
        private readonly OrganisationRegistryContext _context;
        private readonly IOrganisationRegistryConfiguration _configuration;
        private readonly Func<Guid, bool> _isAuthorizedForLabelType;

        protected override IQueryable<LabelTypeListItem> Filter(FilteringHeader<LabelTypeListItem> filtering)
        {
            var labelTypes = _context.LabelTypeList.AsQueryable();

            if (filtering.Filter is not { } filter)
                return labelTypes;

            if (!filter.Name.IsNullOrWhiteSpace())
                labelTypes = labelTypes.Where(x => x.Name.Contains(filtering.Filter.Name));

            return labelTypes;
        }

        protected override Expression<Func<LabelTypeListItem, LabelTypeListItemResult>> Transformation =>
            x => new LabelTypeListItemResult(
                x.Id,
                x.Name,
                x.Id != _configuration.Kbo.KboV2FormalNameLabelTypeId,
                _isAuthorizedForLabelType);

        protected override ISorting Sorting => new LabelTypeListSorting();

        public LabelTypeListQuery(OrganisationRegistryContext context, IOrganisationRegistryConfiguration configuration,
            Func<Guid, bool> isAuthorizedForLabelType)
        {
            _context = context;
            _configuration = configuration;
            _isAuthorizedForLabelType = isAuthorizedForLabelType;
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

        public LabelTypeListItemResult(Guid id, string name, bool userPermitted, Func<Guid, bool> policyFunc)
        {
            Id = id;
            Name = name;
            UserPermitted = userPermitted && policyFunc(id);
        }

        public Guid Id { get; }

        public string Name { get; }

        public bool UserPermitted { get; }
    }
}
