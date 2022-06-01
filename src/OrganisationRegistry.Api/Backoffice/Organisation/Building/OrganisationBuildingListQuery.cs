namespace OrganisationRegistry.Api.Backoffice.Organisation.Building;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Infrastructure.Search;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Sorting;
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

    public OrganisationBuildingListQueryResult(
        Guid organisationBuildingId,
        string buildingName,
        bool isMainBuilding,
        DateTime? validFrom,
        DateTime? validTo)
    {
        OrganisationBuildingId = organisationBuildingId;
        BuildingName = buildingName;
        IsMainBuilding = isMainBuilding;
        ValidFrom = validFrom;
        ValidTo = validTo;

        IsActive = new Period(new ValidFrom(validFrom), new ValidTo(validTo)).OverlapsWith(DateTime.Today);
    }
}

public class OrganisationBuildingListQuery : Query<OrganisationBuildingListItem, OrganisationBuildingListItemFilter, OrganisationBuildingListQueryResult>
{
    private readonly OrganisationRegistryContext _context;
    private readonly Guid _organisationId;

    protected override ISorting Sorting => new OrganisationBuildingListSorting();

    protected override Expression<Func<OrganisationBuildingListItem, OrganisationBuildingListQueryResult>> Transformation =>
        x => new OrganisationBuildingListQueryResult(
            x.OrganisationBuildingId,
            x.BuildingName,
            x.IsMainBuilding,
            x.ValidFrom,
            x.ValidTo);

    public OrganisationBuildingListQuery(OrganisationRegistryContext context, Guid organisationId)
    {
        _context = context;
        _organisationId = organisationId;
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
            nameof(OrganisationBuildingListItem.ValidTo)
        };

        public SortingHeader DefaultSortingHeader { get; } =
            new SortingHeader(nameof(OrganisationBuildingListItem.BuildingName), SortOrder.Ascending);
    }
}

public class OrganisationBuildingListItemFilter
{
    public bool ActiveOnly { get; set; }
}
