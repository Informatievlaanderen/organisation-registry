namespace OrganisationRegistry.Api.Backoffice.Organisation.FormalFramework;

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
using OrganisationRegistry.Infrastructure.Configuration;
using SqlServer.Infrastructure;
using SqlServer.Organisation;

public class OrganisationFormalFrameworkListQueryResult
{
    public Guid OrganisationFormalFrameworkId { get; }
    public Guid FormalFrameworkId { get; }
    public string? FormalFrameworkName { get; }
    public Guid ParentOrganisationId { get; }
    public string? ParentOrganisationName { get; }
    public DateTime? ValidFrom { get; }
    public DateTime? ValidTo { get; }
    public bool IsActive { get; }

    public bool IsEditable { get; }

    public OrganisationFormalFrameworkListQueryResult(Guid organisationFormalFrameworkId,
        Guid formalFrameworkId, string? formalFrameworkName,
        Guid parentOrganisationId, string? parentOrganisationName,
        DateTime? validFrom, DateTime? validTo,
        string ovoNumber,
        IOrganisationRegistryConfiguration configuration, IUser user)
    {
        OrganisationFormalFrameworkId = organisationFormalFrameworkId;
        FormalFrameworkId = formalFrameworkId;
        FormalFrameworkName = formalFrameworkName;
        ParentOrganisationId = parentOrganisationId;
        ParentOrganisationName = parentOrganisationName;
        ValidFrom = validFrom;
        ValidTo = validTo;

        IsActive = new Period(new ValidFrom(validFrom), new ValidTo(validTo)).OverlapsWith(DateTime.Today);
        IsEditable =
            new FormalFrameworkPolicy(() => ovoNumber, formalFrameworkId, configuration)
                .Check(user)
                .IsSuccessful;
    }
}

public class OrganisationFormalFrameworkListQuery : Query<OrganisationFormalFrameworkListItem, OrganisationFormalFrameworkListItemFilter, OrganisationFormalFrameworkListQueryResult>
{
    private readonly OrganisationRegistryContext _context;
    private readonly IMemoryCaches _memoryCaches;
    private readonly IOrganisationRegistryConfiguration _configuration;
    private readonly IUser _user;
    private readonly Guid _organisationId;

    protected override ISorting Sorting => new OrganisationFormalFrameworkListSorting();

    protected override Expression<Func<OrganisationFormalFrameworkListItem, OrganisationFormalFrameworkListQueryResult>> Transformation =>
        x => new OrganisationFormalFrameworkListQueryResult(
            x.OrganisationFormalFrameworkId,
            x.FormalFrameworkId,
            x.FormalFrameworkName,
            x.ParentOrganisationId,
            x.ParentOrganisationName,
            x.ValidFrom,
            x.ValidTo,
            _memoryCaches.OvoNumbers[x.OrganisationId],
            _configuration,
            _user);

    public OrganisationFormalFrameworkListQuery(
        OrganisationRegistryContext context,
        IMemoryCaches memoryCaches,
        IOrganisationRegistryConfiguration configuration,
        IUser user,
        Guid organisationId)
    {
        _context = context;
        _memoryCaches = memoryCaches;
        _configuration = configuration;
        _user = user;
        _organisationId = organisationId;
    }

    protected override IQueryable<OrganisationFormalFrameworkListItem> Filter(FilteringHeader<OrganisationFormalFrameworkListItemFilter> filtering)
    {
        var organisationFormalFrameworks = _context.OrganisationFormalFrameworkList
            .AsQueryable()
            .Where(x => x.OrganisationId == _organisationId).AsQueryable();

        if (filtering.Filter is not { } filter)
            return organisationFormalFrameworks;

        if (filter.ActiveOnly)
            organisationFormalFrameworks = organisationFormalFrameworks.Where(x =>
                (!x.ValidFrom.HasValue || x.ValidFrom <= DateTime.Today) &&
                (!x.ValidTo.HasValue || x.ValidTo >= DateTime.Today));

        return organisationFormalFrameworks;
    }

    private class OrganisationFormalFrameworkListSorting : ISorting
    {
        public IEnumerable<string> SortableFields { get; } = new[]
        {
            nameof(OrganisationFormalFrameworkListItem.ParentOrganisationName),
            nameof(OrganisationFormalFrameworkListItem.FormalFrameworkName),
            nameof(OrganisationFormalFrameworkListItem.ValidFrom),
            nameof(OrganisationFormalFrameworkListItem.ValidTo),
        };

        public SortingHeader DefaultSortingHeader { get; } =
            new(nameof(OrganisationFormalFrameworkListItem.ParentOrganisationName), SortOrder.Ascending);
    }
}

public class OrganisationFormalFrameworkListItemFilter
{
    public bool ActiveOnly { get; set; }
}
