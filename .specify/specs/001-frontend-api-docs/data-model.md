# Data Model: Internal/Screen API OpenAPI Documentation

**Feature**: 001-frontend-api-docs  
**Date**: 2026-03-17  
**Scope**: Data models and contracts for OpenAPI documentation

## OpenAPI Specification Structure

### Overview

The OpenAPI specification will document all internal/screen API endpoints from the Backoffice controllers. The specification will be generated automatically from C# Swashbuckle annotations.

**OpenAPI Version**: [TO BE DETERMINED - match existing public API version]  
**API Title**: Organisation Registry Internal/Screen APIs  
**API Description**: Internal APIs used by the Angular SPA for organisation management, administrative tasks, and backend operations

### Core API Entities

These entities are returned by various internal API endpoints and must be fully documented in the OpenAPI spec:

#### 1. Organisatie (Organisation)
- **Fields**:
  - `id` (string): OVO-nummer identifier (format: OVO + 6 digits)
  - `name` (string): Organisation name
  - `description` (string, optional): Organisation description
  - `status` (enum): Active/Inactive/Archived
  - `type` (enum): Public organisation type (municipaliteit, provincia, waterschap, etc.)
  - `founded` (date): Founding date
  - `validFrom` (date): Valid from date
  - `validTo` (date, optional): Valid to date (if dissolved)
  - `addresses` (array): List of address objects
  - `contacts` (array): List of contact objects
  - `buildings` (array): List of building objects
  - `organen` (array): List of governance bodies (Orgaan objects)

- **Relationships**:
  - Many-to-many with Delegations (via delegations API)
  - One-to-many with Orgaan (governance bodies)
  - One-to-many with Buildings
  - Synchronized from KBO (for legal entities)

- **Validation Rules**:
  - `id`: Required, immutable, unique across system
  - `name`: Required, max 255 characters
  - `status`: Required, one of: Active, Inactive, Archived
  - `validFrom`: Required, must be past date or today
  - `validTo`: Optional, if present must be after `validFrom`

#### 2. OVO-nummer (Identifier)
- **Format**: String, "OVO" + 6 digits (e.g., "OVO000123")
- **Uniqueness**: Globally unique within Organisation Registry
- **Usage**: Primary identifier for all organisation queries, URL paths, API parameters
- **Immutability**: Never changes once assigned
- **Validation**: Must match pattern `OVO\d{6}`

#### 3. Orgaan (Governance Body)
- **Fields**:
  - `id` (string): Unique identifier
  - `organisationId` (string): Parent organisation OVO-nummer
  - `name` (string): Body name (e.g., "Gemeenteraad", "Burgemeester en Wethouders")
  - `type` (enum): Type of governance body
  - `validFrom` (date): Effective date
  - `validTo` (date, optional): End date

- **Relationships**:
  - Many-to-one with Organisatie (parent organisation)
  - May have delegated permissions (via delegations API)

#### 4. Address
- **Fields**:
  - `id` (string): Unique identifier
  - `street` (string): Street name
  - `number` (string): Building number
  - `busBox` (string, optional): Bus/box number (apartment, suite)
  - `zipCode` (string): Postal code
  - `city` (string): City name
  - `country` (string): Country (ISO 3166)
  - `type` (enum): Registered/Operational/Visiting
  - `validFrom` (date): Effective date
  - `validTo` (date, optional): End date

#### 5. Contact
- **Fields**:
  - `id` (string): Unique identifier
  - `type` (enum): Email/Phone/Website/Fax
  - `value` (string): Contact value (email, phone number, URL)
  - `validFrom` (date): Effective date
  - `validTo` (date, optional): End date

#### 6. Building
- **Fields**:
  - `id` (string): Unique identifier
  - `name` (string): Building name
  - `address` (Address object): Building address
  - `validFrom` (date): Effective date
  - `validTo` (date, optional): End date

### HTTP Status Codes & Error Responses

All endpoints must document these standard HTTP responses:

- **200 OK**: Request succeeded, response body present
- **201 Created**: Resource created successfully
- **204 No Content**: Request succeeded, no response body
- **400 Bad Request**: Validation error (invalid parameters, missing required fields)
- **401 Unauthorized**: Authentication required or failed
- **403 Forbidden**: Authenticated but lacks permission for this endpoint
- **404 Not Found**: Resource does not exist
- **409 Conflict**: Resource state conflict (e.g., trying to update deleted entity)
- **422 Unprocessable Entity**: Semantic error (invalid state transition, business rule violation)
- **500 Internal Server Error**: Server error

### Error Response Format

All error responses follow this format:

