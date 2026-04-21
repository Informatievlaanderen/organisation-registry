# 003 Keycloak Demo — Local Keycloak and Test Stabilization Fixes

This note documents the local Keycloak and test stabilization fixes that were added on top of the main implementation.

It focuses on the follow-up changes needed to make the local demo environment and test suite behave correctly.

## Files In Scope

- [demo/k8s/ingress.yaml](/Users/oussamasadik/Documents/GitHub/organisation-registry/demo/k8s/ingress.yaml)
- [src/OrganisationRegistry.Api/Infrastructure/Security/BffClaimsTransformation.cs](/Users/oussamasadik/Documents/GitHub/organisation-registry/src/OrganisationRegistry.Api/Infrastructure/Security/BffClaimsTransformation.cs)
- [src/OrganisationRegistry.Api/Infrastructure/Startup.cs](/Users/oussamasadik/Documents/GitHub/organisation-registry/src/OrganisationRegistry.Api/Infrastructure/Startup.cs)
- [src/OrganisationRegistry.Api/Security/SecurityService.cs](/Users/oussamasadik/Documents/GitHub/organisation-registry/src/OrganisationRegistry.Api/Security/SecurityService.cs)
- [test/OrganisationRegistry.Api.IntegrationTests/ApiFixture.cs](/Users/oussamasadik/Documents/GitHub/organisation-registry/test/OrganisationRegistry.Api.IntegrationTests/ApiFixture.cs)
- [test/OrganisationRegistry.Api.IntegrationTests/IntegrationTestConfigurationTests.cs](/Users/oussamasadik/Documents/GitHub/organisation-registry/test/OrganisationRegistry.Api.IntegrationTests/IntegrationTestConfigurationTests.cs)
- [test/OrganisationRegistry.ElasticSearch.Tests/ElasticSearchFixture.cs](/Users/oussamasadik/Documents/GitHub/organisation-registry/test/OrganisationRegistry.ElasticSearch.Tests/ElasticSearchFixture.cs)
- [test/OrganisationRegistry.ElasticSearch.Tests/appsettings.json](/Users/oussamasadik/Documents/GitHub/organisation-registry/test/OrganisationRegistry.ElasticSearch.Tests/appsettings.json)
- [test/OrganisationRegistry.UnitTests/Security/AuthenticationSetupTests.cs](/Users/oussamasadik/Documents/GitHub/organisation-registry/test/OrganisationRegistry.UnitTests/Security/AuthenticationSetupTests.cs)

## Changes

### 1. Ingress host rules explicitly support `*.localhost`

Files:

- [demo/k8s/ingress.yaml](/Users/oussamasadik/Documents/GitHub/organisation-registry/demo/k8s/ingress.yaml)

What changed:

- Traefik routes were changed from only `HostRegexp(...)` to explicit `Host('x.localhost') || HostRegexp(...)` rules for:
  - `keycloak`
  - `api`
  - `ui`
  - `app`
  - `m2m`
  - `seq`
  - `opensearch`
  - `mock`

Why needed:

- The local demo setup is accessed through `*.localhost:9080`.
- With the previous regex-only setup, these hostnames were not always matched correctly in local use.
- That made healthy services look unavailable from the browser and from local test traffic.

### 2. EditApi policy evaluation now supports multi-scope Keycloak tokens

Files:

- [src/OrganisationRegistry.Api/Infrastructure/Startup.cs](/Users/oussamasadik/Documents/GitHub/organisation-registry/src/OrganisationRegistry.Api/Infrastructure/Startup.cs)
- [src/OrganisationRegistry.Api/Security/SecurityService.cs](/Users/oussamasadik/Documents/GitHub/organisation-registry/src/OrganisationRegistry.Api/Security/SecurityService.cs)
- [test/OrganisationRegistry.UnitTests/Security/AuthenticationSetupTests.cs](/Users/oussamasadik/Documents/GitHub/organisation-registry/test/OrganisationRegistry.UnitTests/Security/AuthenticationSetupTests.cs)

What changed:

- `Startup` no longer uses plain `RequireClaim(scope, ...)` for EditApi policies.
- It now parses all `scope` claims, splits space-separated scope values, and matches against the required scopes.
- `SecurityService.GetRequiredUser()` now does the same parsing before deciding whether the caller is a well-known M2M client.
- A unit test was added for the case where Keycloak returns multiple scopes in a single `scope` claim.

Why needed:

- Real Keycloak introspection does not always return scopes in the exact shape the old code expected.
- A token can contain multiple scopes in one claim, for example `"info cjmbeheerder"`.
- Without parsing, valid machine-to-machine clients miss the fallback path and fail as if they were missing human claims such as first name and last name.

### 3. `BffClaimsTransformation` was narrowed back to BFF user claim mapping

Files:

