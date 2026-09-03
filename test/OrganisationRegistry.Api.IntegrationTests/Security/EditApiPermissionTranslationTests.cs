namespace OrganisationRegistry.Api.IntegrationTests.Security;

using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using OrganisationRegistry.Infrastructure.Authorization;
using Tests.Shared;
using Xunit;

/// <summary>
/// T013 — Baseline regression coverage for the edit-api entry point (interactive user JWT).
/// Verifies that today's role-based identity translation continues to produce the
/// expected <see cref="SecurityInformation"/> shape after US2 flips controller gates
/// from roles to permissions. The interactive user seeded by <see cref="ApiFixture"/>
/// carries the <c>WegwijsBeheerder-algemeenbeheerder:OVO002949</c> claim, which must
/// map to <see cref="Role.AlgemeenBeheerder"/> at the API boundary.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class EditApiPermissionTranslationTests
{
    private readonly ApiFixture _apiFixture;

    public EditApiPermissionTranslationTests(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [EnvVarIgnoreFact]
    public async Task InteractiveUser_WithAlgemeenBeheerderClaim_TranslatesToAlgemeenBeheerderRole()
    {
        var response = await _apiFixture.HttpClient.GetAsync("/v1/security");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = JObject.Parse(await response.Content.ReadAsStringAsync());

        // The API serializes enums via StringEnumConverter + CamelCaseNamingStrategy
        // (see JsonSerializerSettingsExtensions.ConfigureForOrganisationRegistry).
        var roles = payload["roles"]?.Values<string>().ToArray();

        roles.Should().NotBeNull();
        roles!.Should().Contain("algemeenBeheerder");
    }
}
