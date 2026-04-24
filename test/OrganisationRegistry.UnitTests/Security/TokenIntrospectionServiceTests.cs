namespace OrganisationRegistry.UnitTests.Security;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using OrganisationRegistry.Api.Security;
using Xunit;

public class TokenIntrospectionServiceTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpHandler;
    private readonly Mock<IOptions<TokenExchangeConfiguration>> _mockConfig;
    private readonly HttpClient _httpClient;
    private readonly TokenIntrospectionService _service;
    private readonly Fixture _fixture;

    public TokenIntrospectionServiceTests()
    {
        _fixture = new Fixture();
        _mockHttpHandler = new Mock<HttpMessageHandler>();
        _mockConfig = new Mock<IOptions<TokenExchangeConfiguration>>();
        
        _httpClient = new HttpClient(_mockHttpHandler.Object);
        
        _mockConfig.Setup(x => x.Value).Returns(new TokenExchangeConfiguration
        {
            Authority = "https://test-auth.example.com",
            IntrospectionEndpoint = "https://test-auth.example.com/oauth2/introspect",
            ClientId = "test_client",
            ClientSecret = "test_secret_123",
            TimeoutMs = 5000,
            Enabled = true
        });
        
        _service = new TokenIntrospectionService(_httpClient, _mockConfig.Object);
    }

    [Theory]
    [InlineData("valid_token", true, "VO12345")]
    [InlineData("expired_token", false, null)]
    [InlineData("malformed_token", false, null)]
    public async Task IntrospectTokenAsync_WithVariousTokens_ReturnsExpectedResults(
        string token, bool expectedActive, string? expectedVoId)
    {
        // Arrange
        SetupMockHttpResponse(token, expectedActive, expectedVoId);

        // Act
        var result = await _service.IntrospectTokenAsync(token);

        // Assert
        result.Should().NotBeNull();
        result.Active.Should().Be(expectedActive);
        result.VoId.Should().Be(expectedVoId);
    }

    [Fact]
    public async Task IntrospectTokenAsync_WithValidUserToken_ReturnsCompleteUserTokenResponse()
    {
        // Arrange
        const string token = "valid_user_token";
        const string expectedVoId = "VO12345";
        const string expectedSubject = "user_12345";
        const string expectedRoleClaim = "WegwijsBeheerder-algemeenbeheerder:OVO002949";

        SetupMockHttpResponse(token, true, expectedVoId, expectedSubject, expectedRoleClaim);

        // Act
        var result = await _service.IntrospectTokenAsync(token);

        // Assert - Required fields for user tokens
        result.Should().NotBeNull();
        result.Active.Should().BeTrue();
        result.VoId.Should().Be(expectedVoId);
        result.Subject.Should().Be(expectedSubject);
        result.AdditionalClaims.Should().ContainKey("iv_wegwijs_rol_3D");
        result.AdditionalClaims["iv_wegwijs_rol_3D"].Should().Be(expectedRoleClaim);
    }

    [Fact]
    public async Task IntrospectTokenAsync_WithInactiveToken_ReturnsInactiveResponse()
    {
        // Arrange
        const string token = "inactive_token";
        SetupMockHttpResponse(token, false, null);

        // Act
        var result = await _service.IntrospectTokenAsync(token);

        // Assert
        result.Should().NotBeNull();
        result.Active.Should().BeFalse();
        result.VoId.Should().BeNull();
    }

    [Fact]
    public async Task IntrospectTokenAsync_WithHttpTimeout_ThrowsTimeoutException()
    {
        // Arrange
        const string token = "timeout_token";
        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new TaskCanceledException("Request timeout"));

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(() => _service.IntrospectTokenAsync(token));
    }

    [Fact]
    public async Task IntrospectTokenAsync_WithUnauthorizedResponse_ThrowsAuthenticationException()
    {
        // Arrange
        const string token = "unauthorized_token";
        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                Content = new StringContent("{\"error\":\"invalid_client\"}")
            });

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
            _service.IntrospectTokenAsync(token));
        exception.Message.Should().Contain("invalid_client");
    }

    [Fact]
    public async Task IntrospectTokenAsync_WithServerError_ThrowsHttpRequestException()
    {
        // Arrange
        const string token = "server_error_token";
        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("{\"error\":\"server_error\"}")
            });

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => _service.IntrospectTokenAsync(token));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task IntrospectTokenAsync_WithInvalidToken_ThrowsArgumentException(string? invalidToken)
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.IntrospectTokenAsync(invalidToken!));
    }

    [Fact]
    public async Task IntrospectTokenAsync_SendsCorrectHttpRequest()
    {
        // Arrange
        const string token = "test_token";
        HttpRequestMessage? actualRequest = null;
        string? requestContent = null;

        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>(async (req, _) => 
            {
                actualRequest = req;
                if (req.Content != null)
                {
                    requestContent = await req.Content.ReadAsStringAsync();
                }
            })
            .ReturnsAsync(CreateIntrospectionHttpResponse(true, "VO12345"));

        // Act
        await _service.IntrospectTokenAsync(token);

        // Assert - Verify HTTP request format
        actualRequest.Should().NotBeNull();
        actualRequest!.Method.Should().Be(HttpMethod.Post);
        actualRequest.RequestUri.Should().Be(_mockConfig.Object.Value.IntrospectionEndpoint);
        actualRequest.Headers.Authorization.Should().NotBeNull();
        actualRequest.Headers.Authorization!.Scheme.Should().Be("Basic");

        // Verify request body contains token
        requestContent.Should().NotBeNull();
        requestContent!.Should().Contain($"token={token}");
        requestContent.Should().Contain("token_type_hint=access_token");
    }

    private void SetupMockHttpResponse(string token, bool active, string? voId, 
        string? subject = null, string? roleClaim = null)
    {
        var response = CreateIntrospectionHttpResponse(active, voId, subject, roleClaim);
        
        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.RequestUri.ToString().Contains("introspect")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);
    }

    private static HttpResponseMessage CreateIntrospectionHttpResponse(bool active, string? voId, 
        string? subject = null, string? roleClaim = null)
    {
        var responseContent = active 
            ? CreateActiveIntrospectionJson(voId, subject, roleClaim)
            : "{\"active\":false}";

        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseContent, System.Text.Encoding.UTF8, "application/json")
        };
    }

    private static string CreateActiveIntrospectionJson(string? voId, string? subject, string? roleClaim)
    {
        var additionalFields = new List<string>();

        if (!string.IsNullOrEmpty(subject))
            additionalFields.Add($"\"sub\":\"{subject}\"");
        
        if (!string.IsNullOrEmpty(voId))
            additionalFields.Add($"\"vo_id\":\"{voId}\"");
        
        if (!string.IsNullOrEmpty(roleClaim))
            additionalFields.Add($"\"iv_wegwijs_rol_3D\":\"{roleClaim}\"");

        var allFields = string.Join(",", 
            new[] { "\"active\":true", "\"token_type\":\"Bearer\"" }
            .Concat(additionalFields));

        return $"{{{allFields}}}";
    }
}