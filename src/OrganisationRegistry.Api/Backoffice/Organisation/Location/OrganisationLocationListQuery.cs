namespace OrganisationRegistry.Api.Backoffice.Organisation.Location;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Handling.Authorization;
using Infrastructure.Search;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Sorting;
using OrganisationRegistry.Infrastructure.AppSpecific;
using OrganisationRegistry.Infrastructure.Authorization;
using SqlServer.Infrastructure;
using SqlServer.Organisation;

public class OrganisationLocationListQueryResult
{
    public Guid OrganisationLocationId { get; }
    public bool IsMainLocation { get; }
    public string LocationName { get; }
    public string? LocationTypeName { get; }
    public DateTime? ValidFrom { get; }
    public DateTime? ValidTo { get; }

    public bool IsActive { get; }

    public bool IsEditable { get; }

    public OrganisationLocationListQueryResult(
        Guid organisationLocationId,
        bool isMainLocation,
        string locationName,
        string? locationTypeName,
        DateTime? validFrom,
        DateTime? validTo,
        string ovoNumber,
        IUser user)
    {
        OrganisationLocationId = organisationLocationId;
        IsMainLocation = isMainLocation;
        LocationName = locationName;
        LocationTypeName = locationTypeName;
        ValidFrom = validFrom;
        ValidTo = validTo;

        IsActive = new Period(new ValidFrom(validFrom), new ValidTo(validTo)).OverlapsWith(DateTime.Today);
        IsEditable =
            new BeheerderForOrganisationRegardlessOfVlimpersPolicy(ovoNumber)
                .Check(user)
                .IsSuccessful;
    }
}

public class OrganisationLocationListQuery : Query<OrganisationLocationListItem, OrganisationLocationListItemFilter, OrganisationLocationListQueryResult>
{
    private readonly OrganisationRegistryContext _context;
    private readonly IMemoryCaches _memoryCaches;
    private readonly Guid _organisationId;
    private readonly IUser _user;

    protected override ISorting Sorting => new OrganisationLocationListSorting();

    protected override Expression<Func<OrganisationLocationListItem, OrganisationLocationListQueryResult>> Transformation =>
        x => new OrganisationLocationListQueryResult(
            x.OrganisationLocationId,
            x.IsMainLocation,
            x.LocationName,
            x.LocationTypeName,
            x.ValidFrom,
            x.ValidTo,
            _memoryCaches.OvoNumbers[x.OrganisationId],
            _user);

    public OrganisationLocationListQuery(OrganisationRegistryContext context, IMemoryCaches memoryCaches, Guid organisationId, IUser user)
    {
        _context = context;
        _memoryCaches = memoryCaches;
        _organisationId = organisationId;
        _user = user;
    }

    protected override IQueryable<OrganisationLocationListItem> Filter(FilteringHeader<OrganisationLocationListItemFilter> filtering)
    {
        var organisationLocations = _context.OrganisationLocationList
            .AsQueryable()
            .Where(x => x.OrganisationId == _organisationId).AsQueryable();

        if (filtering.Filter is not { } filter)
            return organisationLocations;

        if (filter.ActiveOnly)
            organisationLocations = organisationLocations.Where(x =>
                (!x.ValidFrom.HasValue || x.ValidFrom <= DateTime.Today) &&
                (!x.ValidTo.HasValue || x.ValidTo >= DateTime.Today));

        return organisationLocations;
    }

    private class OrganisationLocationListSorting : ISorting
    {
        public IEnumerable<string> SortableFields { get; } = new[]
        {
            nameof(OrganisationLocationListItem.LocationName),
            nameof(OrganisationLocationListItem.LocationTypeName),
            nameof(OrganisationLocationListItem.IsMainLocation),
            nameof(OrganisationLocationListItem.ValidFrom),
            nameof(OrganisationLocationListItem.ValidTo)
        };

        public SortingHeader DefaultSortingHeader { get; } =
            new(nameof(OrganisationLocationListItem.LocationName), SortOrder.Ascending);
    }
}

public class OrganisationLocationListItemFilter
{
    public bool ActiveOnly { get; set; }
}