- [src/OrganisationRegistry.Api/Infrastructure/Security/BffClaimsTransformation.cs](/Users/oussamasadik/Documents/GitHub/organisation-registry/src/OrganisationRegistry.Api/Infrastructure/Security/BffClaimsTransformation.cs)

What changed:

- The class now only transforms introspected user claims where `given_name` and `family_name` must be mapped to internal claim types.
- The temporary machine-to-machine scope normalization logic was removed.
- The identity cloning logic was kept cleaner by replacing the transformed identity inside the principal rather than rebuilding a single-identity principal.

Why needed:

- You explicitly preferred the original design where EditApi scope handling lives in `Startup` and `SecurityService`.
- This keeps responsibilities clear:
  - `BffClaimsTransformation` handles BFF/introspection user claim normalization
  - `Startup` handles policy evaluation
  - `SecurityService` handles M2M user fallback logic

### 4. API integration tests now use a dynamic local API port

Files:

- [test/OrganisationRegistry.Api.IntegrationTests/ApiFixture.cs](/Users/oussamasadik/Documents/GitHub/organisation-registry/test/OrganisationRegistry.Api.IntegrationTests/ApiFixture.cs)

What changed:

- `ApiFixture` no longer assumes `http://localhost:5000`.
- It now picks a free TCP port once and binds the test host to that port.
- The fixture `HttpClient` and helper clients use the same dynamic endpoint.

Why needed:

- Port `5000` is often already in use by another local process.
- Rider and `dotnet test` can run multiple test sessions, which creates port collisions with a fixed endpoint.
- A dynamic port removes that source of flaky failures.

### 5. API integration tests use real Keycloak M2M tokens

Files:

- [test/OrganisationRegistry.Api.IntegrationTests/ApiFixture.cs](/Users/oussamasadik/Documents/GitHub/organisation-registry/test/OrganisationRegistry.Api.IntegrationTests/ApiFixture.cs)

What changed:

- `CreateMachine2MachineClientFor()` no longer uses fake `token-{client}-{scope}` values.
- The fixture now requests a real `client_credentials` access token from Keycloak using the configured `EditApi:Authority`.
- It fails fast with a descriptive error if Keycloak token retrieval fails.

Why needed:

- The branch goal is to validate the real Keycloak integration, not a fake local token shortcut.
- Using real tokens catches the actual auth issues:
  - wrong authority
  - wrong client id / secret
  - wrong scope handling
  - wrong introspection behaviour

### 6. Integration tests now read local machine overrides

Files:

- [test/OrganisationRegistry.Api.IntegrationTests/ApiFixture.cs](/Users/oussamasadik/Documents/GitHub/organisation-registry/test/OrganisationRegistry.Api.IntegrationTests/ApiFixture.cs)
- [test/OrganisationRegistry.Api.IntegrationTests/IntegrationTestConfigurationTests.cs](/Users/oussamasadik/Documents/GitHub/organisation-registry/test/OrganisationRegistry.Api.IntegrationTests/IntegrationTestConfigurationTests.cs)

What changed:

- Integration-test config loading now checks:
  - `appsettings.json`
  - `appsettings.{Environment.MachineName}.json`
  - `appsettings.{Environment.MachineName.ToLowerInvariant()}.json`
- The config test now reads those same override files.
- `ApiFixture` still provides Keycloak fallback defaults through in-memory config when local values are absent.

Why needed:

- You wanted a local-override solution without forcing shared config changes every time.
- Some machines already have a machine-specific file, and casing differences in the filename can prevent it from loading.
- The config test previously ignored local overrides entirely, which made local setup corrections invisible to that test.

### 7. Elasticsearch tests now point to the running local OpenSearch route

Files:

- [test/OrganisationRegistry.ElasticSearch.Tests/appsettings.json](/Users/oussamasadik/Documents/GitHub/organisation-registry/test/OrganisationRegistry.ElasticSearch.Tests/appsettings.json)
- [test/OrganisationRegistry.ElasticSearch.Tests/ElasticSearchFixture.cs](/Users/oussamasadik/Documents/GitHub/organisation-registry/test/OrganisationRegistry.ElasticSearch.Tests/ElasticSearchFixture.cs)

What changed:

- Elasticsearch test config now uses `http://opensearch.localhost:9080/` for read and write connections.
- The Elasticsearch fixture no longer loads machine-specific appsettings overrides.

Why needed:

- In the current local demo environment, OpenSearch is exposed via Traefik on `opensearch.localhost:9080`, not `localhost:9200`.
- A machine-specific override copied into the test output could silently point tests back to `localhost:9200`, causing connection-refused failures even though OpenSearch was up.

## Summary

These fixes solve four concrete local problems:

1. local hostnames were not consistently routed through Traefik
2. real Keycloak EditApi tokens with multiple scopes were not interpreted correctly
3. integration tests were too dependent on fixed ports and shared config
4. Elasticsearch tests were still targeting the wrong OpenSearch endpoint
