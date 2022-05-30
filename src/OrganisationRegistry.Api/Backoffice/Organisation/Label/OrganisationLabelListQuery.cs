namespace OrganisationRegistry.Api.Backoffice.Organisation.Label;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Infrastructure.Search;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Sorting;
using SqlServer.Infrastructure;
using SqlServer.Organisation;

public class OrganisationLabelListQueryResult
{
    public Guid OrganisationLabelId { get; }
    public string LabelTypeName { get; }
    public string LabelValue { get; }
    public DateTime? ValidFrom { get; }
    public DateTime? ValidTo { get; }

    public bool IsActive { get; }

    public bool IsEditable { get; }

    public OrganisationLabelListQueryResult(Guid organisationLabelId,
        string labelTypeName,
        string labelValue,
        DateTime? validFrom,
        DateTime? validTo,
        bool isEditable,
        Guid labelTypeId,
        Func<Guid, bool> isAuthorizedForLabelType)
    {
        OrganisationLabelId = organisationLabelId;
        LabelTypeName = labelTypeName;
        LabelValue = labelValue;
        ValidFrom = validFrom;
        ValidTo = validTo;
        IsEditable = isEditable && isAuthorizedForLabelType(labelTypeId);

        IsActive = new Period(new ValidFrom(validFrom), new ValidTo(validTo)).OverlapsWith(DateTime.Today);
    }
}

public class OrganisationLabelListQuery : Query<OrganisationLabelListItem, OrganisationLabelListItemFilter, OrganisationLabelListQueryResult>
{
    private readonly OrganisationRegistryContext _context;
    private readonly Guid _organisationId;
    private readonly Func<Guid, bool> _isAuthorizedForLabelType;

    protected override ISorting Sorting => new OrganisationLabelListSorting();

    protected override Expression<Func<OrganisationLabelListItem, OrganisationLabelListQueryResult>> Transformation =>
        x => new OrganisationLabelListQueryResult(
            x.OrganisationLabelId,
            x.LabelTypeName,
            x.LabelValue,
            x.ValidFrom,
            x.ValidTo,
            x.IsEditable,
            x.LabelTypeId,
            _isAuthorizedForLabelType);

    public OrganisationLabelListQuery(OrganisationRegistryContext context,
        Guid organisationId,
        Func<Guid, bool> isAuthorizedForLabelType)
    {
        _context = context;
        _organisationId = organisationId;
        _isAuthorizedForLabelType = isAuthorizedForLabelType;
    }

    protected override IQueryable<OrganisationLabelListItem> Filter(FilteringHeader<OrganisationLabelListItemFilter> filtering)
    {
        var organisationLabels = _context.OrganisationLabelList
            .AsQueryable()
            .Where(x => x.OrganisationId == _organisationId).AsQueryable();

        if (filtering.Filter is not { } filter)
            return organisationLabels;

        if (filter.ActiveOnly)
            organisationLabels = organisationLabels.Where(x =>
                (!x.ValidFrom.HasValue || x.ValidFrom <= DateTime.Today) &&
                (!x.ValidTo.HasValue || x.ValidTo >= DateTime.Today));

        return organisationLabels;
    }

    private class OrganisationLabelListSorting : ISorting
    {
        public IEnumerable<string> SortableFields { get; } = new[]
        {
            nameof(OrganisationLabelListItem.LabelTypeName),
            nameof(OrganisationLabelListItem.LabelValue),
            nameof(OrganisationLabelListItem.ValidFrom),
            nameof(OrganisationLabelListItem.ValidTo)
        };

        public SortingHeader DefaultSortingHeader { get; } =
            new SortingHeader(nameof(OrganisationLabelListItem.LabelTypeName), SortOrder.Ascending);
    }
}

public class OrganisationLabelListItemFilter
{
    public bool ActiveOnly { get; set; }
}