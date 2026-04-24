namespace OrganisationRegistry.Api.Security;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

public class TokenIntrospectionService : ITokenIntrospectionService
{
    private readonly HttpClient _httpClient;
    private readonly TokenExchangeConfiguration _configuration;

    public TokenIntrospectionService(HttpClient httpClient, IOptions<TokenExchangeConfiguration> configuration)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _configuration = configuration?.Value ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<IntrospectionResponse> IntrospectTokenAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token cannot be null or empty", nameof(token));

        var requestBody = new List<KeyValuePair<string, string>>
        {
            new("token", token),
            new("token_type_hint", "access_token")
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, _configuration.IntrospectionEndpoint)
        {
            Content = new FormUrlEncodedContent(requestBody)
        };

        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_configuration.ClientId}:{_configuration.ClientSecret}"));
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);

        try
        {
            using var response = await _httpClient.SendAsync(request);
            
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new UnauthorizedAccessException($"Introspection authentication failed: {errorContent}");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Introspection request failed with status {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(content);

            var introspectionResponse = new IntrospectionResponse
            {
                Active = jsonResponse.TryGetProperty("active", out var activeProp) && activeProp.GetBoolean(),
                TokenType = GetStringProperty(jsonResponse, "token_type"),
                Subject = GetStringProperty(jsonResponse, "sub"),
                VoId = GetStringProperty(jsonResponse, "vo_id"),
                ClientId = GetStringProperty(jsonResponse, "client_id"),
                Username = GetStringProperty(jsonResponse, "username"),
                Scope = GetStringProperty(jsonResponse, "scope"),
                Issuer = GetStringProperty(jsonResponse, "iss"),
                JwtId = GetStringProperty(jsonResponse, "jti")
            };

            if (jsonResponse.TryGetProperty("exp", out var expProp) && expProp.TryGetInt64(out var exp))
                introspectionResponse.ExpiresAt = DateTimeOffset.FromUnixTimeSeconds(exp);

            if (jsonResponse.TryGetProperty("iat", out var iatProp) && iatProp.TryGetInt64(out var iat))
                introspectionResponse.IssuedAt = DateTimeOffset.FromUnixTimeSeconds(iat);

            if (jsonResponse.TryGetProperty("aud", out var audProp))
            {
                if (audProp.ValueKind == JsonValueKind.Array)
                {
                    var audiences = new List<string>();
                    foreach (var element in audProp.EnumerateArray())
                    {
                        if (element.ValueKind == JsonValueKind.String)
                            audiences.Add(element.GetString()!);
                    }
                    introspectionResponse.Audience = audiences.ToArray();
                }
                else if (audProp.ValueKind == JsonValueKind.String)
                {
                    introspectionResponse.Audience = new[] { audProp.GetString()! };
                }
            }

            foreach (var property in jsonResponse.EnumerateObject())
            {
                if (!IsStandardProperty(property.Name))
                {
                    introspectionResponse.AdditionalClaims[property.Name] = property.Value.ValueKind switch
                    {
                        JsonValueKind.String => property.Value.GetString()!,
                        JsonValueKind.Number => property.Value.GetDouble(),
                        JsonValueKind.True => true,
                        JsonValueKind.False => false,
                        JsonValueKind.Array => property.Value.EnumerateArray().Select(x => x.GetString()).ToArray(),
                        _ => property.Value.GetRawText()
                    };
                }
            }

            return introspectionResponse;
        }
        catch (TaskCanceledException)
        {
            throw new TaskCanceledException("Introspection request timeout");
        }
    }

    private static string? GetStringProperty(JsonElement jsonElement, string propertyName)
    {
        return jsonElement.TryGetProperty(propertyName, out var property) && 
               property.ValueKind == JsonValueKind.String 
            ? property.GetString() 
            : null;
    }

    private static bool IsStandardProperty(string propertyName)
    {
        return propertyName switch
        {
            "active" or "token_type" or "sub" or "vo_id" or "client_id" or 
            "username" or "scope" or "iss" or "jti" or "exp" or "iat" or "aud" => true,
            _ => false
        };
    }
}