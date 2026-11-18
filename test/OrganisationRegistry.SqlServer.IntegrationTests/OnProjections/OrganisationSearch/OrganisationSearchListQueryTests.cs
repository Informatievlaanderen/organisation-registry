namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections.OrganisationSearch;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Backoffice.Organisation.List;
using Api.Infrastructure.Search.Filtering;
using Api.Infrastructure.Search.Pagination;
using Api.Infrastructure.Search.Sorting;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.Organisation.Events;
using Tests.Shared;
using Tests.Shared.Stubs;
using TestBases;
using Xunit;

/// <summary>
/// Het filterveld <c>code</c> op de hoofdpagina (OR-3785): één invoerveld dat zoekt op
/// OVO-nummer, KBO-nummer en Orafin-sleutelwaarde.
/// </summary>
[Collection(SqlServerTestsCollection.Name)]
public class OrganisationSearchListQueryTests : ListViewTestBase
{
    private static readonly Guid OrafinKeyTypeId = Guid.NewGuid();
    private static readonly Guid OtherKeyTypeId = Guid.NewGuid();

    private readonly Guid _organisationId = Guid.NewGuid();
    private readonly Guid _otherOrganisationId = Guid.NewGuid();

    private const string OvoNumber = "OVO000021";
    private const string KboNumber = "0123.456.789";
    private const string OrafinCode = "ORAFIN-1";

    private async Task GivenAnOrganisationWithAllCodes()
        => await HandleEvents(
            new OrganisationCreatedFromKbo(
                _organisationId,
                KboNumber,
                "Organisatie",
                OvoNumber,
                "ORG",
                null,
                null,
                new List<Purpose>(),
                false,
                null,
                null,
                null,
                null),
            // Een vervallen Orafin-sleutel: die moet nog steeds gevonden worden.
            new OrganisationKeyAdded(
                _organisationId,
                Guid.NewGuid(),
                OrafinKeyTypeId,
                "Orafin",
                OrafinCode,
                new DateTime(2019, 1, 1),
                new DateTime(2020, 12, 31)),
            // Een sleutel van een ander type: die mag niet doorzoekbaar zijn.
            new OrganisationKeyAdded(
                _organisationId,
                Guid.NewGuid(),
                OtherKeyTypeId,
                "INR",
                "INR-7",
                null,
                null),
            Created(_otherOrganisationId, "OVO000022"));

    [Theory]
    [InlineData(OvoNumber)]
    [InlineData("ovo000021")]
    [InlineData("  OVO000021 ")]
    public async Task FindsAnOrganisationByItsOvoNumber(string code)
    {
        await GivenAnOrganisationWithAllCodes();

        var results = await Search(code);

        results.Should().ContainSingle().Which.OvoNumber.Should().Be(OvoNumber);
    }

    [Theory]
    [InlineData("0123.456.789")]
    [InlineData("0123456789")]
    [InlineData("BE0123456789")]
    [InlineData("123456789")]
    public async Task FindsAnOrganisationByEveryKboNotation(string code)
    {
        await GivenAnOrganisationWithAllCodes();

        var results = await Search(code);

        results.Should().ContainSingle().Which.OvoNumber.Should().Be(OvoNumber);
    }

    [Theory]
    [InlineData(OrafinCode)]
    [InlineData("orafin1")]
    public async Task FindsAnOrganisationByAnExpiredOrafinKey(string code)
    {
        await GivenAnOrganisationWithAllCodes();

        var results = await Search(code);

        results.Should().ContainSingle().Which.OvoNumber.Should().Be(OvoNumber);
    }

    [Fact]
    public async Task DoesNotFindOrganisationsByAKeyOfAnotherKeyType()
    {
        await GivenAnOrganisationWithAllCodes();

        var results = await Search("INR-7");

        results.Should().BeEmpty();
    }

    [Fact]
    public async Task FindsNothingForAnUnknownCode()
    {
        await GivenAnOrganisationWithAllCodes();

        var results = await Search("OVO999999");

        results.Should().BeEmpty();
    }

    [Fact]
    public async Task DoesNotMatchPartialCodes()
    {
        await GivenAnOrganisationWithAllCodes();

        var results = await Search("OVO0000");

        results.Should().BeEmpty();
    }

    [Fact]
    public async Task IgnoresAnEmptyCode()
    {
        await GivenAnOrganisationWithAllCodes();

        var results = await Search("   ");

        results.Should().HaveCount(2, "zonder code-filter blijven alle organisaties over");
    }

    /// <summary>
    /// Het Orafin-sleuteltype komt uit de configuratie en wordt bij het zoeken toegepast: met een
    /// andere instelling geeft dezelfde data een ander resultaat, zonder herbouw van de projectie.
    /// </summary>
    [Fact]
    public async Task FollowsTheConfiguredOrafinKeyType()
    {
        await GivenAnOrganisationWithAllCodes();

        var withOrafinAsConfigured = await Search(OrafinCode, OrafinKeyTypeId);
        var withAnotherKeyTypeConfigured = await Search(OrafinCode, OtherKeyTypeId);
        var withTheOtherKeyNowSearchable = await Search("INR-7", OtherKeyTypeId);

        withOrafinAsConfigured.Should().ContainSingle();
        withAnotherKeyTypeConfigured.Should().BeEmpty();
        withTheOtherKeyNowSearchable.Should().ContainSingle();
    }

