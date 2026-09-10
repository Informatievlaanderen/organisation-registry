namespace OrganisationRegistry.Api.Backoffice.Organisation.Function;

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

public class OrganisationFunctionListQueryResult
{
    public string FunctionName { get; }
    public Guid OrganisationFunctionId { get; }
    public Guid PersonId { get; }
    public string PersonName { get; }
    public DateTime? ValidFrom { get; }
    public DateTime? ValidTo { get; }

    public bool IsActive { get; }

    public bool IsEditable { get; }

    public ResourceEditPermissions Permissions { get; }

    public OrganisationFunctionListQueryResult(
        Guid organisationFunctionId,
        Guid personId,
        string personName,
        string functionName,
        DateTime? validFrom,
        DateTime? validTo,
        string ovoNumber,
        IUser user)
    {
        OrganisationFunctionId = organisationFunctionId;
        PersonId = personId;
        PersonName = personName;
        FunctionName = functionName;
        ValidFrom = validFrom;
        ValidTo = validTo;

        IsActive = new Period(new ValidFrom(validFrom), new ValidTo(validTo)).OverlapsWith(DateTime.Today);
        IsEditable = new FunctionPolicy(ovoNumber).Check(user).IsSuccessful;
        Permissions = new ResourceEditPermissions(IsEditable);
    }
}

public class OrganisationFunctionListQuery : Query<OrganisationFunctionListItem, OrganisationFunctionListItemFilter, OrganisationFunctionListQueryResult>
{
    private readonly OrganisationRegistryContext _context;
    private readonly IMemoryCaches _memoryCaches;
    private readonly Guid _organisationId;
    private readonly IUser _user;

    protected override ISorting Sorting => new OrganisationFunctionListSorting();

    protected override Expression<Func<OrganisationFunctionListItem, OrganisationFunctionListQueryResult>> Transformation =>
        x => new OrganisationFunctionListQueryResult(
            x.OrganisationFunctionId,
            x.PersonId,
            x.PersonName,
            x.FunctionName,
            x.ValidFrom,
            x.ValidTo,
            _memoryCaches.OvoNumbers[x.OrganisationId],
            _user);

    public OrganisationFunctionListQuery(OrganisationRegistryContext context, IMemoryCaches memoryCaches, Guid organisationId, IUser user)
    {
        _context = context;
        _memoryCaches = memoryCaches;
        _organisationId = organisationId;
        _user = user;
    }

    protected override IQueryable<OrganisationFunctionListItem> Filter(FilteringHeader<OrganisationFunctionListItemFilter> filtering)
    {
        var organisationFunctions = _context.OrganisationFunctionList
            .AsQueryable()
            .Where(x => x.OrganisationId == _organisationId).AsQueryable();

        if (filtering.Filter is not { } filter)
            return organisationFunctions;

        if (filter.ActiveOnly)
            organisationFunctions = organisationFunctions.Where(x =>
                (!x.ValidFrom.HasValue || x.ValidFrom <= DateTime.Today) &&
                (!x.ValidTo.HasValue || x.ValidTo >= DateTime.Today));

        return organisationFunctions;
    }

    private class OrganisationFunctionListSorting : ISorting
    {
        public IEnumerable<string> SortableFields { get; } = new[]
        {
            nameof(OrganisationFunctionListItem.FunctionName),
            nameof(OrganisationFunctionListItem.PersonName),
            nameof(OrganisationFunctionListItem.ValidFrom),
            nameof(OrganisationFunctionListItem.ValidTo),
        };

        public SortingHeader DefaultSortingHeader { get; } =
            new(nameof(OrganisationFunctionListItem.PersonName), SortOrder.Ascending);
    }
}

public class OrganisationFunctionListItemFilter
{
    public bool ActiveOnly { get; set; }
}
