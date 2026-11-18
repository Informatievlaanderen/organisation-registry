namespace OrganisationRegistry.Api.IntegrationTests.BackOffice.Organisation;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using Backoffice.Organisation.Key;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

/// <summary>
/// Het overzicht op de hoofdpagina (OR-3785): <c>GET /v1/organisations/overview</c> gedraagt zich
/// als de bestaande organisatielijst, met één bijkomend filterveld <c>code</c> dat zoekt op
/// OVO-nummer, KBO-nummer en Orafin-sleutelwaarde.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class OrganisationOverviewTests
{
    private const string OverviewRoute = "/v1/organisations/overview";
    private const string ListRoute = "/v1/organisations";

    private readonly ApiFixture _apiFixture;

    public OrganisationOverviewTests(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task ReturnsTheSameShapeAsTheOrganisationList()
    {
        var overview = await GetFiltered(OverviewRoute, new { });
        var list = await GetFiltered(ListRoute, new { });

        overview.Should().NotBeEmpty();

        var overviewKeys = overview.First().Keys.OrderBy(key => key);
        var listKeys = list.First().Keys.OrderBy(key => key);

        overviewKeys.Should().BeEquivalentTo(listKeys, "de kolommen van het overzicht blijven ongewijzigd");
    }

    [Fact]
    public async Task FindsAnOrganisationByItsOvoNumber()
    {
        var (_, ovoNumber) = await GivenAnOrganisation();

        var results = await GetFiltered(OverviewRoute, new { code = ovoNumber });

        results.Should().ContainSingle()
            .Which["ovoNumber"].ToString().Should().Be(ovoNumber);
    }

    [Fact]
    public async Task FindsAnOrganisationByItsOrafinKey()
    {
        var (organisationId, ovoNumber) = await GivenAnOrganisation();
        var orafinCode = $"ORAFIN-{_apiFixture.Fixture.Create<int>()}";

        await GivenAnOrafinKey(organisationId, orafinCode);

        var results = await GetFiltered(OverviewRoute, new { code = orafinCode });

        results.Should().ContainSingle()
            .Which["ovoNumber"].ToString().Should().Be(ovoNumber);
    }

    /// <summary>De geldigheidsperiode van een sleutel speelt geen rol (FR-005b).</summary>
    [Fact]
    public async Task FindsAnOrganisationByAnExpiredOrafinKey()
    {
        var (organisationId, ovoNumber) = await GivenAnOrganisation();
        var orafinCode = $"VERVALLEN-{_apiFixture.Fixture.Create<int>()}";

        await GivenAnOrafinKey(
            organisationId,
            orafinCode,
            validFrom: new DateTime(2019, 1, 1),
            validTo: new DateTime(2020, 12, 31));

        var results = await GetFiltered(OverviewRoute, new { code = orafinCode });

        results.Should().ContainSingle()
            .Which["ovoNumber"].ToString().Should().Be(ovoNumber);
    }

    [Fact]
    public async Task FindsNothingForAnUnknownCode()
    {
        var results = await GetFiltered(OverviewRoute, new { code = $"ONBEKEND-{_apiFixture.Fixture.Create<int>()}" });

        results.Should().BeEmpty();
    }

    [Fact]
    public async Task DoesNotMatchAPartialCode()
    {
        var (_, ovoNumber) = await GivenAnOrganisation();

        var results = await GetFiltered(OverviewRoute, new { code = ovoNumber[..6] });

        results.Should().BeEmpty();
    }

    [Fact]
    public async Task LeavesTheOrganisationListUnchanged()
    {
        var (_, ovoNumber) = await GivenAnOrganisation();

        // De bestaande lijst kent het veld code niet en negeert het.
        var results = await GetFiltered(ListRoute, new { code = ovoNumber });

        results.Should().NotBeEmpty();
        results.Should().HaveCountGreaterThan(1, "zonder werkend code-filter blijft de volledige lijst over");
    }

    [Fact]
    public async Task AcceptsACodeAsTheOnlyFilter()
    {
        var (_, ovoNumber) = await GivenAnOrganisation();

        var results = await GetFiltered(OverviewRoute, new { code = ovoNumber });

        results.Should().ContainSingle();
    }

    [Fact]
    public async Task LeavesTheOtherFiltersUntouchedWhenCodeIsEmpty()
    {
        var (_, ovoNumber) = await GivenAnOrganisation();

        var byOvoNumberOnly = await GetFiltered(OverviewRoute, new { ovoNumber });
        var byOvoNumberWithEmptyCode = await GetFiltered(OverviewRoute, new { ovoNumber, code = "" });

        byOvoNumberOnly.Should().NotBeEmpty();
        byOvoNumberWithEmptyCode.Should().HaveCount(byOvoNumberOnly.Length);
    }

    private async Task<(Guid organisationId, string ovoNumber)> GivenAnOrganisation()
    {
        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());

        using var response = await ApiFixture.Get(_apiFixture.HttpClient, $"{ListRoute}/{organisationId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var organisation = await ApiFixture.Deserialize(response);

        return (organisationId, organisation["ovoNumber"].ToString()!);
    }

    private async Task GivenAnOrafinKey(
        Guid organisationId,
        string keyValue,
        DateTime? validFrom = null,
        DateTime? validTo = null)
    {
        var orafinKeyTypeId = _apiFixture.Configuration.OrafinKeyTypeId;
        await _apiFixture.Create.KeyType(orafinKeyTypeId);

        using var response = await ApiFixture.Post(
            _apiFixture.HttpClient,
            $"{ListRoute}/{organisationId}/keys",
            new AddOrganisationKeyRequest
            {
                OrganisationKeyId = _apiFixture.Fixture.Create<Guid>(),
                KeyTypeId = orafinKeyTypeId,
                KeyValue = keyValue,
                ValidFrom = validFrom,
                ValidTo = validTo,
            });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    private async Task<Dictionary<string, object>[]> GetFiltered(string route, object filter)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, route);
        request.Headers.Add("x-filtering", JsonConvert.SerializeObject(filter));

        using var response = await _apiFixture.HttpClient.SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        return await ApiFixture.DeserializeAsList(response);
    }
}
