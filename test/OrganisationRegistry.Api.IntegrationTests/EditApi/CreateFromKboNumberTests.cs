namespace OrganisationRegistry.Api.IntegrationTests.EditApi;

using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using OrganisationRegistry.Infrastructure.Authorization;
using Tests.Shared;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class CreateFromKboNumberTests
{
    private readonly ApiFixture _fixture;

    public CreateFromKboNumberTests(ApiFixture fixture)
    {
        _fixture = fixture;
    }

    [EnvVarIgnoreFact]
    public async Task WithoutBearer_ReturnsUnauthorized()
    {
        var response = await CreateOrganisationFromKboNumber(_fixture.HttpClient);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [EnvVarIgnoreFact]
    public async Task AsOrafin_ReturnsForbidden()
    {
        var httpClient = await _fixture.CreateOrafinClient();

        var response = await CreateOrganisationFromKboNumber(httpClient);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [EnvVarIgnoreFact]
    public async Task AsCjmBeheerder_ReturnsCreated()
    {
        var httpClient = await _fixture.CreateCjmClient();

        var response = await CreateOrganisationFromKboNumber(httpClient);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        await VerifyContent(response);
    }

    [EnvVarIgnoreFact]
    public async Task AsCjmBeheerder_DuplicateCallReturnsFound()
    {
        var httpClient = await _fixture.CreateCjmClient();

        await CreateOrganisationFromKboNumber(httpClient);
        var response = await CreateOrganisationFromKboNumber(httpClient);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        await VerifyContent(response);
    }

    private static async Task VerifyContent(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        var organisationResponse = JsonConvert.DeserializeObject<OrganisationResponse>(content);
        organisationResponse.Should().NotBeNull();
        organisationResponse.OvoNumber.Should().NotBeNullOrEmpty();
    }

    private static async Task<HttpResponseMessage> CreateOrganisationFromKboNumber(HttpClient httpClient)
    {
        var response = await httpClient.PutAsync(
            "edit/organisations/kbo/0563634435",
            new StringContent("{}", Encoding.UTF8, "application/json"));
        return response;
    }

    private class OrganisationResponse
    {
        public string OvoNumber { get; set; }
    }
}
