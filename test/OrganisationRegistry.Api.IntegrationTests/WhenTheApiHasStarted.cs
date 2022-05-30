namespace OrganisationRegistry.Api.IntegrationTests;

using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class WhenTheApiStartsUp
{
    private readonly ApiFixture _fixture;

    public WhenTheApiStartsUp(ApiFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task StatusResourceReturns200()
    {
        (await _fixture.HttpClient.GetAsync("status")).StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task StatusResourceReturnsItsOk()
    {
        (await (await _fixture.HttpClient.GetAsync("status")).Content.ReadAsStringAsync()).Should().Be("\"I'm ok!\"");
    }
}