namespace OrganisationRegistry.Api.Security;

using System;
using System.Collections.Generic;

public class IntrospectionResponse
{
    public bool Active { get; set; }
    public string? Scope { get; set; }
    public string? ClientId { get; set; }
    public string? Username { get; set; }
    public string? TokenType { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }
    public DateTimeOffset? IssuedAt { get; set; }
    public string? Subject { get; set; }
    public string[]? Audience { get; set; }
    public string? Issuer { get; set; }
    public string? JwtId { get; set; }
    public string? VoId { get; set; }
    public Dictionary<string, object> AdditionalClaims { get; set; } = new();
}