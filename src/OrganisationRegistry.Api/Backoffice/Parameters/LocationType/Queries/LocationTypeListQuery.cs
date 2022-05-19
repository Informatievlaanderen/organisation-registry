namespace OrganisationRegistry.Api.Backoffice.Parameters.LocationType.Queries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
    using Infrastructure.Search;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Sorting;
    using OrganisationRegistry.Infrastructure.Configuration;
    using SqlServer.Infrastructure;
    using SqlServer.LocationType;

    public class LocationTypeListQuery: Query<LocationTypeListItem, LocationTypeListItem, LocationTypeListItemResult>
    {
        private readonly OrganisationRegistryContext _context;
        private readonly IOrganisationRegistryConfiguration _configuration;

        protected override ISorting Sorting => new LocationTypeListSorting();

        public LocationTypeListQuery(OrganisationRegistryContext context, IOrganisationRegistryConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        protected override Expression<Func<LocationTypeListItem, LocationTypeListItemResult>> Transformation =>
            x => new LocationTypeListItemResult
            {
                Id = x.Id,
                Name = x.Name,
                UserPermitted = x.Id != _configuration.Kbo.KboV2RegisteredOfficeLocationTypeId
            };

        protected override IQueryable<LocationTypeListItem> Filter(FilteringHeader<LocationTypeListItem> filtering)
        {
            var locationTypes = _context.LocationTypeList.AsQueryable();

            if (filtering.Filter is not { } filter)
                return locationTypes;

            if (!filter.Name.IsNullOrWhiteSpace())
                locationTypes = locationTypes.Where(x => x.Name.Contains(filter.Name));

            return locationTypes;
        }

        private class LocationTypeListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(LocationTypeListItem.Name)
            };

            public SortingHeader DefaultSortingHeader { get; } = new(nameof(LocationTypeListItem.Name), SortOrder.Ascending);
        }
    }

    public class LocationTypeListItemResult
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public bool UserPermitted { get; set; }
    }
}
