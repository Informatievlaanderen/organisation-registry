namespace OrganisationRegistry.Api.IntegrationTests.BackOffice.Organisation;

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using Backoffice.Organisation.Regulation;
using FluentAssertions;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class OrganisationRegulationTests
{
    private readonly ApiFixture _apiFixture;
    private readonly OrganisationHelpers _helpers;

    public OrganisationRegulationTests(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
        _helpers = new OrganisationHelpers(_apiFixture);
    }

    [Fact]
    public async Task TestOrganisationRegulations()
    {
        var organisationId = _apiFixture.Fixture.Create<Guid>();
        var route = $"/v1/organisations/{organisationId}/regulations";
        await _helpers.CreateOrganisation(organisationId, _apiFixture.Fixture.Create<string>());

        var getResponse = await ApiFixture.Get(_apiFixture.HttpClient, $"{route}/{_apiFixture.Fixture.Create<Guid>()}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        // CREATE
        await _apiFixture.CreateWithInvalidDataAndVerifyBadRequest(route);

        var id = await CreateAndVerify(route);

        // UPDATE
        await UpdateAndVerify(route, id);

        await _apiFixture.UpdateWithInvalidDataAndVerifyBadRequest(route, id);

        // LIST
        await GetListAndVerify(route);
    }

    private async Task<Guid> CreateAndVerify(string baseRoute)
    {
        var regulationThemeId = await _helpers.CreateRegulationTheme();
        var regulationSubThemeId = await _helpers.CreateRegulationSubTheme(regulationThemeId);
        var today = _apiFixture.Fixture.Create<DateTime>();

        var id = _apiFixture.Fixture.Create<Guid>();
        var body = new AddOrganisationRegulationRequest
        {
            OrganisationRegulationId = id,
            Date = _apiFixture.Fixture.Create<DateTime>(),
            Name = _apiFixture.Fixture.Create<string>(),
            Description = _apiFixture.Fixture.Create<string>(),
            Url = "www.yahoo.com",
            DescriptionRendered = _apiFixture.Fixture.Create<string>(),
            WorkRulesUrl = "http://www.yahoo.com/rules.pdf",
            RegulationThemeId = regulationThemeId,
            RegulationSubThemeId = regulationSubThemeId,
            ValidFrom = today.AddDays(-10),
            ValidTo = today.AddDays(10),
        };

        await _apiFixture.CreateAndVerify(baseRoute, body, VerifyResult);

        return id;
    }

    private async Task UpdateAndVerify(string baseRoute, Guid id)
    {
        var regulationThemeId = await _helpers.CreateRegulationTheme();
        var regulationSubThemeId = await _helpers.CreateRegulationSubTheme(regulationThemeId);
        var today = _apiFixture.Fixture.Create<DateTime>();

        var body = new UpdateOrganisationRegulationRequest
        {
            OrganisationRegulationId = id,
            Date = _apiFixture.Fixture.Create<DateTime>(),
            Name = _apiFixture.Fixture.Create<string>(),
            Description = _apiFixture.Fixture.Create<string>(),
            Url = "www.google.com",
            DescriptionRendered = _apiFixture.Fixture.Create<string>(),
            WorkRulesUrl = "http://www.google.com/rules.pdf",
            RegulationThemeId = regulationThemeId,
            RegulationSubThemeId = regulationSubThemeId,
            ValidFrom = today.AddDays(-10),
            ValidTo = today.AddDays(10),
        };

        await _apiFixture.UpdateAndVerify(baseRoute, id, body, VerifyResult);
    }

    private static void VerifyResult(IReadOnlyDictionary<string, object> responseBody, dynamic body)
    {
        responseBody["name"].Should().Be(body.Name.ToString());
        responseBody["description"].Should().Be(body.Description.ToString());
        responseBody["url"].Should().Be(body.Url.ToString());
        responseBody["descriptionRendered"].Should().Be(body.DescriptionRendered.ToString());
        responseBody["workRulesUrl"].Should().Be(body.WorkRulesUrl.ToString());
        responseBody["regulationThemeId"].Should().Be(body.RegulationThemeId.ToString());
        responseBody["regulationSubThemeId"].Should().Be(body.RegulationSubThemeId.ToString());
        responseBody["date"].Should().Be(body.Date?.Date);
        responseBody["validFrom"].Should().Be(body.ValidFrom?.Date);
        responseBody["validTo"].Should().Be(body.ValidTo?.Date);
    }

    private async Task GetListAndVerify(string route)
    {
        await CreateAndVerify(route);
        await CreateAndVerify(route);

        await _apiFixture.GetListAndVerify(route);
    }
}
