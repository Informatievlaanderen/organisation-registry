# Quickstart: Authentication Rework — Multi-Method Support

**Branch**: `002-auth-rework` | **Date**: 2026-03-23

---

## What This Feature Changes

This feature makes the API accept three token types simultaneously:

1. **Self-minted HMAC-SHA256 JWT** — existing SPA path, unchanged.
2. **ACM/IDM client credentials token** — existing M2M path, unchanged behaviour.
3. **ACM/IDM token exchange token** — new: arrives as `Authorization: Bearer <acm-idm-token>`, validated via introspection, accepted by any endpoint whose policy the token's claims satisfy.

No SPA changes. No M2M client changes. No new NuGet packages.

---

## Key Code Locations

| What | File |
|---|---|
| Authentication scheme registration | `src/OrganisationRegistry.Api/Infrastructure/Startup.cs` (line ~129) |
| Composite scheme constant | `src/OrganisationRegistry.Api/Infrastructure/Security/AuthenticationSchemes.cs` |
| `[OrganisationRegistryAuthorize]` attribute | `src/OrganisationRegistry.Api/Infrastructure/Security/OrganisationRegistryAuthorizeAttribute.cs` |
| Multi-scheme claims selector middleware | `src/OrganisationRegistry.Api/Infrastructure/Security/ConfigureClaimsPrincipalSelectorMiddleware.cs` |
| Async multi-scheme auth info helper | `src/OrganisationRegistry.Api/Infrastructure/Security/GetAuthenticateInfoExtensions.cs` |
| `SecurityService.GetRequiredUser` | `src/OrganisationRegistry.Api/Security/SecurityService.cs` (line ~134) |
| Edit API controllers (5 files) | `src/OrganisationRegistry.Api/Edit/Organisation/*/` |

---

## Making the Change: Step by Step

### Step 1 — Add composite scheme constant

`AuthenticationSchemes.cs`:
```csharp
public const string Composite = "Composite";
```

### Step 2 — Register composite scheme in `Startup.cs`

Replace:
```csharp
options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
```

With:
```csharp
options.DefaultScheme = AuthenticationSchemes.Composite;
options.DefaultAuthenticateScheme = AuthenticationSchemes.Composite;
options.DefaultChallengeScheme = AuthenticationSchemes.Composite;
```

Then chain `.AddPolicyScheme(...)` after `.AddOAuth2Introspection(...)`:
```csharp
.AddPolicyScheme(
    AuthenticationSchemes.Composite,
    displayName: "Composite (Bearer → EditApi)",
    options =>
    {
        options.ForwardDefaultSelector = context =>
        {
            // Inspect the JWT header alg field (decode only — no verify).
            // alg=HS256 → self-minted JWT → Bearer.
            // Anything else → ACM/IDM token → EditApi.
            var authorization = context.Request.Headers[HeaderNames.Authorization].FirstOrDefault();
            if (authorization?.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) == true)
            {
                var token = authorization["Bearer ".Length..].Trim();
                var parts = token.Split('.');
                if (parts.Length == 3)
                {
                    try
                    {
                        var header = Base64Url.Decode(parts[0]);
                        if (System.Text.Json.JsonSerializer.Deserialize<JwtHeader>(header)?.Alg == "HS256")
                            return JwtBearerDefaults.AuthenticationScheme;
                    }
                    catch { /* malformed — fall through */ }
                }
            }
            return AuthenticationSchemes.EditApi;
        };
    })
```

### Step 3 — Remove scheme hardcoding from `OrganisationRegistryAuthorizeAttribute`

Remove the `AuthenticationSchemes` assignment:
```csharp
// Before:
public OrganisationRegistryAuthorizeAttribute()
{
    AuthenticationSchemes = string.Join(", ", JwtBearerDefaults.AuthenticationScheme);
}

// After:
public OrganisationRegistryAuthorizeAttribute()
{
    // No AuthenticationSchemes — uses DefaultAuthenticateScheme (Composite)
}
```

