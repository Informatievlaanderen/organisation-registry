# Phase 3 Plan: Validation and Testing

## Objective

Validate that all 126 Backoffice controllers are properly documented in the generated OpenAPI specification and that the OpenAPI schema can be consumed by frontend API documentation tools.

## Phase Duration

**Estimated**: 12 hours

## Phase Overview

| Task | Duration | Status | Owner |
|------|----------|--------|-------|
| **3.1** Verify OpenAPI spec generation | 3h | Pending | OpenCode |
| **3.2** Validate Swagger UI functionality | 2h | Pending | OpenCode |
| **3.3** Ensure 100% endpoint coverage | 3h | Pending | OpenCode |
| **3.4** Write integration tests | 2h | Optional | OpenCode |
| **3.5** Document API error responses | 2h | Optional | OpenCode |
| **Total** | 12h | - | - |

## Detailed Tasks

### Task 3.1: Verify OpenAPI Spec Generation (3 hours)

**Objective**: Ensure the OpenAPI specification includes all documented endpoints with proper schemas.

**Deliverables**:
- Generate OpenAPI spec from running API
- Validate OpenAPI schema structure
- Confirm all 126 controllers appear in spec
- Check for any documentation gaps

**Acceptance Criteria**:
- OpenAPI spec is valid (passes OpenAPI Validator)
- Contains ~290 Backoffice endpoints
- All required fields present (summary, description, response codes)
- No missing required fields for Swagger UI rendering

**Commands**:
```bash
# Start API
dotnet run --project src/OrganisationRegistry.Api/

# Access Swagger JSON at:
# http://localhost:5000/api/v1/swagger.json

# Validate against OpenAPI spec
npm install -g @apidevtools/swagger-cli
swagger-cli validate http://localhost:5000/api/v1/swagger.json
```

**Deliverable Files**:
- `task-3.1-openapi-validation.md` - Validation report
- `openapi-spec-sample.json` - Sample of generated spec

### Task 3.2: Validate Swagger UI Functionality (2 hours)

**Objective**: Ensure Swagger UI renders correctly and all endpoints are accessible.

**Deliverables**:
- Swagger UI loads and renders properly
- All endpoints appear in the interface
- Endpoint documentation is readable
- Try-it-out functionality works

**Acceptance Criteria**:
- Swagger UI accessible at `/api/v1/docs` (or similar endpoint)
- Page loads without JavaScript errors
- Can expand controller groups
- Can view all 126 controller groups
- Can see endpoint summaries and descriptions
- Try-it-out button functional

**Manual Testing Steps**:
1. Start API: `dotnet run --project src/OrganisationRegistry.Api/`
2. Navigate to Swagger UI: `http://localhost:5000/api/v1/docs`
3. Expand "Backoffice" section
4. Verify all controller groups appear
5. Click on various endpoints
6. Verify summaries and response codes display
7. Try to execute one GET endpoint to test functionality

**Deliverable Files**:
- `task-3.2-swagger-ui-verification.md` - UI testing report
- Screenshots of Swagger UI if needed

### Task 3.3: Ensure 100% Endpoint Coverage (3 hours)

**Objective**: Verify that all ~290 Backoffice endpoints appear in the OpenAPI spec with complete documentation.

**Deliverables**:
- Endpoint inventory from generated spec
- Coverage report showing 100% documentation
- Identify any missing or undocumented endpoints
- Cross-reference with controller audit

**Acceptance Criteria**:
- All 126 controllers documented in spec
- All ~290 endpoints have summaries
- All endpoints have proper response codes
- No endpoints missing documentation
- Coverage report shows 100%

**Commands**:
```bash
# Parse OpenAPI spec to find gaps
jq '.paths | length' swagger.json  # Count endpoints

# Find endpoints without summaries
jq '.paths[] | select(.get.summary == null)' swagger.json

# Find endpoints without response codes
jq '.paths[] | select(.get.responses == null)' swagger.json
```

**Deliverable Files**:
- `task-3.3-endpoint-coverage-report.md` - Coverage analysis
- `endpoint-inventory.json` - Full endpoint list from spec

### Task 3.4: Write Integration Tests (2 hours - Optional)

**Objective**: Create integration tests to verify OpenAPI documentation is accurate.

**Deliverables**:
- Integration test class for OpenAPI validation
- Tests for endpoint discovery
- Tests for response code accuracy
- Tests for required fields

**Test Scenarios**:
```
Given: API is running
When: I request the OpenAPI spec
Then: I receive a valid OpenAPI document with:
  - All 126 controllers documented
  - ~290 endpoints with summaries
  - Complete response codes
  - No validation errors

Given: I query a specific documented endpoint
When: I invoke it
Then: The response matches the documented schema
  - Response code is documented
  - Response body matches description
```

**Deliverable Files**:
- `OpenApiDocumentationTests.cs` - Integration tests
- Test results report

**Note**: This task is optional per project constitution (which exempts controllers from test coverage).

### Task 3.5: Document API Error Responses (2 hours - Optional)

**Objective**: Create comprehensive documentation of common API error responses.

**Deliverables**:
- Error response documentation
- Common HTTP status codes guide
- Error response schema examples
- Troubleshooting guide

**Content**:
- 400 Bad Request - Validation failures
- 401 Unauthorized - Authentication required
- 403 Forbidden - Authorization denied
- 404 Not Found - Resource not found
- 500 Internal Server Error - Server failures
- 503 Service Unavailable - Maintenance/downtime

**Deliverable Files**:
- `error-responses.md` - Complete error documentation
- `common-errors.md` - Troubleshooting guide
- `status-codes.md` - HTTP status code reference

**Note**: This task is optional and provides additional documentation value.

## Success Criteria for Phase 3

- ✅ OpenAPI spec generated successfully
- ✅ All 126 controllers in spec
- ✅ All ~290 endpoints documented
- ✅ Swagger UI loads and functions correctly
- ✅ 100% endpoint coverage verified
- ✅ No validation errors in OpenAPI schema
- ✅ Optional: Integration tests passing (Task 3.4)
- ✅ Optional: Error documentation complete (Task 3.5)

## Risk Assessment

| Risk | Impact | Mitigation |
|------|--------|-----------|
| OpenAPI validation fails | Medium | Verify schema structure against OpenAPI spec |
| Swagger UI won't load | High | Check API configuration for Swagger middleware |
| Missing endpoints in spec | High | Compare controller count to spec endpoint count |
| XML comments not parsed | Medium | Verify XML file location in Startup.cs |

## Deliverable Summary

**Required Deliverables** (Tasks 3.1-3.3):
- OpenAPI validation report
- Swagger UI verification report
- Endpoint coverage report (100%)
- Analysis documents

**Optional Deliverables** (Tasks 3.4-3.5):
- Integration tests
- Error response documentation

## Output

All analysis documents will be placed in:
```
.specify/specs/001-frontend-api-docs/
  ├── task-3.1-openapi-validation.md
  ├── task-3.2-swagger-ui-verification.md
  ├── task-3.3-endpoint-coverage-report.md
  ├── task-3.4-integration-tests.md (optional)
  └── phase-3-summary.md
```

## Next Phase

Upon successful completion of Phase 3:
- Proceed to **Phase 4: CI/CD Integration** (10 hours)
- Focus on build pipeline validation
- Automate OpenAPI spec generation
- Deploy API documentation

## Notes

- All manual verification steps can be documented with screenshots
- OpenAPI spec should be validated against official OpenAPI 3.0 or 3.1 spec
- Swagger UI should render at standard endpoint `/api/{version}/docs`
- All 10 previously undocumented endpoints will be in the spec (from Phase 2)
