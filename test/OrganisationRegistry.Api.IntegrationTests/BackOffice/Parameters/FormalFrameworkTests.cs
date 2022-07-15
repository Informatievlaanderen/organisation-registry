namespace OrganisationRegistry.Api.IntegrationTests.BackOffice.Parameters;

using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class FormalFrameworkTests
{
    private readonly ApiFixture _apiFixture;

    public FormalFrameworkTests(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task VademecumTest()
    {
        var getResponse = await ApiFixture.Get(_apiFixture.HttpClient, $"/v1/formalframeworks/vademecum");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