### Step 4 — Remove scheme hardcoding from 5 Edit API controllers

In each of the five Edit API controller `[Authorize]` attributes, remove `AuthenticationSchemes = AuthenticationSchemes.EditApi`:
```csharp
// Before:
[Authorize(AuthenticationSchemes = AuthenticationSchemes.EditApi, Policy = PolicyNames.OrganisationContacts)]

// After:
[Authorize(Policy = PolicyNames.OrganisationContacts)]
```

Files:
- `Edit/Organisation/Contacts/OrganisationContactsController.cs`
- `Edit/Organisation/BankAccount/OrganisationBankAccountController.cs`
- `Edit/Organisation/Key/OrganisationKeyController.cs`
- `Edit/Organisation/Classification/OrganisationOrganisationClassificationController.cs`
- `Edit/Organisation/KboNumber/OrganisationsController.cs`

### Step 5 — Fix `GetAuthenticateInfoAsync` to try both schemes

`GetAuthenticateInfoExtensions.cs`:
```csharp
public static async Task<AuthenticateResult?> GetAuthenticateInfoAsync(this HttpContext source)
{
    var bearerInfo = await source.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
    if (bearerInfo.Succeeded) return bearerInfo;

    var editApiInfo = await source.AuthenticateAsync(AuthenticationSchemes.EditApi);
    return editApiInfo.Succeeded ? editApiInfo : null;
}
```

### Step 6 — Fix `SecurityService.GetRequiredUser` claim name fallback

`SecurityService.cs`, `GetRequiredUser`:
```csharp
var firstName = principal.FindFirst(ClaimTypes.GivenName)
    ?? principal.FindFirst(AcmIdmConstants.Claims.Firstname);
if (firstName == null)
    throw new Exception("Could not determine current user's first name");

var lastName = principal.FindFirst(ClaimTypes.Surname)
    ?? principal.FindFirst(AcmIdmConstants.Claims.FamilyName);
if (lastName == null)
    throw new Exception("Could not determine current user's last name");
```

Note: `AcmIdmConstants.Claims.FamilyName` (`urn:be:vlaanderen:acm:familienaam`) must be added to `AcmIdmConstants.cs` if not present (currently only `Firstname` exists as `urn:be:vlaanderen:acm:voornaam`).

### Step 7 — Add integration tests for token exchange path

In `test/OrganisationRegistry.Api.IntegrationTests/EditApi/TokenExchangeAuthorizationTests.cs`:
- `WithTokenExchangeToken_OnBackofficeEndpoint_WithSufficientClaims_Returns200`
- `WithTokenExchangeToken_OnBackofficeEndpoint_WithInsufficientClaims_Returns403`
- `WithInvalidTokenExchangeToken_Returns401`

Add `CreateTokenExchangeClient()` to `ApiFixture` that obtains a token via a grant type that returns user identity claims from the local Keycloak test instance.

---

## Testing

```bash
# Run all integration tests
dotnet test test/OrganisationRegistry.Api.IntegrationTests/OrganisationRegistry.Api.IntegrationTests.csproj

# Run only token exchange tests
dotnet test test/OrganisationRegistry.Api.IntegrationTests/OrganisationRegistry.Api.IntegrationTests.csproj \
  --filter "FullyQualifiedName~TokenExchange"

# Run full test suite
dotnet test
```

---

## Verification Checklist

- [ ] `grep -r "AuthenticationSchemes.*Bearer" src/` → zero results in `[Authorize]` attributes
- [ ] `grep -r "AuthenticationSchemes.*EditApi" src/` → zero results in `[Authorize]` attributes  
- [ ] `grep -r "AddPolicyScheme" src/` → exactly one result in `Startup.cs`
- [ ] `DefaultAuthenticateScheme = "Composite"` in `Startup.cs`
- [ ] All existing integration tests pass (SC-001, SC-002)
- [ ] `TokenExchangeAuthorizationTests` pass: 200, 403, 401 (SC-003, SC-006)
