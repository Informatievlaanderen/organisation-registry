namespace OrganisationRegistry.Api.Backoffice.Organisation.List;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
using Infrastructure;
using Infrastructure.Search;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Sorting;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.Infrastructure.Authorization;
using Security;
using SqlServer.Infrastructure;
using SqlServer.Organisation;

public class OrganisationListQueryResult
{
    [ExcludeFromCsv]
    public Guid Id { get; }

    [DisplayName("OVO-nummer")]
    public string OvoNumber { get; }

    [DisplayName("Naam")]
    public string Name { get; }

    [DisplayName("Korte naam")]
    public string? ShortName { get; }

    [DisplayName("OVO-nummer moeder entiteit")]
    public string? ParentOrganisationOvoNumber { get; }

    [DisplayName("Moeder entiteit")]
    public string? ParentOrganisation { get; }

    [ExcludeFromCsv]
    public Guid? ParentOrganisationId { get; }

    public OrganisationListQueryResult(
        Guid id,
        string ovoNumber,
        string name,
        string? shortName,
        string? parentOrganisation,
        Guid? parentOrganisationId,
        string? parentOrganisationOvoNumber)
    {
        Id = id;
        OvoNumber = ovoNumber;
        Name = name;
        ShortName = shortName;
        ParentOrganisation = parentOrganisation;
        ParentOrganisationId = parentOrganisationId;
        ParentOrganisationOvoNumber = parentOrganisationOvoNumber;
    }
}

public class OrganisationListQuery : Query<OrganisationListItem, OrganisationListItemFilter, OrganisationListQueryResult>
{
    private readonly OrganisationRegistryContext _context;
    private readonly SecurityInformation _securityInformation;

    protected override ISorting Sorting => new OrganisationListSorting();

    protected override Expression<Func<OrganisationListItem, OrganisationListQueryResult>> Transformation =>
        x => new OrganisationListQueryResult(
            x.OrganisationId,
            x.OvoNumber,
            x.Name,
            x.ShortName,
            x.ParentOrganisation,
            x.ParentOrganisationId,
            x.ParentOrganisationOvoNumber);

    public OrganisationListQuery(OrganisationRegistryContext context, SecurityInformation securityInformation)
    {
        _context = context;
        _securityInformation = securityInformation;
    }

    protected override IQueryable<OrganisationListItem> Filter(FilteringHeader<OrganisationListItemFilter> filtering)
    {
        var organisations = _context.OrganisationList.AsAsyncQueryable();

        if (filtering.Filter is not { } filter)
            return organisations.Where(x => x.FormalFrameworkId == null);

        if (filter.Name is { } name && name.IsNotEmptyOrWhiteSpace())
            organisations = organisations.Where(x => x.Name.Contains(name) || (x.ShortName != null && x.ShortName.Contains(name)));

        if (filter.OvoNumber is { } ovoNumber && ovoNumber.IsNotEmptyOrWhiteSpace())
            organisations = organisations.Where(x => x.OvoNumber.Contains(ovoNumber));

        if (filter.ActiveOnly)
            organisations = organisations.Where(x =>
                (!x.ValidFrom.HasValue || x.ValidFrom <= DateTime.Today) &&
                (!x.ValidTo.HasValue || x.ValidTo >= DateTime.Today));

        organisations = filter.FormalFrameworkId.IsEmptyGuid() ?
            organisations.Where(x => x.FormalFrameworkId == null) :
            organisations.Where(x => x.FormalFrameworkId == filter.FormalFrameworkId);

        if (filter.ActiveOnly && !filter.FormalFrameworkId.IsEmptyGuid())
            organisations = organisations.Where(x =>
                x.FormalFrameworkValidities.Any(y =>
                    (!y.ValidFrom.HasValue || y.ValidFrom <= DateTime.Today) &&
                    (!y.ValidTo.HasValue || y.ValidTo >= DateTime.Today)));

        if (!filter.OrganisationClassificationId.IsEmptyGuid())
            organisations = organisations.Where(x =>
                x.OrganisationClassificationValidities.Any(y =>
                    y.OrganisationClassificationId == filter.OrganisationClassificationId &&
                    (!filter.ActiveOnly ||
                     ((!y.ValidFrom.HasValue || y.ValidFrom <= DateTime.Today) &&
                      (!y.ValidTo.HasValue || y.ValidTo >= DateTime.Today)))));

        if (!filter.OrganisationClassificationTypeId.IsEmptyGuid())
            organisations = organisations.Where(x =>
                x.OrganisationClassificationValidities.Any(y =>
                    y.OrganisationClassificationTypeId == filter.OrganisationClassificationTypeId &&
                    (!filter.ActiveOnly ||
                     ((!y.ValidFrom.HasValue || y.ValidFrom <= DateTime.Today) &&
                      (!y.ValidTo.HasValue || y.ValidTo >= DateTime.Today)))));

        if (filter.AuthorizedOnly)
        {
            if (!_securityInformation.Roles.Any(role => role is Role.AlgemeenBeheerder or Role.CjmBeheerder))
                organisations = organisations.Where(x => _securityInformation.OvoNumbers.Contains(x.OvoNumber));
        }

        return organisations;
    }

    private class OrganisationListSorting : ISorting
    {
        public IEnumerable<string> SortableFields { get; } = new[]
        {
            nameof(OrganisationListItem.Name),
            nameof(OrganisationListItem.ParentOrganisation),
            nameof(OrganisationListItem.OvoNumber),
        };

        public SortingHeader DefaultSortingHeader { get; } = new(nameof(OrganisationListItem.Name), SortOrder.Ascending);
    }
}

public class OrganisationListItemFilter
{
    public string? Name { get; set; }
    public string? OvoNumber { get; set; }
    public Guid? FormalFrameworkId { get; set; }
    public Guid? OrganisationClassificationId { get; set; }
    public Guid? OrganisationClassificationTypeId { get; set; }
    public bool ActiveOnly { get; set; }
    public bool AuthorizedOnly { get; set; }
}
