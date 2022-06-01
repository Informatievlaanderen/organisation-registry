namespace OrganisationRegistry.Api.Backoffice.Body.Mandate;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using OrganisationRegistry.Api.Infrastructure.Search;
using OrganisationRegistry.Api.Infrastructure.Search.Filtering;
using OrganisationRegistry.Api.Infrastructure.Search.Sorting;
using OrganisationRegistry.Body;
using OrganisationRegistry.SqlServer.Body;
using OrganisationRegistry.SqlServer.Infrastructure;

public class BodyMandateListQueryResult
{
    public Guid BodyMandateId { get; }
    public BodyMandateType BodyMandateType { get; }
    public Guid BodySeatId { get; }
    public string? BodySeatNumber { get; }
    public string BodySeatName { get; }

    public Guid DelegatorId { get; }
    public string DelegatorName { get; }
    public Guid? DelegatedId { get; }
    public string? DelegatedName { get; }
    public Guid? AssignedToId { get; set; }
    public string? AssignedToName { get; set; }
    public Guid? BodySeatTypeId { get; }
    public string? BodySeatTypeName { get; }

    public DateTime? ValidFrom { get; }
    public DateTime? ValidTo { get; }

    public bool IsActive { get; }

    public BodyMandateListQueryResult(Guid bodyMandateId,
        BodyMandateType bodyMandateType,
        Guid bodySeatId,
        string? bodySeatNumber,
        string bodySeatName,
        Guid delegatorId,
        string delegatorName,
        Guid? delegatedId,
        string? delegatedName,
        Guid? assignedToId,
        string? assignedToName,
        Guid? bodySeatTypeId,
        string? bodySeatTypeName,
        DateTime? validFrom,
        DateTime? validTo)
    {
        BodyMandateId = bodyMandateId;
        BodyMandateType = bodyMandateType;
        BodySeatId = bodySeatId;
        BodySeatNumber = bodySeatNumber;
        BodySeatName = bodySeatName;
        DelegatorId = delegatorId;
        DelegatorName = delegatorName;
        DelegatedId = delegatedId;
        DelegatedName = delegatedName;
        AssignedToId = assignedToId;
        AssignedToName = assignedToName;
        BodySeatTypeId = bodySeatTypeId;
        BodySeatTypeName = bodySeatTypeName;
        ValidFrom = validFrom;
        ValidTo = validTo;

        IsActive = new Period(new ValidFrom(validFrom), new ValidTo(validTo)).OverlapsWith(DateTime.Today);
    }
}

public class BodyMandateListQuery : Query<BodyMandateListItem, BodyMandateListItemFilter, BodyMandateListQueryResult>
{
    private readonly OrganisationRegistryContext _context;
    private readonly Guid _bodyId;

    protected override ISorting Sorting => new BodyMandateListSorting();

    protected override Expression<Func<BodyMandateListItem, BodyMandateListQueryResult>> Transformation =>
        x => new BodyMandateListQueryResult(
            x.BodyMandateId,
            x.BodyMandateType,
            x.BodySeatId,
            x.BodySeatNumber,
            x.BodySeatName,
            x.DelegatorId,
            x.DelegatorName,
            x.DelegatedId,
            x.DelegatedName,
            x.AssignedToId,
            x.AssignedToName,
            x.BodySeatTypeId,
            x.BodySeatTypeName,
            x.ValidFrom,
            x.ValidTo);

    public BodyMandateListQuery(OrganisationRegistryContext context, Guid bodyId)
    {
        _context = context;
        _bodyId = bodyId;
    }

    protected override IQueryable<BodyMandateListItem> Filter(FilteringHeader<BodyMandateListItemFilter> filtering)
    {
        var bodyMandates = _context.BodyMandateList
            .AsQueryable()
            .Where(x => x.BodyId == _bodyId).AsQueryable();

        if (filtering.Filter is not { } filter)
            return bodyMandates;

        if (filter.ActiveOnly)
            bodyMandates = bodyMandates.Where(x =>
                (!x.ValidFrom.HasValue || x.ValidFrom <= DateTime.Today) &&
                (!x.ValidTo.HasValue || x.ValidTo >= DateTime.Today));

        return bodyMandates;
    }

    private class BodyMandateListSorting : ISorting
    {
        public IEnumerable<string> SortableFields { get; } = new[]
        {
            nameof(BodyMandateListItem.BodySeatName),
            nameof(BodyMandateListItem.BodySeatNumber),
            nameof(BodyMandateListItem.DelegatorName),
            nameof(BodyMandateListItem.ValidFrom),
            nameof(BodyMandateListItem.ValidTo),
            nameof(BodyMandateListItem.BodySeatTypeName),
            nameof(BodyMandateListItem.BodySeatTypeOrder)
        };

        public SortingHeader DefaultSortingHeader { get; } =
            new SortingHeader(nameof(BodyMandateListItem.BodySeatTypeOrder), SortOrder.Ascending);
    }
}

public class BodyMandateListItemFilter
{
    public bool ActiveOnly { get; set; }
}