```json
{
  "code": "string",
  "message": "string",
  "details": "string (optional)",
  "timestamp": "ISO 8601 datetime"
}
```

Example:
```json
{
  "code": "ORGANISATION_NOT_FOUND",
  "message": "Organisation with OVO-nummer OVO000999 does not exist",
  "timestamp": "2026-03-17T10:30:00Z"
}
```

### Authentication & Authorization

All internal APIs require authentication via one of:
- **OAuth2**: Access token in Authorization header (`Bearer <token>`)
- **JWT**: JSON Web Token in Authorization header
- **Session Cookie**: Existing session from login

**Authorization Scopes/Roles** (to be documented per endpoint):
- `admin`: Full access to all endpoints
- `editor`: Read and write access to most endpoints
- `viewer`: Read-only access
- `delegated`: Limited access based on delegated permissions

Endpoints that require specific roles will be marked with:
```
x-authorization: [roles_list]
```

### Query Parameters & Filtering

Common query parameters for list/search endpoints:

- `skip` (integer): Number of items to skip (pagination)
- `take` (integer): Number of items to return (max 100)
- `filter` (string): Filter by name, type, status (endpoint-specific)
- `sort` (string): Sort field and direction (e.g., "name asc", "validFrom desc")

### Request/Response Content Types

- **Request**: `application/json` (POST, PUT, PATCH bodies)
- **Response**: `application/json` (all responses)
- **Accept Header**: Clients should send `Accept: application/json`

---

## OpenAPI Specification Sections

### Paths (Endpoints)

Each endpoint path will document:
- Operation ID: Unique identifier for the endpoint
- Summary: Brief description (1-2 sentences)
- Description: Detailed explanation of behavior
- Parameters: Path, query, header parameters
- RequestBody: If POST/PUT/PATCH, schema of request
- Responses: Schema and description for each status code
- Security: Which OAuth2 scopes or roles required
- Tags: Categorization (Organisation, Delegation, Report, etc.)

Example structure:
```yaml
paths:
  /api/v1/organisations/{ovoNumber}:
    get:
      summary: Get organisation by OVO-nummer
      operationId: GetOrganisationByOvoNumber
      parameters:
        - name: ovoNumber
          in: path
          required: true
          schema:
            type: string
            pattern: 'OVO\d{6}'
      responses:
        200:
          description: Organisation found
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/Organisatie'
        401:
          description: Unauthorized
        403:
          description: Forbidden
        404:
          description: Organisation not found
      security:
        - oauth2: [viewer]
```

### Components (Schemas)

Reusable schemas for all entities (Organisatie, Orgaan, Address, Contact, Building, error responses)

### Security Schemes

Definition of OAuth2/JWT authentication (already in public API spec; will be referenced)

---

## Swagger UI Display

### Organization

Endpoints will be grouped by resource type:
- **Organisations**: CRUD operations for organisations
- **Governance**: Orgaan (governance body) operations
- **Addresses**: Address management
- **Contacts**: Contact information management
- **Buildings**: Building management
- **Delegations**: Delegation and permission management
- **Reports**: Reporting endpoints

### Interactive Features

- **Try It Out**: Developers can test endpoints with real/example data
- **Parameter Validation**: Swagger UI validates required parameters before sending
- **Response Examples**: Show successful (200/201) and error (400/404/422) responses
- **Schema Documentation**: Field types, formats, validation rules visible on hover

---

## Implementation Considerations

### Attribute Requirements

Each controller action must have:

```csharp
[HttpGet("{ovoNumber}")]
[ProducesResponseType(typeof(OrganisationDto), 200)]
[ProducesResponseType(401)]
[ProducesResponseType(403)]
[ProducesResponseType(404)]
[Authorize(Roles = "viewer,editor,admin")]
public async Task<IActionResult> GetOrganisation(string ovoNumber)
{
    // Implementation
}
```

### XML Documentation Comments

Controller and DTO classes should have XML documentation:

```csharp
/// <summary>
/// Get organisation by OVO-nummer
/// </summary>
/// <param name="ovoNumber">Organisation identifier (format: OVO + 6 digits)</param>
/// <returns>Organisation details if found</returns>
public async Task<IActionResult> GetOrganisation(string ovoNumber)
```

### Response Type Models

DTOs used in responses must be documented:

```csharp
/// <summary>
/// Organisation data transfer object
/// </summary>
public class OrganisationDto
{
    /// <summary>OVO-nummer identifier</summary>
    public string Id { get; set; }
    
    /// <summary>Organisation name</summary>
    public string Name { get; set; }
    // ... other fields
}
```

