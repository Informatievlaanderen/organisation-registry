namespace OrganisationRegistry.Api.Backoffice.Parameters.Location.Queries
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
    using SqlServer.Location;

    public class LocationListQueryResult
    {
        public Guid Id { get; }
        public string Street { get; }
        public string City { get; }
        public string ZipCode { get; }
        public string Country { get; }
        public bool HasCrabLocation { get; }

        public LocationListQueryResult(
            Guid id,
            string city,
            string street,
            string zipCode,
            string country,
            bool hasCrabLocation)
        {
            Id = id;
            City = city;
            Street = street;
            ZipCode = zipCode;
            Country = country;
            HasCrabLocation = hasCrabLocation;
        }
    }

    public class LocationListQuery: Query<LocationListItem, LocationListItemFilter, LocationListQueryResult>
    {
        private readonly OrganisationRegistryContext _context;

        protected override ISorting Sorting => new LocationListSorting();

        protected override Expression<Func<LocationListItem, LocationListQueryResult>> Transformation =>
            x => new LocationListQueryResult(
                x.Id,
                x.City,
                x.Street,
                x.ZipCode,
                x.Country,
                x.HasCrabLocation);

        public LocationListQuery(OrganisationRegistryContext context)
        {
            _context = context;
        }

        protected override IQueryable<LocationListItem> Filter(FilteringHeader<LocationListItemFilter> filtering)
        {
            var locations = _context.LocationList.AsQueryable();

            if (!filtering.ShouldFilter)
                return locations;

            if (!filtering.Filter.Street.IsNullOrWhiteSpace())
                locations = locations.Where(x => x.Street.Contains(filtering.Filter.Street));

            if (!filtering.Filter.ZipCode.IsNullOrWhiteSpace())
                locations = locations.Where(x => x.ZipCode.Contains(filtering.Filter.ZipCode));

            if (!filtering.Filter.City.IsNullOrWhiteSpace())
                locations = locations.Where(x => x.City.Contains(filtering.Filter.City));

            if (!filtering.Filter.Country.IsNullOrWhiteSpace())
                locations = locations.Where(x => x.Country.Contains(filtering.Filter.Country));

            if (filtering.Filter.NonCrabOnly)
                locations = locations.Where(x => string.IsNullOrEmpty(x.CrabLocationId));

            return locations;
        }

        private class LocationListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(LocationListItem.Street),
                nameof(LocationListItem.ZipCode),
                nameof(LocationListItem.City),
                nameof(LocationListItem.Country),
                nameof(LocationListItem.HasCrabLocation),
            };

            public SortingHeader DefaultSortingHeader { get; } =
                new SortingHeader(nameof(LocationListItem.Street), SortOrder.Ascending);
        }
    }

    public class LocationListItemFilter
    {
        public Guid Id { get; set; }

        public string Street { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public bool NonCrabOnly { get; set; }
    }
}
