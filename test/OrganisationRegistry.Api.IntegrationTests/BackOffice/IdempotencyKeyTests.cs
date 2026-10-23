namespace OrganisationRegistry.Api.IntegrationTests.BackOffice;

using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Newtonsoft.Json;
using OrganisationRegistry.Api.Backoffice.Organisation.Detail;
using OrganisationRegistry.Api.Infrastructure;
using Xunit;

/// <summary>
/// OR-3539: without an Idempotency-Key, an integrator retrying a Create call
/// (e.g. after a timeout) has no safe way to detect whether the original
/// request actually succeeded, short of resending the exact same identifier
/// and risking the duplicate-id error covered by
/// <see cref="DuplicateIdentifierTests"/>. These tests cover the
/// <c>IdempotencyKey</c> header support (backed by the <c>IdempotentAPI</c>
/// library) added to the Create (POST) organisation endpoint: a retried
/// request carrying the same key and the same body is NOT re-executed and
/// instead returns the original cached response.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class IdempotencyKeyTests
{
    private readonly ApiFixture _apiFixture;

    public IdempotencyKeyTests(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task When_Retrying_Create_Organisation_With_Same_IdempotencyKey_And_Same_Body_Then_Returns_Original_Response()
    {
        using var client = await _apiFixture.CreateAlgemeenbeheerderClient();
        var idempotencyKey = _apiFixture.Fixture.Create<Guid>().ToString();
        var request = new CreateOrganisationRequest
        {
            Id = _apiFixture.Fixture.Create<Guid>(),
            Name = _apiFixture.Fixture.Create<string>(),
            ParentOrganisationId = null,
        };

        using var firstResponse = await PostWithIdempotencyKey(client, "/v1/organisations", request, idempotencyKey);
        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var firstLocation = firstResponse.Headers.Location;

        using var secondResponse = await PostWithIdempotencyKey(client, "/v1/organisations", request, idempotencyKey);

        secondResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        secondResponse.Headers.Location.Should().Be(firstLocation);
    }

    [Fact]
    public async Task When_Reusing_IdempotencyKey_With_A_Different_Body_Then_Returns_BadRequest()
    {
        using var client = await _apiFixture.CreateAlgemeenbeheerderClient();
        var idempotencyKey = _apiFixture.Fixture.Create<Guid>().ToString();

        using var firstResponse = await PostWithIdempotencyKey(
            client,
            "/v1/organisations",
            new CreateOrganisationRequest
            {
                Id = _apiFixture.Fixture.Create<Guid>(),
                Name = _apiFixture.Fixture.Create<string>(),
                ParentOrganisationId = null,
            },
            idempotencyKey);
        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        using var secondResponse = await PostWithIdempotencyKey(
            client,
            "/v1/organisations",
            new CreateOrganisationRequest
            {
                // Different id/name, same idempotency key: the library should
                // detect the request hash mismatch and reject it, rather than
                // returning the unrelated cached response.
                Id = _apiFixture.Fixture.Create<Guid>(),
                Name = _apiFixture.Fixture.Create<string>(),
                ParentOrganisationId = null,
            },
            idempotencyKey);

        secondResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task When_Creating_Organisation_Without_IdempotencyKey_Then_Still_Returns_Created()
    {
        // IsIdempotencyOptional = true: existing integrators that do not send
        // the header must keep working unchanged.
        using var client = await _apiFixture.CreateAlgemeenbeheerderClient();

        using var response = await ApiFixture.Post(
            client,
            "/v1/organisations",
            new CreateOrganisationRequest
            {
                Id = _apiFixture.Fixture.Create<Guid>(),
                Name = _apiFixture.Fixture.Create<string>(),
                ParentOrganisationId = null,
            });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task When_Retrying_Create_OrganisationKey_With_Same_IdempotencyKey_Then_Returns_Original_Response()
    {
        // OrganisationKeyCommandController never had [Idempotent] applied by
        // hand: this proves the IdempotencyConvention (applied globally to
        // every POST/PATCH action on a command controller) covers it too,
        // not just the 3 endpoints from the original bug report.
        using var client = await _apiFixture.CreateAlgemeenbeheerderClient();

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var keyTypeId = await _apiFixture.Create.KeyType();

        var idempotencyKey = _apiFixture.Fixture.Create<Guid>().ToString();
        var request = new
        {
            OrganisationKeyId = _apiFixture.Fixture.Create<Guid>(),
            KeyValue = _apiFixture.Fixture.Create<string>(),
            KeyTypeId = keyTypeId,
            ValidFrom = (DateTime?) null,
            ValidTo = (DateTime?) null,
        };
        var route = $"/v1/organisations/{organisationId}/keys";

        using var firstResponse = await PostWithIdempotencyKey(client, route, request, idempotencyKey);
        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var firstLocation = firstResponse.Headers.Location;

        using var secondResponse = await PostWithIdempotencyKey(client, route, request, idempotencyKey);

        secondResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        secondResponse.Headers.Location.Should().Be(firstLocation);
    }

    private static async Task<HttpResponseMessage> PostWithIdempotencyKey(HttpClient client, string route, object body, string idempotencyKey)
    {
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, route)
        {
            Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json"),
        };
        httpRequest.Headers.TryAddWithoutValidation(IdempotencyConstants.HeaderName, idempotencyKey);

        return await client.SendAsync(httpRequest);
    }
}