    [Fact]
    public async Task ReturnsAnOrganisationOnlyOnceWhenSeveralOfItsCodesMatch()
    {
        var organisationId = Guid.NewGuid();

        await HandleEvents(
            Created(organisationId, "OVO000023"),
            // Twee Orafin-sleutels met dezelfde waarde, bijvoorbeeld na een hernieuwing.
            new OrganisationKeyAdded(organisationId, Guid.NewGuid(), OrafinKeyTypeId, "Orafin", "DUBBEL", null, null),
            new OrganisationKeyAdded(organisationId, Guid.NewGuid(), OrafinKeyTypeId, "Orafin", "dubbel", null, null));

        var results = await Search("DUBBEL");

        results.Should().ContainSingle();
    }

    /// <summary>
    /// De filter aanvaardt een zoekopdracht met enkel Code ingevuld; de overige velden zijn
    /// geen voorwaarde.
    /// </summary>
    [Fact]
    public async Task AcceptsACodeAsTheOnlyCriterion()
    {
        await GivenAnOrganisationWithAllCodes();

        var results = await Search(new OrganisationSearchListItemFilter { Code = OvoNumber });

        results.Should().ContainSingle().Which.OvoNumber.Should().Be(OvoNumber);
    }

    [Fact]
    public async Task CombinesCodeWithNameAsAnIntersection()
    {
        await GivenAnOrganisationWithAllCodes();

        var matchingBoth = await Search(new OrganisationSearchListItemFilter { Code = OvoNumber, Name = "Organisatie" });
        var matchingCodeOnly = await Search(new OrganisationSearchListItemFilter { Code = OvoNumber, Name = "Bestaat niet" });

        matchingBoth.Should().ContainSingle();
        matchingCodeOnly.Should().BeEmpty();
    }

    [Fact]
    public async Task CombinesCodeWithActiveOnly()
    {
        await GivenAnOrganisationWithAllCodes();
        await HandleEvents(new OrganisationValidityUpdated(_organisationId, new DateTime(2019, 1, 1), new DateTime(2020, 12, 31)));

        var activeOnly = await Search(new OrganisationSearchListItemFilter { Code = OvoNumber, ActiveOnly = true });
        var includingInactive = await Search(new OrganisationSearchListItemFilter { Code = OvoNumber, ActiveOnly = false });

        activeOnly.Should().BeEmpty("de organisatie is niet meer actief");
        includingInactive.Should().ContainSingle();
    }

    [Fact]
    public async Task CombinesCodeWithClassification()
    {
        var organisationClassificationId = Guid.NewGuid();
        var organisationClassificationTypeId = Guid.NewGuid();

        await GivenAnOrganisationWithAllCodes();
        await HandleEvents(
            new OrganisationOrganisationClassificationAdded(
                _organisationId,
                Guid.NewGuid(),
                organisationClassificationTypeId,
                "Classificatietype",
                organisationClassificationId,
                "Classificatie",
                null,
                null));

        var matching = await Search(
            new OrganisationSearchListItemFilter
            {
                Code = OvoNumber,
                OrganisationClassificationId = organisationClassificationId,
            });

        var notMatching = await Search(
            new OrganisationSearchListItemFilter
            {
                Code = OvoNumber,
                OrganisationClassificationId = Guid.NewGuid(),
            });

        matching.Should().ContainSingle();
        notMatching.Should().BeEmpty();
    }

    /// <summary>
    /// De geldigheidsperiode van een sleutel speelt geen rol: zowel een vervallen als een
    /// toekomstige sleutel geeft een match. Daardoor bevat het code-filter geen
    /// datumvergelijking en is het resultaat datum-onafhankelijk.
    /// </summary>
    [Fact]
    public async Task FindsKeysRegardlessOfTheirValidityPeriod()
    {
        var organisationId = Guid.NewGuid();

        await HandleEvents(
            Created(organisationId, "OVO000024"),
            new OrganisationKeyAdded(organisationId, Guid.NewGuid(), OrafinKeyTypeId, "Orafin", "VERVALLEN", new DateTime(2010, 1, 1), new DateTime(2011, 1, 1)),
            new OrganisationKeyAdded(organisationId, Guid.NewGuid(), OrafinKeyTypeId, "Orafin", "TOEKOMST", new DateTime(2099, 1, 1), new DateTime(2099, 12, 31)));

        (await Search("VERVALLEN")).Should().ContainSingle();
        (await Search("TOEKOMST")).Should().ContainSingle();
    }

    private async Task<List<OrganisationSearchListQueryResult>> Search(string code, Guid? orafinKeyTypeId = null)
        => await Search(new OrganisationSearchListItemFilter { Code = code }, orafinKeyTypeId);

    private async Task<List<OrganisationSearchListQueryResult>> Search(
        OrganisationSearchListItemFilter filter,
        Guid? orafinKeyTypeId = null)
    {
        await using var context = Context;

        var configuration = new OrganisationRegistryConfigurationStub
        {
            OrafinKeyTypeId = orafinKeyTypeId ?? OrafinKeyTypeId,
        };

        var query = new OrganisationSearchListQuery(
            context,
            new UserBuilder().Build(),
            configuration);

        var paged = query.Fetch(
            new FilteringHeader<OrganisationSearchListItemFilter>(filter),
            new SortingHeader(nameof(OrganisationSearchListQueryResult.Name), SortOrder.Ascending),
            new PaginationRequest(1, 100));

        return await paged.Items.ToListAsync();
    }

    private static OrganisationCreated Created(Guid organisationId, string ovoNumber)
        => new(
            organisationId,
            "Andere organisatie",
            ovoNumber,
            "AND",
            null,
            null,
            new List<Purpose>(),
            false,
            null,
            null,
            null,
            null);
}
