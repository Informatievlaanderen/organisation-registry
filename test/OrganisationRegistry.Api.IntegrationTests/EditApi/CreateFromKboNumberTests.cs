namespace OrganisationRegistry.Api.IntegrationTests.EditApi;

using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class CreateFromKboNumberTests
{
    private readonly ApiFixture _fixture;

    public CreateFromKboNumberTests(ApiFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task WithoutBearer_Returns401()
    {
        var response = await _fixture.HttpClient.PutAsync(
            "edit/organisations/kbo/0563634435",
            new StringContent("{}", Encoding.UTF8, "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AsOrafin_Returns401()
    {
        var httpClient = await _fixture.CreateHttpClientFor("orafinClient", "dv_organisatieregister_orafinbeheerder");

        var response = await httpClient.PutAsync(
            "edit/organisations/kbo/0563634435",
            new StringContent("{}", Encoding.UTF8, "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AsCjmBeheerder_ReturnsCreated()
    {
        var httpClient = await _fixture.CreateHttpClientFor("cjmClient", "dv_organisatieregister_cjmbeheerder");

        var response = await httpClient.PutAsync(
            "edit/organisations/kbo/0563634435",
            new StringContent("{}", Encoding.UTF8, "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        await VerifyContent(response);
    }

    [Fact]
    public async Task AsCjmBeheerder_DuplicateCallReturnsFound()
    {
        var httpClient = await _fixture.CreateHttpClientFor("cjmClient", "dv_organisatieregister_cjmbeheerder");

        await httpClient.PutAsync(
            "edit/organisations/kbo/0563634435",
            new StringContent("{}", Encoding.UTF8, "application/json"));

        var response = await httpClient.PutAsync(
            "edit/organisations/kbo/0563634435",
            new StringContent("{}", Encoding.UTF8, "application/json"));

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

    private class OrganisationResponse
    {
        public string OvoNumber { get; set; }
    }
}
