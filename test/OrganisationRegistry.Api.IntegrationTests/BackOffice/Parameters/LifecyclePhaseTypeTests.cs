namespace OrganisationRegistry.Api.IntegrationTests.BackOffice.Parameters;

using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using Backoffice.Parameters.LifecyclePhaseType.Requests;
using FluentAssertions;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class LifecyclePhaseTypeTests
{
    private readonly ApiFixture _apiFixture;

    public LifecyclePhaseTypeTests(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    /// <summary>
    /// special case, because only 1 lifecyclephasetype can be default by business rule
    /// </summary>
    [Fact]
    public async Task ParameterListTestLifecyclePhaseTypes()
    {
        const string baseRoute = ParametersTestData.LifecyclephasetypesRoute;

        var request1 = new CreateLifecyclePhaseTypeRequest { Id = _apiFixture.Fixture.Create<Guid>(), Name = _apiFixture.Fixture.Create<string>() };
        await ApiFixture.VerifyStatusCode(await _apiFixture.Create(baseRoute, request1), HttpStatusCode.Created);

        var request2 = new CreateLifecyclePhaseTypeRequest { Id = _apiFixture.Fixture.Create<Guid>(), Name = _apiFixture.Fixture.Create<string>() };
        await ApiFixture.VerifyStatusCode(await _apiFixture.Create(baseRoute, request2), HttpStatusCode.Created);

        var getResponse = await ApiFixture.Get(_apiFixture.HttpClient, $"{baseRoute}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var deserializedResponse = await ApiFixture.DeserializeAsList(getResponse);
        deserializedResponse.Should().NotBeNull();

        deserializedResponse.Should().HaveCountGreaterThanOrEqualTo(2);
    }
}
