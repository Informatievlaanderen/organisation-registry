namespace OrganisationRegistry.Api.Backoffice.Organisation.Relation;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Infrastructure.Search;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Sorting;
using SqlServer.Infrastructure;
using SqlServer.Organisation;

public class OrganisationRelationListQueryResult
{
    public string RelationName { get; }
    public Guid OrganisationRelationId { get; }
    public Guid RelatedOrganisationId { get; }
    public string RelatedOrganisationName { get; }
    public DateTime? ValidFrom { get; }
    public DateTime? ValidTo { get; }

    public bool IsActive { get; }

    public OrganisationRelationListQueryResult(
        Guid organisationRelationId,
        Guid relatedOrganisationId,
        string relatedOrganisationName,
        string functionName,
        DateTime? validFrom,
        DateTime? validTo)
    {
        OrganisationRelationId = organisationRelationId;
        RelatedOrganisationId = relatedOrganisationId;
        RelatedOrganisationName = relatedOrganisationName;
        RelationName = functionName;
        ValidFrom = validFrom;
        ValidTo = validTo;

        IsActive = new Period(new ValidFrom(validFrom), new ValidTo(validTo)).OverlapsWith(DateTime.Today);
    }
}

public class OrganisationRelationListQuery : Query<OrganisationRelationListItem, OrganisationRelationListItemFilter, OrganisationRelationListQueryResult>
{
    private readonly OrganisationRegistryContext _context;
    private readonly Guid _organisationId;

    protected override ISorting Sorting => new OrganisationRelationListSorting();

    protected override Expression<Func<OrganisationRelationListItem, OrganisationRelationListQueryResult>> Transformation =>
        x => new OrganisationRelationListQueryResult(
            x.OrganisationRelationId,
            x.RelatedOrganisationId,
            x.RelatedOrganisationName,
            x.RelationName,
            x.ValidFrom,
            x.ValidTo);

    public OrganisationRelationListQuery(OrganisationRegistryContext context, Guid organisationId)
    {
        _context = context;
        _organisationId = organisationId;
    }

    protected override IQueryable<OrganisationRelationListItem> Filter(FilteringHeader<OrganisationRelationListItemFilter> filtering)
    {
        var organisationRelations = _context.OrganisationRelationList
            .AsQueryable()
            .Where(x => x.OrganisationId == _organisationId);

        if (filtering.Filter is not { } filter)
            return organisationRelations;

        if (filter.ActiveOnly)
            organisationRelations = organisationRelations.Where(x =>
                (!x.ValidFrom.HasValue || x.ValidFrom <= DateTime.Today) &&
                (!x.ValidTo.HasValue || x.ValidTo >= DateTime.Today));

        return organisationRelations;
    }

    private class OrganisationRelationListSorting : ISorting
    {
        public IEnumerable<string> SortableFields { get; } = new[]
        {
            nameof(OrganisationRelationListItem.RelationName),
            nameof(OrganisationRelationListItem.RelatedOrganisationName),
            nameof(OrganisationRelationListItem.ValidFrom),
            nameof(OrganisationRelationListItem.ValidTo)
        };

        public SortingHeader DefaultSortingHeader { get; } =
            new SortingHeader(nameof(OrganisationRelationListItem.RelatedOrganisationName), SortOrder.Ascending);
    }
}

public class OrganisationRelationListItemFilter
{
    public bool ActiveOnly { get; set; }
}