namespace OrganisationRegistry.Api.IntegrationTests.Security;

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Tests.Shared;
using Xunit;

/// <summary>
/// Integration tests for the Nuxt BFF callback endpoint.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class OAuth2CallbackTests : IDisposable
{
    private const string NuxtBffBaseUrl = "http://app.localhost:9080";

    private readonly HttpClient _httpClient;

    public OAuth2CallbackTests(ApiFixture apiFixture)
    {
        _ = apiFixture;
        _httpClient = new HttpClient(new HttpClientHandler { AllowAutoRedirect = false });
    }

    [EnvVarIgnoreFact]
    public async Task NuxtBffCallback_WithErrorParameters_RedirectsToHomeWithErrorAndClearsSession()
    {
        var callbackUrl = $"{NuxtBffBaseUrl}/callback?error=access_denied&error_description=User+denied+access&state=test_state";

        var response = await _httpClient.GetAsync(callbackUrl);

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location?.ToString().Should().Be("/?error=User%20denied%20access");
        AssertSessionCookiesAreCleared(response);
    }

    [EnvVarIgnoreFact]
    public async Task NuxtBffCallback_WithoutCodeOrState_RedirectsToMissingCodeOrStateAndClearsSession()
    {
        var response = await _httpClient.GetAsync($"{NuxtBffBaseUrl}/callback");

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location?.ToString().Should().Be("/?error=missing_code_or_state");
        AssertSessionCookiesAreCleared(response);
    }

    [EnvVarIgnoreFact]
    public async Task NuxtBffCallback_WithInvalidState_RedirectsToInvalidStateAndClearsSession()
    {
        var response = await _httpClient.GetAsync($"{NuxtBffBaseUrl}/callback?code=test_code&state=invalid-state");

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location?.ToString().Should().Be("/?error=invalid_state");
        AssertSessionCookiesAreCleared(response);
    }

    [EnvVarIgnoreFact]
    public async Task NuxtBffCallbackEndpoint_IsReachableThroughTiltIngress()
    {
        var response = await _httpClient.GetAsync($"{NuxtBffBaseUrl}/callback");

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.StatusCode.Should().NotBe(HttpStatusCode.ServiceUnavailable);
        response.StatusCode.Should().NotBe(HttpStatusCode.BadGateway);
    }

    private static void AssertSessionCookiesAreCleared(HttpResponseMessage response)
    {
        response.Headers.TryGetValues("Set-Cookie", out var values).Should().BeTrue();

        var setCookies = values!.ToArray();
        setCookies.Should().Contain(cookie =>
            cookie.StartsWith("bff_session=; Max-Age=0", StringComparison.OrdinalIgnoreCase));
        setCookies.Should().Contain(cookie =>
            cookie.StartsWith("bff_session_ex=; Max-Age=0", StringComparison.OrdinalIgnoreCase));
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
            _httpClient.Dispose();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
