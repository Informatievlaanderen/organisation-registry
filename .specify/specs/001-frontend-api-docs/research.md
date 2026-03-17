# Research: Internal/Screen API OpenAPI Documentation

**Feature**: 001-frontend-api-docs  
**Date**: 2026-03-17  
**Scope**: Research findings for OpenAPI/Swagger documentation of internal Backoffice APIs

## Decisions & Rationale

### Decision 1: OpenAPI Version Selection

**Decision**: Match the existing OpenAPI version used for public API documentation

**Rationale**: 
- Ensures consistency across the codebase
- Allows reuse of existing Swashbuckle configuration and tooling
- Reduces maintenance burden (single version to upgrade, single Swagger UI instance)
- Enables developers to reference one Swagger documentation for both public and internal APIs

**Alternatives Considered**:
- Use latest OpenAPI 3.1: Would provide better JSON Schema support, but public APIs may not be on 3.1; version mismatch adds complexity
- Use OpenAPI 2.0: Legacy version, not recommended for new specifications

**Implementation Impact**: Determine current OpenAPI version during Phase 1 planning

---

### Decision 2: Specification Generation Method

**Decision**: Auto-generate OpenAPI spec from C# Swashbuckle annotations in code

**Rationale**:
- Spec stays in sync with actual API implementation automatically
- Reduces manual maintenance burden (documentation drift is eliminated)
- Swashbuckle is already integrated and proven in the project
- Follows existing project pattern for public API documentation
- Build-time generation enables CI/CD validation

**Alternatives Considered**:
- Manual OpenAPI YAML/JSON files: Higher maintenance cost, prone to drift, but more control
- OpenAPI generation from attributes without Swashbuckle: Reinvents existing tooling

**Implementation Impact**: 
- Enhance C# controller attributes with Swagger metadata (ProducesResponseType, Consumes, Produces, Description, etc.)
- Configure Swashbuckle to include Backoffice controllers in generated spec
- Integrate spec generation into build pipeline

---

### Decision 3: Scope - Which APIs to Document

**Decision**: Document ALL internal/screen APIs used by Angular SPA (all controllers in `src/OrganisationRegistry.Api/Backoffice/`)

**Rationale**:
- Angular SPA uses these APIs almost exclusively; comprehensive documentation is essential
- All-or-nothing approach prevents gaps (developers need to know all available endpoints)
- Auto-generation makes 100% coverage achievable (no manual selection bias)
- Easier to maintain one complete spec than multiple partial specs

**Alternatives Considered**:
- Document only "core" APIs first, rest later: Leaves developers confused about what exists; creates maintenance fragmentation
- Document specific controllers by request: Ad-hoc and reactive; violates DRY principle

**Implementation Impact**: 
- Configure Swashbuckle to include all Backoffice controllers (not just public ones)
- May require filtering to exclude internal/helper endpoints if needed

---

### Decision 4: Swagger UI Hosting & Access

**Decision**: Keep current approach - host Swagger UI the same way as public API documentation

**Rationale**:
- Consistent developer experience (one place to find all API docs)
- Leverages existing infrastructure and deployment process
- No additional tooling or hosting complexity
- Developers can compare public vs. internal APIs side-by-side if helpful

**Alternatives Considered**:
- Separate Swagger UI endpoint for internal APIs: Fragments documentation, duplicates infrastructure
- Publish to external docs portal: Out of scope (handled by separate documentation project)
- Interactive API testing tools beyond Swagger UI: Out of scope; Swagger UI "Try It Out" is sufficient

**Implementation Impact**:
- No infrastructure changes needed
- Configure Swagger UI to display both public and internal specs (or separate endpoints)

---

### Decision 5: Response Example & Error Documentation

**Decision**: Auto-generate response examples from actual API implementation; document error scenarios explicitly

**Rationale**:
- Response examples generated from real responses are always accurate
- Error documentation requires explicit specification (Swashbuckle ProducesResponseType attributes)
- Developers learn from concrete, realistic examples
- Auto-generated examples prevent "outdated example" problems

**Alternatives Considered**:
- Manual example creation: Maintenance burden; prone to drift
- No examples: Defeats purpose of interactive testing

**Implementation Impact**:
- Add ProducesResponseType attributes to controllers for each possible response (200, 400, 401, 403, 404, 422, 500, etc.)
- Add XML documentation comments explaining error conditions
- Test that Swagger UI displays all documented responses

---

## Next Steps (Phase 1)

1. **Determine current OpenAPI version**: Check `paket.dependencies` and existing Swashbuckle configuration
2. **Audit Backoffice controllers**: Inventory all controllers and endpoints to document
3. **Define data model**: Map request/response types to OpenAPI schema
4. **Create OpenAPI contracts**: Define spec structure and Swagger UI configuration
5. **Generate quickstart guide**: Document how to access and use the Swagger UI

