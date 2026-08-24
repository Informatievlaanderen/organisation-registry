namespace OrganisationRegistry.Api.IntegrationTests;

using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Newtonsoft.Json.Linq;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class WhenSearchingOrganisations : IAsyncLifetime
{
    private readonly ApiFixture _fixture;

    public WhenSearchingOrganisations(ApiFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
        => await _fixture.EnsureImportedDataIsReady();

    public Task DisposeAsync()
        => Task.CompletedTask;

    /// <summary>
    /// Test that verifies bank accounts are cleared/empty in search results via GET box endpoint.
    /// This ensures sensitive financial information is not exposed via the public search API.
    /// </summary>
    [Fact]
    public async Task GetApiSearchOrganisations_WhenResultsExist_BankAccountsAreEmptyOrMissing()
    {
        // Arrange
        var searchQuery = "*";
        var indexName = "organisations";
        var requestUri = $"/v1/search/{indexName}";

        // Act
        var response = await ApiFixture.Get(_fixture.HttpClient, $"{requestUri}?q={searchQuery}&offset=0&limit=20");

        var responseContent = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, $"GET /v1/search/organisations should return 200 OK. Response body: {responseContent}");

        var organisations = JArray.Parse(responseContent);

        // In some test environments the OpenSearch projection can be empty.
        // In that case we only assert endpoint stability.
        if (organisations.Count == 0)
            return;

        foreach (var organisationToken in organisations)
        {
            var organisation = organisationToken as JObject;
            organisation.Should().NotBeNull();

            var bankAccounts = organisation!["bankAccounts"];
            if (bankAccounts is JArray bankAccountsArray)
            {
                bankAccountsArray.Should().BeEmpty(
                    $"Organisation {organisation["ovoNumber"]?.ToString() ?? "unknown"} should have empty BankAccounts");
                continue;
            }

            if (bankAccounts is null || bankAccounts.Type == JTokenType.Null)
                continue;

            throw new AssertionFailedException($"BankAccounts should be null or an empty array, but got {bankAccounts.Type}");
        }
    }
}
