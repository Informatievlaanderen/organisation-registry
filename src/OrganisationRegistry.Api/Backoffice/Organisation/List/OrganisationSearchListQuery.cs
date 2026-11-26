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
using OrganisationRegistry.Handling.Authorization;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Authorization.Restrictions;
using OrganisationRegistry.Infrastructure.Configuration;
using SqlServer.Infrastructure;
using SqlServer.Organisation;

public class OrganisationSearchListQueryResult
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

    /// <summary>
    /// Whether this organisation may be selected as a value in create/update
    /// commands (e.g. as parent organisation). Evaluated with
    /// <see cref="OrganisationPolicy"/> so restricted roles (DecentraalBeheerder,
    /// VlimpersBeheerder) are taken into account.
    /// </summary>
    public bool CanSelect { get; }

    public OrganisationSearchListQueryResult(
        Guid id,
        string ovoNumber,
        string name,
        string? shortName,
        string? parentOrganisation,
        Guid? parentOrganisationId,
        string? parentOrganisationOvoNumber,
        bool canSelect)
    {
        Id = id;
        OvoNumber = ovoNumber;
        Name = name;
        ShortName = shortName;
        ParentOrganisation = parentOrganisation;
        ParentOrganisationId = parentOrganisationId;
        ParentOrganisationOvoNumber = parentOrganisationOvoNumber;
        CanSelect = canSelect;
    }
}

/// <summary>
/// Het overzicht op de hoofdpagina (OR-3785). Gelijk aan <see cref="OrganisationListQuery"/>,
/// met één bijkomend filterveld: <c>code</c>, dat zoekt op OVO-nummer, KBO-nummer en
/// Orafin-sleutelwaarde. Leest uit het eigen read model <c>OrganisationSearchList</c>.
/// </summary>
public class OrganisationSearchListQuery : Query<OrganisationSearchListItem, OrganisationSearchListItemFilter, OrganisationSearchListQueryResult>
{
    private readonly OrganisationRegistryContext _context;
    private readonly IUser _user;
    private readonly IOrganisationRegistryConfiguration _configuration;

    protected override ISorting Sorting => new OrganisationSearchListSorting();

    protected override Expression<Func<OrganisationSearchListItem, OrganisationSearchListQueryResult>> Transformation =>
        x => new OrganisationSearchListQueryResult(
            x.OrganisationId,
            x.OvoNumber,
            x.Name,
            x.ShortName,
            x.ParentOrganisation,
            x.ParentOrganisationId,
            x.ParentOrganisationOvoNumber,
            // TODO: OR-3114 CanSelect should also respect Vlimpers management restrictions.
            // Zie OrganisationListQuery voor de volledige toelichting.
            new OrganisationPolicy(Permission.CanManageOrganisation, x.OvoNumber, false).Check(_user).IsSuccessful);

    public OrganisationSearchListQuery(
        OrganisationRegistryContext context,
        IUser user,
        IOrganisationRegistryConfiguration configuration)
    {
        _context = context;
        _user = user;
        _configuration = configuration;
    }

    protected override IQueryable<OrganisationSearchListItem> Filter(FilteringHeader<OrganisationSearchListItemFilter> filtering)
    {
        var organisations = _context.OrganisationSearchList.AsAsyncQueryable();

        if (filtering.Filter is not { } filter)
            return organisations.Where(x => x.FormalFrameworkId == null);

        if (filter.Name is { } name && name.IsNotEmptyOrWhiteSpace())
            organisations = organisations.Where(x => x.Name.Contains(name) || (x.ShortName != null && x.ShortName.Contains(name)));

        if (filter.OvoNumber is { } ovoNumber && ovoNumber.IsNotEmptyOrWhiteSpace())
            organisations = organisations.Where(x => x.OvoNumber.Contains(ovoNumber));

        if (filter.Code is { } code && OrganisationCodeNormalizer.HasSearchableValue(code))
            organisations = FilterOnCode(organisations, code);

        if (filter.ActiveOnly)
            organisations = organisations.Where(x =>
                (!x.ValidFrom.HasValue || x.ValidFrom <= DateTime.Today) &&
                (!x.ValidTo.HasValue || x.ValidTo >= DateTime.Today));

        organisations = filter.FormalFrameworkId.IsEmptyGuid()
            ? organisations.Where(x => x.FormalFrameworkId == null)
            : organisations.Where(x => x.FormalFrameworkId == filter.FormalFrameworkId);

        if (filter.ActiveOnly && !filter.FormalFrameworkId.IsEmptyGuid())
            organisations = organisations.Where(x =>
                x.FormalFrameworkValidities.Any(y =>
                    (!y.ValidFrom.HasValue || y.ValidFrom <= DateTime.Today) &&
                    (!y.ValidTo.HasValue || y.ValidTo >= DateTime.Today)));

        if (!filter.OrganisationClassificationId.IsEmptyGuid())
            organisations = organisations.Where(x =>
                x.OrganisationSearchClassificationValidities.Any(y =>
                    y.OrganisationClassificationId == filter.OrganisationClassificationId &&
                    (!filter.ActiveOnly ||
                     ((!y.ValidFrom.HasValue || y.ValidFrom <= DateTime.Today) &&
                      (!y.ValidTo.HasValue || y.ValidTo >= DateTime.Today)))));

        if (!filter.OrganisationClassificationTypeId.IsEmptyGuid())
            organisations = organisations.Where(x =>
                x.OrganisationSearchClassificationValidities.Any(y =>
                    y.OrganisationClassificationTypeId == filter.OrganisationClassificationTypeId &&
                    (!filter.ActiveOnly ||
                     ((!y.ValidFrom.HasValue || y.ValidFrom <= DateTime.Today) &&
                      (!y.ValidTo.HasValue || y.ValidTo >= DateTime.Today)))));

        if (filter.AuthorizedOnly && _user.OrganisationIds.Any())
        {
            // Users with explicit organisation restrictions (e.g. DecentraalBeheerder)
            // only see organisations in their scope. Users without restrictions see
            // everything and rely on per-item CanSelect evaluated in the projection.
            organisations = organisations.Where(x => _user.OrganisationIds.Contains(x.OrganisationId));
        }

        return organisations;
    }

