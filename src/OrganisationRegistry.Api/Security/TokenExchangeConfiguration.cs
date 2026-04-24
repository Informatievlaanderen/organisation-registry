namespace OrganisationRegistry.Api.Security;

using System;
using System.ComponentModel.DataAnnotations;

public class TokenExchangeConfiguration
{
    [Required]
    [Url]
    public string Authority { get; set; } = string.Empty;
    
    [Required] 
    [Url]
    public string IntrospectionEndpoint { get; set; } = string.Empty;
    
    [Required]
    [MinLength(1)]
    public string ClientId { get; set; } = string.Empty;
    
    [Required]
    [MinLength(8)]
    public string ClientSecret { get; set; } = string.Empty;
    
    [Range(typeof(TimeSpan), "00:00:30", "00:30:00")]
    public TimeSpan CacheTtl { get; set; } = TimeSpan.FromMinutes(2);
    
    public bool Enabled { get; set; } = false;
    
    [Range(1000, 30000)]
    public int TimeoutMs { get; set; } = 5000;
    
    [Range(1, 10)]
    public int MaxRetries { get; set; } = 3;
    
    public string[]? RequiredScopes { get; set; }
    
    public string[]? AllowedIssuers { get; set; }
}