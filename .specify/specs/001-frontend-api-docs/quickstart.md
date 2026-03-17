# Quickstart: Internal/Screen API Documentation

**Feature**: 001-frontend-api-docs  
**Date**: 2026-03-17  
**Audience**: Angular SPA front-end developers

## What Is This?

This is the **Swagger/OpenAPI documentation for internal Organisation Registry APIs** used by the Angular SPA. It provides interactive API documentation with the ability to test endpoints directly from the browser.

## Accessing the Swagger UI

### During Development

While developing locally, access the Swagger UI at:

```
http://localhost:5000/swagger/index.html
```

(Adjust port if running on different port - check your launch settings)

### In Production

The Swagger UI is available at:

```
https://registry.example.com/api/docs
```

Or check your deployment documentation for the exact URL.

## Using Swagger UI

### 1. Browse Available Endpoints

The left sidebar groups endpoints by resource type:
- **Organisations**: Create, read, update, delete organisations
- **Governance**: Manage Organen (governance bodies)
- **Addresses**: Manage addresses
- **Contacts**: Manage contact information
- **Buildings**: Manage buildings
- **Delegations**: Manage permissions
- **Reports**: Generate reports

Click any endpoint to expand and see details.

### 2. Understanding an Endpoint

Each endpoint shows:

```
GET /api/v1/organisations/{ovoNumber}

Summary:       Get organisation by OVO-nummer
Description:   Retrieves details of a single organisation
Parameters:    ovoNumber (path parameter, required)
Response:      Organisation object (200), or error (401/403/404)
Authentication: Requires OAuth2 viewer scope
```

### 3. Testing an Endpoint

To test an endpoint interactively:

1. **Click the endpoint** to expand it
2. **Click "Try It Out"** button (bottom right of endpoint details)
3. **Fill in parameters**:
   - Path parameters (e.g., `ovoNumber`) are required
   - Query parameters are optional (will show defaults)
   - Request body (for POST/PUT/PATCH) shown as JSON editor
4. **Click "Execute"** button
5. **View the response**:
   - Response status code (200, 404, etc.)
   - Response body (JSON) with actual data or error
   - Response headers
   - cURL command equivalent (for reference)

### 4. Authentication

Most endpoints require authentication. To add your credentials:

1. **Click the lock icon** in the top right
2. **Select authentication method**:
   - OAuth2: You'll be redirected to login page
   - Authorization header: Paste your token/API key
3. **All subsequent requests** will include your credentials

### 5. Understanding Responses

**Success Response (200/201)**:
```json
{
  "id": "OVO000123",
  "name": "Stad Antwerpen",
  "status": "Active",
  "addresses": [
    {
      "id": "addr-1",
      "street": "Groenplaats",
      "number": "20",
      "zipCode": "2000",
      "city": "Antwerpen",
      "country": "BE"
    }
  ]
}
```

**Error Response (400/404/422)**:
```json
{
  "code": "ORGANISATION_NOT_FOUND",
  "message": "Organisation with OVO-nummer OVO000999 does not exist",
  "timestamp": "2026-03-17T10:30:00Z"
}
```

### 6. Common Error Codes

| Code | HTTP Status | Meaning | Action |
|------|-------------|---------|--------|
| ORGANISATION_NOT_FOUND | 404 | OVO-nummer doesn't exist | Verify the OVO-nummer is correct (format: OVO + 6 digits) |
| UNAUTHORISED | 401 | Not authenticated | Login and get a valid token |
| FORBIDDEN | 403 | Authenticated but lack permission | Contact admin to request access |
| VALIDATION_ERROR | 400 | Invalid input parameters | Check field types and required fields |
| BUSINESS_RULE_VIOLATION | 422 | Cannot perform operation (state conflict) | Check current state and try different operation |

## Common Tasks

### List All Organisations

```
GET /api/v1/organisations?skip=0&take=20
```

Parameters:
- `skip`: Number of items to skip (default 0)
- `take`: Number of items to return (default 20, max 100)
- `filter` (optional): Filter by name

### Get Single Organisation

```
GET /api/v1/organisations/{ovoNumber}
```

Example:
```
GET /api/v1/organisations/OVO000123
```

### Create New Organisation

```
POST /api/v1/organisations
Content-Type: application/json

{
  "name": "New Organisation",
  "status": "Active",
  "validFrom": "2026-03-17"
}
```

### Update Organisation

```
PUT /api/v1/organisations/{ovoNumber}
Content-Type: application/json

{
  "name": "Updated Name",
  "status": "Inactive"
}
```

### Delete Organisation

```
DELETE /api/v1/organisations/{ovoNumber}
```

## Key Concepts

### OVO-nummer
- **Format**: OVO + 6 digits (e.g., OVO000123)
- **Uniqueness**: Unique identifier for each organisation
- **Immutable**: Never changes once assigned
- **Usage**: Used in URL paths and as primary identifier

### Organisation Status
- **Active**: Organisation is currently operating
- **Inactive**: Organisation is not operating but not deleted
- **Archived**: Historical organisation, no longer active

### Governance Bodies (Organen)
- **Definition**: Decision-making bodies within an organisation (e.g., City Council, Executive Board)
- **Access**: Retrieved via endpoints under "Governance" tag
- **Relationships**: Each Orgaan belongs to one Organisatie

### Addresses
- **Types**: Registered address (official), Operational address (day-to-day), Visiting address (physical location)
- **Validity**: Each address has validFrom and validTo dates

### Delegations
- **Purpose**: Allow delegation of administrative permissions to users
- **Scope**: Can delegate access to specific organisations or bodies

## Troubleshooting

### "401 Unauthorized"
- **Cause**: Not authenticated or token expired
- **Solution**: Click lock icon and re-authenticate

### "403 Forbidden"
- **Cause**: Authenticated but don't have required permissions
- **Solution**: Contact your administrator to request access

### "404 Not Found"
- **Cause**: Resource doesn't exist or wrong OVO-nummer format
- **Solution**: Verify OVO-nummer format (OVO + 6 digits)

### "400 Bad Request" or "422 Unprocessable Entity"
- **Cause**: Invalid input or business rule violation
- **Solution**: Check error message for details; verify parameter types and formats

### Endpoint not appearing in Swagger UI
- **Cause**: Endpoint may require specific role or might be disabled
- **Solution**: Check authentication and permissions; try different authentication scope

## Getting Help

- **Documentation**: See this quickstart and endpoint descriptions in Swagger UI
- **Support**: Contact the backend team or #api-help Slack channel
- **Issues**: File an issue in the project repository

## Next Steps

1. **Explore the API**: Browse different endpoint groups in Swagger UI
2. **Test an endpoint**: Click "Try It Out" on a GET endpoint to see live data
3. **Understand the data**: Read field descriptions to understand what each field means
4. **Start integrating**: Use example requests in your Angular application

Happy exploring! 🚀

