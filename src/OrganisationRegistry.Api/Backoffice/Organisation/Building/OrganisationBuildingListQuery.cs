namespace OrganisationRegistry.Api.Backoffice.Organisation.Building;

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

public class OrganisationBuildingListQueryResult
{
    public Guid OrganisationBuildingId { get; }
    public string BuildingName { get; }
    public bool IsMainBuilding { get; }
    public DateTime? ValidFrom { get; }
    public DateTime? ValidTo { get; }

    public bool IsActive { get; }

    public bool IsEditable { get; }

    public ResourceEditPermissions Permissions { get; }

    public OrganisationBuildingListQueryResult(
        Guid organisationBuildingId,
        string buildingName,
        bool isMainBuilding,
        DateTime? validFrom,
        DateTime? validTo,
        string ovoNumber,
        IUser user)
    {
        OrganisationBuildingId = organisationBuildingId;
        BuildingName = buildingName;
        IsMainBuilding = isMainBuilding;
        ValidFrom = validFrom;
        ValidTo = validTo;

        IsActive = new Period(new ValidFrom(validFrom), new ValidTo(validTo)).OverlapsWith(DateTime.Today);
        IsEditable = new BuildingPolicy(ovoNumber).Check(user).IsSuccessful;
        Permissions = new ResourceEditPermissions(IsEditable);
    }
}

public class OrganisationBuildingListQuery : Query<OrganisationBuildingListItem, OrganisationBuildingListItemFilter, OrganisationBuildingListQueryResult>
{
    private readonly OrganisationRegistryContext _context;
    private readonly IMemoryCaches _memoryCaches;
    private readonly Guid _organisationId;
    private readonly IUser _user;

    protected override ISorting Sorting => new OrganisationBuildingListSorting();

    protected override Expression<Func<OrganisationBuildingListItem, OrganisationBuildingListQueryResult>> Transformation =>
        x => new OrganisationBuildingListQueryResult(
            x.OrganisationBuildingId,
            x.BuildingName,
            x.IsMainBuilding,
            x.ValidFrom,
            x.ValidTo,
            _memoryCaches.OvoNumbers[x.OrganisationId],
            _user);

    public OrganisationBuildingListQuery(OrganisationRegistryContext context, IMemoryCaches memoryCaches, Guid organisationId, IUser user)
    {
        _context = context;
        _memoryCaches = memoryCaches;
        _organisationId = organisationId;
        _user = user;
    }

    protected override IQueryable<OrganisationBuildingListItem> Filter(FilteringHeader<OrganisationBuildingListItemFilter> filtering)
    {
        var organisationBuildings = _context.OrganisationBuildingList
            .AsQueryable()
            .Where(x => x.OrganisationId == _organisationId).AsQueryable();

        if (filtering.Filter is not { } filter)
            return organisationBuildings;

        if (filter.ActiveOnly)
            organisationBuildings = organisationBuildings.Where(x =>
                (!x.ValidFrom.HasValue || x.ValidFrom <= DateTime.Today) &&
                (!x.ValidTo.HasValue || x.ValidTo >= DateTime.Today));

        return organisationBuildings;
    }

    private class OrganisationBuildingListSorting : ISorting
    {
        public IEnumerable<string> SortableFields { get; } = new[]
        {
            nameof(OrganisationBuildingListItem.BuildingName),
            nameof(OrganisationBuildingListItem.IsMainBuilding),
            nameof(OrganisationBuildingListItem.ValidFrom),
            nameof(OrganisationBuildingListItem.ValidTo),
        };

        public SortingHeader DefaultSortingHeader { get; } =
            new(nameof(OrganisationBuildingListItem.BuildingName), SortOrder.Ascending);
    }
}

public class OrganisationBuildingListItemFilter
{
    public bool ActiveOnly { get; set; }
}
