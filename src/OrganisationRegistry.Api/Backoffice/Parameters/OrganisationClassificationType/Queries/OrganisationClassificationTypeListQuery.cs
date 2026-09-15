namespace OrganisationRegistry.Api.Backoffice.Parameters.OrganisationClassificationType.Queries;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
using Infrastructure.Search;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Sorting;
using OrganisationRegistry.Infrastructure.Configuration;
using Parameters;
using SqlServer.Infrastructure;
using SqlServer.OrganisationClassificationType;

public class OrganisationClassificationTypeListQuery: Query<OrganisationClassificationTypeListItem, OrganisationClassificationTypeListItem, OrganisationClassificationTypeListItemResult>
{
    private readonly OrganisationRegistryContext _context;
    private readonly IOrganisationRegistryConfiguration _configuration;
    private readonly Func<Guid, bool> _isAuthorizedForOrganisationClassificationType;

    protected override ISorting Sorting => new OrganisationClassificationTypeListSorting();

    public OrganisationClassificationTypeListQuery(
        OrganisationRegistryContext context,
        IOrganisationRegistryConfiguration configuration,
        Func<Guid, bool> isAuthorizedForOrganisationClassificationType)
    {
        _context = context;
        _configuration = configuration;
        _isAuthorizedForOrganisationClassificationType = isAuthorizedForOrganisationClassificationType;
    }

    protected override
        Expression<Func<OrganisationClassificationTypeListItem, OrganisationClassificationTypeListItemResult>>
        Transformation =>
        x => new OrganisationClassificationTypeListItemResult(
            x.Id,
            x.Name,
            _configuration.Kbo.KboV2LegalFormOrganisationClassificationTypeId,
            _isAuthorizedForOrganisationClassificationType);

    protected override IQueryable<OrganisationClassificationTypeListItem> Filter(FilteringHeader<OrganisationClassificationTypeListItem> filtering)
    {
        var organisationClassificationTypes = _context.OrganisationClassificationTypeList.AsQueryable();

        if (filtering.Filter is not { } filter)
            return organisationClassificationTypes;

        if (!filter.Name.IsNullOrWhiteSpace())
            organisationClassificationTypes = organisationClassificationTypes.Where(x => x.Name.Contains(filter.Name));

        return organisationClassificationTypes;
    }

    private class OrganisationClassificationTypeListSorting : ISorting
    {
        public IEnumerable<string> SortableFields { get; } = new[]
        {
            nameof(OrganisationClassificationTypeListItem.Name),
        };

        public SortingHeader DefaultSortingHeader { get; } =
            new(nameof(OrganisationClassificationTypeListItem.Name), SortOrder.Ascending);
    }
}

public class OrganisationClassificationTypeListItemResult
{
    public OrganisationClassificationTypeListItemResult(
        Guid id,
        string name,
        Guid kboV2LegalFormOrganisationClassificationTypeId,
        Func<Guid, bool> isAuthorizedForOrganisationClassificationType)
    {
        Id = id;
        Name = name;
        UserPermitted = id != kboV2LegalFormOrganisationClassificationTypeId;
        Permissions = new ResourceSelectPermissions(
            UserPermitted && isAuthorizedForOrganisationClassificationType(id));
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool UserPermitted { get; set; }
    public ResourceSelectPermissions Permissions { get; set; }
}
