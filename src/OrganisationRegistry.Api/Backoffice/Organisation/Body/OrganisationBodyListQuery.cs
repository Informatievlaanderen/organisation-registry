namespace OrganisationRegistry.Api.Backoffice.Organisation.Body;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Handling.Authorization;
using Infrastructure.Search;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Sorting;
using OrganisationRegistry.Infrastructure.Authorization;
using SqlServer.Infrastructure;
using SqlServer.Organisation;

public class OrganisationBodyListQueryResult
{
    public Guid OrganisationBodyId { get; }
    public Guid BodyId { get; }
    public string BodyName { get; }
    public DateTime? ValidFrom { get; }
    public DateTime? ValidTo { get; }

    public bool IsActive { get; }

    public bool IsEditable { get; }

    public ResourceEditPermissions Permissions { get; }

    public OrganisationBodyListQueryResult(
        Guid organisationBodyId,
        Guid bodyId,
        string bodyName,
        DateTime? validFrom,
        DateTime? validTo,
        IUser user)
    {
        OrganisationBodyId = organisationBodyId;
        BodyId = bodyId;
        BodyName = bodyName;
        ValidFrom = validFrom;
        ValidTo = validTo;

        IsActive = new Period(new ValidFrom(validFrom), new ValidTo(validTo)).OverlapsWith(DateTime.Today);
        IsEditable = new BodyPolicy(Permission.BodiesCanManageOrganisations, bodyId).Check(user).IsSuccessful;
        Permissions = new ResourceEditPermissions(IsEditable);
    }
}

public class OrganisationBodyListQuery : Query<OrganisationBodyListItem, OrganisationBodyListItemFilter, OrganisationBodyListQueryResult>
{
    private readonly OrganisationRegistryContext _context;
    private readonly Guid _organisationId;
    private readonly IUser _user;

    protected override ISorting Sorting => new OrganisationBodyListSorting();

    protected override Expression<Func<OrganisationBodyListItem, OrganisationBodyListQueryResult>> Transformation =>
        x => new OrganisationBodyListQueryResult(
            x.OrganisationBodyId,
            x.BodyId,
            x.BodyName,
            x.ValidFrom,
            x.ValidTo,
            _user);

    public OrganisationBodyListQuery(OrganisationRegistryContext context, Guid organisationId, IUser user)
    {
        _context = context;
        _organisationId = organisationId;
        _user = user;
    }

    protected override IQueryable<OrganisationBodyListItem> Filter(FilteringHeader<OrganisationBodyListItemFilter> filtering)
    {
        var organisationBodies = _context.OrganisationBodyList
            .AsQueryable()
            .Where(x => x.OrganisationId == _organisationId).AsQueryable();

        if (filtering.Filter is not { } filter)
            return organisationBodies;

        if (filter.ActiveOnly)
            organisationBodies = organisationBodies.Where(x =>
                (!x.ValidFrom.HasValue || x.ValidFrom <= DateTime.Today) &&
                (!x.ValidTo.HasValue || x.ValidTo >= DateTime.Today));

        return organisationBodies;
    }

    private class OrganisationBodyListSorting : ISorting
    {
        public IEnumerable<string> SortableFields { get; } = new[]
        {
            nameof(OrganisationBodyListItem.BodyName),
            nameof(OrganisationBodyListItem.ValidFrom),
            nameof(OrganisationBodyListItem.ValidTo),
        };

        public SortingHeader DefaultSortingHeader { get; } = new(nameof(OrganisationBodyListItem.BodyName), SortOrder.Ascending);
    }
}

public class OrganisationBodyListItemFilter
{
    public bool ActiveOnly { get; set; }
}