    /// <summary>
    /// Zoekt op OVO-nummer, KBO-nummer en Orafin-sleutelwaarde in één veld.
    /// <br />
    /// Het sleuteltype dat als Orafin geldt, komt uit de configuratie en wordt hier — en niet in
    /// de projectie — toegepast: wijzigt die instelling, dan volgen de resultaten onmiddellijk,
    /// zonder herbouw van het read model.
    /// <br />
    /// De geldigheidsperiode van een sleutel wordt niet getoetst: ook vervallen sleutels geven
    /// een match, zodat het zoekresultaat datum-onafhankelijk is.
    /// </summary>
    private IQueryable<OrganisationSearchListItem> FilterOnCode(
        IQueryable<OrganisationSearchListItem> organisations,
        string code)
    {
        var candidates = OrganisationCodeNormalizer.Candidates(code);
        var orafinKeyTypeId = _configuration.OrafinKeyTypeId;

        var matchingOrganisationIds = _context.OrganisationSearchCodeList
            .Where(ContainsAnyCandidate(candidates))
            .Where(x => x.KeyTypeId == null || x.KeyTypeId == orafinKeyTypeId)
            .Select(x => x.OrganisationId);

        return organisations.Where(x => matchingOrganisationIds.Contains(x.OrganisationId));
    }

    // Deelmatch: LIKE '%kandidaat%', met een OR per kandidaat (er zijn er hoogstens twee).
    // EF kan `candidates.Any(c => waarde.Contains(c))` niet vertalen, dus de OR-keten wordt hier opgebouwd.
    // ponytail: geen index-seek mogelijk bij een leidende wildcard; zie T035 voor de performantiecheck.
    private static Expression<Func<OrganisationSearchCodeListItem, bool>> ContainsAnyCandidate(
        IEnumerable<string> candidates)
    {
        var item = Expression.Parameter(typeof(OrganisationSearchCodeListItem), "x");
        var value = Expression.Property(item, nameof(OrganisationSearchCodeListItem.NormalizedValue));
        var contains = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!;

        var body = candidates
            .Select(candidate => (Expression)Expression.Call(value, contains, AsParameter(candidate)))
            .Aggregate(Expression.OrElse);

        return Expression.Lambda<Func<OrganisationSearchCodeListItem, bool>>(body, item);
    }

    // Een member-access op een constante wordt door EF als SQL-parameter behandeld; een kale
    // Expression.Constant zou als literal in de SQL (en in de query-cache-sleutel) terechtkomen,
    // waardoor elke nieuwe zoekterm een nieuwe query compileert.
    private static Expression AsParameter(string candidate)
        => Expression.Property(Expression.Constant(new SearchTerm(candidate)), nameof(SearchTerm.Value));

    private sealed record SearchTerm(string Value);

    private class OrganisationSearchListSorting : ISorting
    {
        public IEnumerable<string> SortableFields { get; } = new[]
        {
            nameof(OrganisationSearchListItem.Name),
            nameof(OrganisationSearchListItem.ParentOrganisation),
            nameof(OrganisationSearchListItem.OvoNumber),
        };

        public SortingHeader DefaultSortingHeader { get; } = new(nameof(OrganisationSearchListItem.Name), SortOrder.Ascending);
    }
}

public class OrganisationSearchListItemFilter
{
    public string? Name { get; set; }
    public string? OvoNumber { get; set; }

    /// <summary>OVO-nummer, KBO-nummer of Orafin-code; deelmatch (substring) op de genormaliseerde waarde.</summary>
    public string? Code { get; set; }

    public Guid? FormalFrameworkId { get; set; }
    public Guid? OrganisationClassificationId { get; set; }
    public Guid? OrganisationClassificationTypeId { get; set; }
    public bool ActiveOnly { get; set; }
    public bool AuthorizedOnly { get; set; }
}
