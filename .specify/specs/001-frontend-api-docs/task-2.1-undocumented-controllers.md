# Task 2.1: Identify Undocumented Controllers

## Status: COMPLETE ✅

### Controllers Requiring Documentation (4 total, 10 endpoints)

#### 1. StatusController (4 endpoints)
**File**: `src/OrganisationRegistry.Api/Backoffice/Admin/Status/StatusController.cs`
**Route**: `/status`

| Method | Endpoint | Current Status | Action Required |
|--------|----------|----------------|-----------------|
| GET | `/status` | No summary/responses | Add docs |
| GET | `/status/toggles` | Has `[ProducesResponseType]` but no summary | Add summary |
| GET | `/status/features` | Has `[ProducesResponseType]` but no summary | Add summary |
| GET | `/status/configuration` | No summary/responses | Add docs |

**Summary**: Provides system health status, feature toggles, and configuration information. Admin endpoints (configuration requires Developer role).

#### 2. ManagementController (2 endpoints)
**File**: `src/OrganisationRegistry.Api/Backoffice/Management/ManagementController.cs`
**Route**: `/mgmt`

| Method | Endpoint | Current Status | Action Required |
|--------|----------|----------------|-----------------|
| GET | `/mgmt/days` | No summary/responses | Add docs |
| GET | `/mgmt/hours` | No summary/responses | Add docs |

**Summary**: Provides lookup data for UI dropdowns (day names, available hours).

#### 3. OrganisationOpeningHourController (2 endpoints)
**File**: `src/OrganisationRegistry.Api/Backoffice/Organisation/OpeningHour/OrganisationOpeningHourController.cs`
**Route**: `/organisations/{organisationId}/openingHours`

| Method | Endpoint | Current Status | Action Required |
|--------|----------|----------------|-----------------|
| GET | `/organisations/{organisationId}/openingHours` | No summary, has pagination support | Add summary |
| GET | `/organisations/{organisationId}/openingHours/{id}` | Has `[ProducesResponseType]` but no summary | Add summary |

**Summary**: Query endpoints for listing and retrieving opening hours for organisations.

#### 4. OrganisationOpeningHourCommandController (2 endpoints)
**File**: `src/OrganisationRegistry.Api/Backoffice/Organisation/OpeningHour/OrganisationOpeningHourCommandController.cs`
**Route**: `/organisations/{organisationId}/openingHours`

| Method | Endpoint | Current Status | Action Required |
|--------|----------|----------------|-----------------|
| POST | `/organisations/{organisationId}/openingHours` | Has `[ProducesResponseType]` but no summary | Add summary |
| PUT | `/organisations/{organisationId}/openingHours/{id}` | Has `[ProducesResponseType]` but no summary | Add summary |

**Summary**: Command endpoints for creating and updating opening hours. Requires `[OrganisationRegistryAuthorize]` attribute.

### Documentation Pattern to Use

Based on already-documented controllers, the XML documentation should follow this pattern:

```csharp
/// <summary>
Brief description of what the endpoint does
</summary>
/// <response code="200">What succeeds and what's returned</response>
/// <response code="201">For POST - what gets created</response>
/// <response code="400">Validation error condition</response>
/// <response code="404">When resource not found</response>
public async Task<IActionResult> MethodName(...)
```

### Example from Already-Documented Controller

From `ConfigurationController.Get()`:
```csharp
/// <summary>Get a configuration value.</summary>
/// <response code="200">If the configuration value is found.</response>
/// <response code="404">If the configuration value cannot be found.</response>
public async Task<IActionResult> Get(...)
```

### What NOT to Document

Per project constitution, we do NOT:
- Add test coverage for these controllers
- Modify XML comments in already-documented controllers
- Change method signatures or behavior
- Add new features

We ONLY add XML `<summary>` and `<response>` tags where missing.

### Next Actions

1. **Task 2.2**: Document StatusController (4 methods)
2. **Task 2.3**: Document ManagementController (2 methods)  
3. **Task 2.4**: Document OrganisationOpeningHourController (2 methods)
4. **Task 2.5**: Document OrganisationOpeningHourCommandController (2 methods)
5. **Task 2.6**: Build and verify XML generation
6. **Task 2.7**: Validate OpenAPI spec generation
7. **Task 2.8**: Commit documentation changes

### Effort Estimate

- **StatusController**: ~10 minutes (4 methods, moderate complexity)
- **ManagementController**: ~5 minutes (2 simple methods)
- **OrganisationOpeningHourController**: ~7 minutes (2 query methods)
- **OrganisationOpeningHourCommandController**: ~8 minutes (2 command methods)
- **Build & Verify**: ~10 minutes
- **Total**: ~40 minutes

### Why This Matters

These 4 controllers represent 10 endpoints (~3% of the ~290 Backoffice endpoints). 
Getting them documented will result in 100% OpenAPI coverage for the Backoffice API.
